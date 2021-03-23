using OnlineShoppingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShoppingSystem.Controllers
{
    public class ProductController : Controller
    {
        OnlineShoppingSystemDBForWEBEntities db = new OnlineShoppingSystemDBForWEBEntities();
        public ActionResult ProductDetails(int PID,string PName)
        {
            //list the product with PID.
            Product myProduct = db.Product.Where(x => x.ProductID == PID).Select(x => x).FirstOrDefault();
            List<ProductComment> productComments = new List<ProductComment>();
            productComments = db.ProductComment.Where(x => x.ProductID == PID).OrderByDescending(x=>x.ProductCommentID).Select(x => x).Take(5).ToList();

            ProductDetail myProductDetail = new ProductDetail();
            myProductDetail.product = myProduct;
            myProductDetail.productComments = productComments;
            return View(myProductDetail);
        }

        [HttpPost]
        public ActionResult ProductDetails(int PID)
        {
            //if the users are not login, so they can not add product to their cart.
            if (Session["UserName"]!=null &&!String.IsNullOrEmpty(Session["UserName"].ToString()))
            {
                String UserName = Session["UserName"].ToString();
                Product Product = new Product();
                int UserID = db.Customer.Where(x => x.UserName == UserName).Select(x => x.CustomerID).FirstOrDefault();
                Product = db.Product.Where(x => x.ProductID == PID).FirstOrDefault();
                if (Product.ProductStock != 0)//if product stock is zero, users cannot add product to their cart.
                {
                    Cart Cart = new Cart();
                    CartItem CartItem = new CartItem();

                    Cart = db.Cart.Where(x => x.CustomerID == UserID).FirstOrDefault();
                    var ControlItem = db.CartItem.Where(x => x.ProductID == PID && x.CartID == Cart.CartID).FirstOrDefault();

                    if (ControlItem != null)//item is already in the cart.
                    {
                        ControlItem.Quantity += 1;//increase quantity,price and discounted price.
                        ControlItem.Price += Product.ProductPrice;
                        ControlItem.DiscountedPrice += Product.ProductDiscountedPrice;
                    }
                    else//item is not in the cart.
                    {//create cart item.
                        CartItem.ProductID = Product.ProductID;
                        CartItem.Price = Product.ProductPrice;
                        CartItem.DiscountedPrice = Product.ProductDiscountedPrice;
                        CartItem.CartID = Cart.CartID;
                        CartItem.Quantity = 1;
                        db.CartItem.Add(CartItem);
                    }
                    Product.ProductStock -= 1;//stock decrease
                    Product.ProductNumberOfSales += 1;//number of sales increase.
                    Cart.TotalCost += Product.ProductPrice;
                    Cart.TotalDiscountedCost += Product.ProductDiscountedPrice;
                    db.SaveChanges();
                    System.Threading.Thread.Sleep(1000);
                    return RedirectToAction("MyCart", "My");
                }
                else
                {
                    List<ProductComment> productComments = new List<ProductComment>();
                    ProductDetail myProductDetail = new ProductDetail();
                    productComments = db.ProductComment.Where(x => x.ProductID == PID).OrderByDescending(x => x.ProductCommentID).Select(x => x).Take(5).ToList();
                    myProductDetail.product = Product;
                    myProductDetail.productComments = productComments;
                    ViewBag.Message = "Sorry! There is no enough stock,please again later.";//no stock.
                    return View(myProductDetail);
                }
            }
            else
            {
                return RedirectToAction("Login", "My");//direct to login page.
            }
        }

        public ActionResult ListProductWithSubCategory(int CatID,int SubID)
        {
            //take the cat id and sub id, list the products.
            List<Product> myQuery = new List<Product>();
            SubCategoryProducts myModel = new SubCategoryProducts();
            myModel.ProductCategoryID = CatID;
            myModel.ProductSubCategoryID = SubID;
            myQuery = db.Product.Where(s => s.CategoryID == CatID && s.SubCategoryID ==SubID).Select(s => s).ToList();
            myModel.myProductList = myQuery;
            return View(myModel);
        }

        [HttpPost]
        public ActionResult ListProductWithSubCategory(int CatID, int SubID,string sortBy)
        {
            List<Product> myQuery = new List<Product>();
            SubCategoryProducts myModel = new SubCategoryProducts();
            switch (sortBy)
            {
                case "ascPrice":
                    myQuery = db.Product.Where(s => s.CategoryID == CatID && s.SubCategoryID == SubID).OrderBy(x=>x.ProductDiscountedPrice).Select(s => s).ToList();
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

        public ActionResult ProductEvaluation(int PID)
        {
        
            Product myProduct = db.Product.Where(x => x.ProductID == PID).Select(x=>x).FirstOrDefault();
            ProductComment myComment = new ProductComment();
            ModelProductComments myModel= new ModelProductComments();
            myModel.product = myProduct;
            myModel.productComment = myComment;
            return View(myModel);
        }
        [HttpPost]
        public ActionResult ProductEvaluation(ModelProductComments modelProductComments)
        {
            String UserName = Session["UserName"].ToString();
            ProductComment myComment = new ProductComment();
            int PID = modelProductComments.product.ProductID;
            int CID= db.Customer.Where(x => x.UserName == UserName).Select(x => x.CustomerID).FirstOrDefault();
            Product myProduct = db.Product.Where(x => x.ProductID == PID).Select(x => x).FirstOrDefault();

            //comment is already in the database, so update the database with new comment.
            if (db.ProductComment.Where(x => x.CustomerID == CID && x.ProductID == PID).Select(x => x.CustomerID).FirstOrDefault()==CID)
            {
                myComment = db.ProductComment.Where(x => x.CustomerID == CID && x.ProductID == PID).Select(x => x).FirstOrDefault();               
                myProduct.ProductStar = ((myProduct.ProductStar * myProduct.ProductNumberOfEvaluate-myComment.ProductStar) + modelProductComments.productComment.ProductStar) / (myProduct.ProductNumberOfEvaluate);
                myComment.Comment = modelProductComments.productComment.Comment;
                myComment.ProductStar = modelProductComments.productComment.ProductStar;
                db.SaveChanges();
            }
            else{
                //add the new comment to the database.
                myComment.ProductStar = modelProductComments.productComment.ProductStar;
                myComment.ProductID = PID;
                myComment.CustomerID = CID;
                myComment.CustomerName = db.Customer.Where(x => x.CustomerID == CID).Select(x => x.UserName).FirstOrDefault();
                myComment.Comment = modelProductComments.productComment.Comment;
                db.ProductComment.Add(myComment);
                myProduct.ProductStar = ((myProduct.ProductStar * myProduct.ProductNumberOfEvaluate) + modelProductComments.productComment.ProductStar) / (myProduct.ProductNumberOfEvaluate+1);
                myProduct.ProductNumberOfEvaluate += 1;
                db.SaveChanges();
            }
            db.SaveChanges();
            return RedirectToAction("Home", "My");
        }
    }
}