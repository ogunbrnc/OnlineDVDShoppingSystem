using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShoppingSystem.Models
{
    public class ModelProducts
    {
        public List<HighestStarproduct> modelHighestStar{get;set;}
        public List<NewProduct> modelNewProducts{get;set;}
        public List<BestDiscountedProduct> modelDiscounteds{get;set;}
        public List<ProductWithBestSelling> modelProductWithBestSelling{get;set;}
        

    }
}