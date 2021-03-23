using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShoppingSystem.Models
{
    public class ModelAdmin
    {
        public List<ProductsWithLowStock> listlowerProducts { get; set; }
        public List<ProductWithBestSelling> listBestSellingProduct { get; set; }
        public List<ProductWithLeastSelling> listLeastSellingProduct { get; set; }

    }
}