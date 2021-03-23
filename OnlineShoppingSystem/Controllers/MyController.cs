using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShoppingSystem.Models;
using System.Web.Security;
using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingSystem.Controllers
{
    public class MyController : Controller
    {
        OnlineShoppingSystemDBForWEBEntities db = new OnlineShoppingSystemDBForWEBEntities();

        public ActionResult Home()
        {
            List<HighestStarproduct> modelHighestStar = new List<HighestStarproduct>();
            List<BestDiscountedProduct> modelBestDiscounteds = new List<BestDiscountedProduct>();
            List<NewProduct> modelNewProducts = new List<NewProduct>();
            List<ProductWithBestSelling> modelproductWithBestSellings = new List<ProductWithBestSelling>();

            modelHighestStar = db.HighestStarproduct.ToList();
            modelBestDiscounteds = db.BestDiscountedProduct.ToList();
            modelNewProducts = db.NewProduct.ToList();
            modelproductWithBestSellings = db.ProductWithBestSelling.ToList();

            ModelProducts modelProducts = new ModelProducts();
            modelProducts.modelHighestStar = modelHighestStar;
            modelProducts.modelNewProducts = modelNewProducts;
            modelProducts.modelDiscounteds = modelBestDiscounteds;
            modelProducts.modelProductWithBestSelling = modelproductWithBestSellings;
            return View(modelProducts);
        }
        
        public ActionResult Search(string searching)
        {
            //take the product name and search
            Product productDetail;
            if (!String.IsNullOrEmpty(searching))
            {
                productDetail = db.Product.Where(s => s.ProductName.Contains(searching)).Select(s => s).FirstOrDefault();
                //if product is in the database.
                if (productDetail != null)
                {
                    System.Threading.Thread.Sleep(1000);
                    return RedirectToAction("ProductDetails", "Product", new { PID = productDetail.ProductID, Pname = productDetail.ProductName });
                }
            }
            return Redirect("Home");
        }

        public ActionResult About()
        {
            return View();
        }
        
        public ActionResult MyCart()
        {   //if the customer logged in.
            if (Session["CustomerID"] != null)
            {
                int UserID = (int)Session["CustomerID"];
                List<ModelMyCartItems> ListMyCarts = new List<ModelMyCartItems>();
                var myCartItems = (from c in db.Cart
                                   join ct in db.CartItem on c.CartID equals ct.CartID
                                   join p in db.Product on ct.ProductID equals p.ProductID
                                   where c.CustomerID == UserID
                                   select new { c, ct, p }).ToList();

                ModelMyCart modelMyCart = new ModelMyCart();
                foreach (var i in myCartItems)
                {
                    ListMyCarts.Add(new ModelMyCartItems
                    {
                        CartItemID = i.ct.CartItemID,
                        CartID = i.c.CartID,
                        ProductID = i.p.ProductID,
                        ProductPrice = i.p.ProductPrice,
                        ProductDiscountedPrice = i.p.ProductDiscountedPrice,
                        Quantity = i.ct.Quantity,
                        ProductImageUrl = i.p.ProductImagePath,
                        ProductName = i.p.ProductName,
                        ProductStock = i.p.ProductStock,
                        TotalItemPrice = i.ct.Price,
                        TotalDiscountedPrice = i.ct.DiscountedPrice,
                    });
                }
                modelMyCart.listMyCarts = ListMyCarts;
                if (!modelMyCart.Empty)
                {
                    modelMyCart.TotalCartPrice = myCartItems[0].c.TotalCost;//all items have same cart price. So take first one.
                    modelMyCart.TotalDiscountedCartPrice = myCartItems[0].c.TotalDiscountedCost;
                    modelMyCart.CartID = myCartItems[0].c.CartID;
                }
                return View(modelMyCart);
            }
            return View();
        }
        [HttpPost]
        public ActionResult MyCart(int ID)
        {
            //drop item from cart.
            var CartItem = new CartItem();
            var Product = new Product();
            var Cart = new Cart();

            int UserID =(int)Session["CustomerID"];
            Cart = db.Cart.Where(x => x.CustomerID == UserID).FirstOrDefault();
            CartItem = db.CartItem.Where(x => x.ProductID == ID && x.CartID==Cart.CartID).FirstOrDefault();
            Product = db.Product.Where(x => x.ProductID == ID).FirstOrDefault();
            if (CartItem.Quantity == 1)//if the quantity is 1, so remove the item.
            {
                db.CartItem.Remove(CartItem);
            }
            else
            {
                CartItem.Quantity -= 1;//if the quantity is higher than 1, update the database.
                CartItem.DiscountedPrice -= Product.ProductDiscountedPrice;
                CartItem.Price -= Product.ProductPrice;
            }
            Cart.TotalCost -= Product.ProductPrice;
            Cart.TotalDiscountedCost -= Product.ProductDiscountedPrice;
            db.SaveChanges();
            ViewBag.Message = "Item has been deleted from your cart.";
            System.Threading.Thread.Sleep(1000);
            return RedirectToAction("MyCart", "My");
        }

        public ActionResult Payment(int CartID)
        {
            var myPayment = (from ci in db.CustomerInformation
                             join c in db.Cart on ci.CustID equals c.CustomerID
                             where c.CartID == CartID
                             select new { c, ci }).FirstOrDefault();

            
            PaymentInformation payInfo = new PaymentInformation();
            payInfo.CustomerID = myPayment.c.CustomerID;
            payInfo.CartID = myPayment.c.CartID;
            payInfo.Address = myPayment.ci.Address1;
            payInfo.Amount = myPayment.c.TotalDiscountedCost;
            payInfo.Email = myPayment.ci.Email;
            payInfo.FirstName = myPayment.ci.FirstName;
            payInfo.LastName = myPayment.ci.LastName;
            payInfo.PhoneNumber = myPayment.ci.PhoneNumber;
            return View(payInfo);
        }
        [HttpPost]
        public ActionResult Payment(int CustID, int CartID)
        {
            Payment myPayment = new Payment();
            Invoice myInvoice = new Invoice();
            InvoiceProducts myInvoiceProducts = new InvoiceProducts();
            Random rand = new Random();
            List<Product> ProductList = new List<Product>();

            //Add payment record.
            Cart myCart = db.Cart.Where(x => x.CartID == CartID).Select(x => x).FirstOrDefault();
            ProductList = db.CartItem.Where(x => x.CartID == CartID).Select(x => x.Product).ToList();//Take Product in the cart.

            myPayment.CartID = CartID;
            myPayment.Amount = myCart.TotalDiscountedCost;
            myPayment.CustomerID = CustID;
            db.Payment.Add(myPayment);
            db.SaveChanges();

            //Add invoice record.
            myInvoice.InvoiceDate = DateTime.Now.Date;
            myInvoice.CustID = CustID;
            myInvoice.PaymentID = myPayment.PaymentID;
            //to generate random barcode number, use unique value.
            myInvoice.BarcodeNumbers = myPayment.PaymentID + rand.Next(99).ToString() + "-" + myPayment.CustomerID + rand.Next(99).ToString() + "-" + myCart.CartID + rand.Next(99).ToString();
            db.Invoice.Add(myInvoice);
            db.SaveChanges();

            foreach(var item in ProductList)
            {
                myInvoiceProducts.InvoiceID = myInvoice.InvoiceID;
                myInvoiceProducts.ProductID = item.ProductID;
                db.InvoiceProducts.Add(myInvoiceProducts);
                db.SaveChanges();
            }
            

            //Delete cartitem.
            List<CartItem> myCartList = db.CartItem.Where(x => x.CartID == CartID).Select(x => x).ToList();
            foreach(var item in myCartList)
            {
                db.CartItem.Remove(item);
            }
            db.SaveChanges();

            //Reset the cart.
            myCart.TotalCost = 0;
            myCart.TotalDiscountedCost = 0;
            db.SaveChanges();
            System.Threading.Thread.Sleep(1000);

            return RedirectToAction("MyCart", "My");
        }

        public ActionResult Invoice(string UserName)
        {
            //list the all invoices 
            List<ModelMyInvoice> myInvoiceList = new List<ModelMyInvoice>();
            var myquery = (from c in db.Customer
                           join i in db.Invoice on c.CustomerID equals i.CustID
                           join p in db.Payment on i.PaymentID equals p.PaymentID
                           where c.UserName==UserName
                           select new { c, i, p}).ToList();

            foreach (var item in myquery)
            {
                myInvoiceList.Add(new ModelMyInvoice
                {
                    InvoiceDate = item.i.InvoiceDate.ToShortDateString(),
                    BarcodeNumbers = item.i.BarcodeNumbers,
                    InvoiceID=item.i.InvoiceID,
                    Amount= (double)item.p.Amount,
                });
            }

            ModelMyInvoice modelMyInvoice = new ModelMyInvoice();
            modelMyInvoice.listMyInvoice = myInvoiceList;
            return View(modelMyInvoice);
        }

        public ActionResult PaymentDetail(int ID)
        {
            //display the payment detail.
            var myPayment = (from p in db.Payment
                             join i in db.Invoice on p.PaymentID equals i.PaymentID
                             join ci in db.CustomerInformation on i.CustID equals ci.CustID
                             where i.InvoiceID == ID
                             select new { ci,p}).FirstOrDefault();

            PaymentInformation payInfo = new PaymentInformation();
            List<Product> myProductList = new List<Product>();
            var myList = (from i in db.InvoiceProducts  
                                join p in db.Product on i.ProductID equals p.ProductID
                                where i.InvoiceID == ID
                             select new { p }).Distinct().ToList();

            foreach(var item in myList)
            {
                myProductList.Add(item.p);
            }

            payInfo.myProductsList = myProductList;
            payInfo.CustomerID = myPayment.ci.CustID;
            payInfo.CartID = myPayment.p.CartID;
            payInfo.Address = myPayment.ci.Address1;
            payInfo.Amount = (double)myPayment.p.Amount;
            payInfo.Email = myPayment.ci.Email;
            payInfo.FirstName = myPayment.ci.FirstName;
            payInfo.LastName = myPayment.ci.LastName;
            payInfo.PhoneNumber = myPayment.ci.PhoneNumber;

            return View(payInfo);
        }
        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactMessage message)
        {
            //take the message and add to the database.
            db.ContactMessage.Add(message);
            db.SaveChanges();
            System.Threading.Thread.Sleep(1000);
            return RedirectToAction("Contact", "My");
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(CustomerWithInformation custWithInformation)
        {
           
            var customer=db.Customer.Where(x => x.UserName == custWithInformation.UserName).FirstOrDefault();
            //if the user name has not been taken by someone.
            if (customer == null)
            {
                Customer Mycustomer = new Customer();
                CustomerInformation customerInfo = new CustomerInformation();
                Cart customerCart = new Cart();
                //add to  customer table.
                Mycustomer.UserName = custWithInformation.UserName;
                Mycustomer.Password = custWithInformation.Password;
                db.Customer.Add(Mycustomer);
                db.SaveChanges();
                // add to the customerInfo table
                customerInfo.CustID = Mycustomer.CustomerID;
                customerInfo.FirstName = custWithInformation.FirstName;
                customerInfo.LastName = custWithInformation.LastName;
                customerInfo.Address1 = custWithInformation.Address1;
                customerInfo.Email = custWithInformation.Email;
                customerInfo.PhoneNumber = custWithInformation.PhoneNumber;
                customerInfo.Country = custWithInformation.Country;

                db.CustomerInformation.Add(customerInfo);
                db.SaveChanges();
                //create the cart for customer.
                customerCart.CustomerID = customerInfo.CustID;
                customerCart.TotalCost = 0;
                db.Cart.Add(customerCart);
                db.SaveChanges();
                return RedirectToAction("Login", "My");
            }
            else
            {
                ViewBag.Message = "*This user name has been taken by someone!*";
                return View();
            }
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (Session["UserName"] != null && !String.IsNullOrEmpty(Session["UserName"].ToString()))
            {
                return RedirectToAction("Home", "My");

            }
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(Customer customer)
        {
            //if the username or password is wrong.
            customer = db.Customer.Where(x => x.UserName == customer.UserName && x.Password == customer.Password).FirstOrDefault();
            if (customer == null)
            {
                ViewBag.Message = "Username or paswword is wrong !";
                return View();
            }
            else
            {
                Session["CustomerID"] = customer.CustomerID;
                Session["UserName"] = customer.UserName;
                Session["Password"] = customer.Password;
                System.Threading.Thread.Sleep(1000);
                return RedirectToAction("Home", "My");
            }
            
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Home", "My");
        }
        
        public ActionResult MyProfile(string UserName)
        {
            //display the profile information for customer.
            var myQuery = db.CustomerWithInformation.Where(x => x.UserName == UserName).Select(x => x).FirstOrDefault();
            return View(myQuery);
        }

        public ActionResult UpdateInformation(int ID)
        {
            //list the shipping and contact information
            var customer = db.CustomerInformation.Find(ID);
            return View(customer);
        }

        [HttpPost]
        public ActionResult UpdateInformation(CustomerInformation customer)
        {
            //update the shipping and contact information
            CustomerInformation customerInformation = db.CustomerInformation.Where(x => x.CustID == customer.CustID).Select(x=>x).FirstOrDefault();
            customerInformation.Address1 = customer.Address1;
            customerInformation.PhoneNumber = customer.PhoneNumber;
            customerInformation.Country = customer.Country;
            string CustUserName = db.CustomerWithInformation.Where(x => x.CustID == customer.CustID).Select(x => x.UserName).FirstOrDefault();

            db.SaveChanges();
            System.Threading.Thread.Sleep(1000);
            return RedirectToAction("MyProfile","My",new { UserName= CustUserName });
        }
    }
}