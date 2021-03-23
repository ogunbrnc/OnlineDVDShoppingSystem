using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShoppingSystem.Models
{
    public class ModelMyCart
    {
        public double TotalCartPrice { get; set; }
        public double TotalDiscountedCartPrice { get; set; }
        public int CartID { get; set; }
        public List<ModelMyCartItems> listMyCarts { get; set; }
        public bool Empty
        {
            get
            {
                return (listMyCarts.Count == 0);

            }
        }
    }
}