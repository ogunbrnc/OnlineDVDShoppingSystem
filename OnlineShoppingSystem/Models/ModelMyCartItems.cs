using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShoppingSystem.Models
{
    public class ModelMyCartItems
    {
        public int CartItemID { get; set; }
        public int CartID { get; set; }
        public int ProductID { get; set; }
        public double ProductPrice { get; set; }
        public double ProductDiscountedPrice { get; set; }
        public int Quantity { get; set; }
        public int ProductStock { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public double TotalItemPrice { get; set; }
        public double TotalDiscountedPrice { get; set; }
        

    }
}