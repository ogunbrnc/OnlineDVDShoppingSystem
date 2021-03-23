using OnlineShoppingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Net;


namespace OnlineShoppingSystem.Controllers
{
    public class AdminController : Controller
    {
        OnlineShoppingSystemDBForWEBEntities db = new OnlineShoppingSystemDBForWEBEntities();

        
        public ActionResult Login()
        {
            if (Session["AdminUserName"] != null && !String.IsNullOrEmpty(Session["AdminUserName"].ToString()))
            {
                return RedirectToAction("Home", "Admin");

            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(Admin admin)
        {
            admin = db.Admin.Where(x => x.UserName == admin.UserName && x.Password == admin.Password).FirstOrDefault();
            //if the admin username or password is wrong.
            if (admin == null)
            {
                ViewBag.Message = "Username or password is wrong !";
                return View();
            }
            else
            {
                Session["AdminID"] = admin.AdminID;
                Session["AdminUserName"] = admin.UserName;
                Session["AdminPassword"] = admin.Password;
                System.Threading.Thread.Sleep(1000);
                return RedirectToAction("Home", "Admin");
            }
        }
        public ActionResult Logout()
        {

            Session.Clear();
            return RedirectToAction("Home", "Admin");
        }

        public ActionResult Home()
        {
            if (Session["AdminUserName"] != null && !String.IsNullOrEmpty(Session["AdminUserName"].ToString()))
            {
                List<ProductsWithLowStock> modelLowerStockProducts = new List<ProductsWithLowStock>();
                List<ProductWithBestSelling> modelBestSellingProducts = new List<ProductWithBestSelling>();
                List<ProductWithLeastSelling> modelLeastSellingProducts = new List<ProductWithLeastSelling>();
                modelLowerStockProducts = db.ProductsWithLowStock.ToList();
                modelBestSellingProducts = db.ProductWithBestSelling.ToList();
                modelLeastSellingProducts = db.ProductWithLeastSelling.ToList();
                ModelAdmin modelAdmin = new ModelAdmin();


                modelAdmin.listlowerProducts = modelLowerStockProducts;
                modelAdmin.listBestSellingProduct = modelBestSellingProducts;
                modelAdmin.listLeastSellingProduct = modelLeastSellingProducts;
                return View(modelAdmin);
            }
            else
            {
                return RedirectToAction("Login", "Admin"); 
            }
        }
        public ActionResult ListProductWithSubCategory(int CatID, int SubID)
        {
            //take the CatID and SubCat ID and list the aall products
            List<Product> myQuery = new List<Product>();
            SubCategoryProducts myModel = new SubCategoryProducts();
            myModel.ProductCategoryID = CatID;
            myModel.ProductSubCategoryID = SubID;
            myQuery = db.Product.Where(s => s.CategoryID == CatID && s.SubCategoryID == SubID).Select(s => s).ToList();
            myModel.myProductList = myQuery;
            return View(myModel);
        }

        [HttpPost]
        public ActionResult ListProductWithSubCategory(int CatID, int SubID, string sortBy)
        {
            List<Product> myQuery = new List<Product>();
            SubCategoryProducts myModel = new SubCategoryProducts();
            switch (sortBy)
            {
                case "ascPrice":
                    myQuery = db.Product.Where(s => s.CategoryID == CatID && s.SubCategoryID == SubID).OrderBy(x => x.ProductDiscountedPrice).Select(s => s).ToList();
                    break;
                case "descPrice":
                    myQuery = db.Product.Where(s => s.CategoryID == CatID && s.SubCategoryID == SubID).OrderByDescending(x => x.ProductDiscountedPrice).Select(s => s).ToList();
                    break;
                case "descStar":
                    myQuery = db.Product.Where(s => s.CategoryID == CatID && s.SubCategoryID == SubID).OrderByDescending(x => x.ProductStar).Select(s => s).ToList();
                    break;
                case "descRate":
                    myQuery = db.Product.Where(s => s.CategoryID == CatID && s.SubCategoryID == SubID).OrderByDescending(x => x.ProductDiscountedRate).Select(s => s).ToList();
                    break;
                case "descSale":
                    myQuery = db.Product.Where(s => s.CategoryID == CatID && s.SubCategoryID == SubID).OrderByDescending(x => x.ProductNumberOfSales).Select(s => s).ToList();
                    break;
                case "descDate":
                    myQuery = db.Product.Where(s => s.CategoryID == CatID && s.SubCategoryID == SubID).OrderByDescending(x => x.ProductDate).Select(s => s).ToList();
                    break;
                case "descEvaluate":
                    myQuery = db.Product.Where(s => s.CategoryID == CatID && s.SubCategoryID == SubID).OrderByDescending(x => x.ProductNumberOfEvaluate).Select(s => s).ToList();
                    break;
                default:
                    myQuery = db.Product.Where(s => s.CategoryID == CatID && s.SubCategoryID == SubID).OrderBy(x => x.ProductDiscountedPrice).Select(s => s).ToList();
                    break;
            }
            myModel.myProductList = myQuery;
            myModel.ProductCategoryID = CatID;
            myModel.ProductSubCategoryID = SubID;
            return View(myModel);
        }



        public ActionResult ProductDetail(int PID, string PName)
        {
            //display the product detail according to the PID.
            Product myProduct = db.Product.Where(x => x.ProductID == PID).Select(x => x).FirstOrDefault();
            List<ProductComment> productComments = new List<ProductComment>();
            productComments = db.ProductComment.Where(x => x.ProductID == PID).OrderByDescending(x => x.ProductCommentID).Select(x => x).Take(5).ToList();

            ProductDetail myProductDetail = new ProductDetail();
            myProductDetail.product = myProduct;
            myProductDetail.productComments = productComments;
            return View(myProductDetail);
        }

        public ActionResult ProductUpdate(int ID)
        {
            var product = db.Product.Find(ID);
            return View(product);
        }

        [HttpPost]
        public ActionResult ProductUpdate(Product myProduct)
        {
            //Update the product information.
            Product updatedProduct = db.Product.Where(x => x.ProductID == myProduct.ProductID).FirstOrDefault();
            updatedProduct.ProductName = myProduct.ProductName;
            updatedProduct.ProductDescription = myProduct.ProductDescription;
            updatedProduct.ProductStock = myProduct.ProductStock;
            updatedProduct.ProductPrice = myProduct.ProductPrice;
            updatedProduct.ProductDiscountedPrice = myProduct.ProductDiscountedPrice;
            updatedProduct.ProductDiscountedRate = Convert.ToDecimal(((100 * (myProduct.ProductPrice - myProduct.ProductDiscountedPrice)) / myProduct.ProductPrice));
            db.SaveChanges();
            System.Threading.Thread.Sleep(1000);
            return RedirectToAction("ProductDetail","Admin",new { PID=myProduct.ProductID,PName=myProduct.ProductName});
        }
        public ActionResult AddNewProduct()
        {
            if (Session["AdminUserName"] != null && !String.IsNullOrEmpty(Session["AdminUserName"].ToString()))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public ActionResult AddNewProduct(Product myProduct)
        {
            //add new product with product information.
            //assign now datetime to product date.
            myProduct.ProductDate = DateTime.Now;
            //assign 0 firstly.
            myProduct.ProductNumberOfEvaluate = 0;
            myProduct.ProductNumberOfSales = 0;
            myProduct.ProductStar = 0;
            //calculate discounted rate.
            myProduct.ProductDiscountedRate = Convert.ToDecimal(((100 * (myProduct.ProductPrice - myProduct.ProductDiscountedPrice)) / myProduct.ProductPrice));
            db.Product.Add(myProduct);
            db.SaveChanges();
            return RedirectToAction("AddNewProduct");
        }
        public ActionResult ContactMessages()
        {
            //display contact messages
            if (Session["AdminUserName"] != null && !String.IsNullOrEmpty(Session["AdminUserName"].ToString()))
            {
                var myQuery = db.ContactMessage.Where(s => s.MessageID > 0).Select(s => s);
                return View(myQuery);

            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult Search(string searching)
        {
            Product productDetail;
            if (!String.IsNullOrEmpty(searching))
            {
                productDetail = db.Product.Where(s => s.ProductName.Contains(searching)).Select(s => s).FirstOrDefault();
                //if the product is in database.
                if (productDetail != null)
                {
                    System.Threading.Thread.Sleep(1000);
                    return RedirectToAction("ProductDetail", "Admin", new { PID = productDetail.ProductID, Pname = productDetail.ProductName });
                }
            }
            return Redirect("Home");
        }
    }
}