//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OnlineShoppingSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class NewProduct
    {
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public int SubCategoryID { get; set; }
        public int ProductStock { get; set; }
        public string ProductName { get; set; }
        public double ProductPrice { get; set; }
        public double ProductDiscountedPrice { get; set; }
        public Nullable<decimal> ProductStar { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImagePath { get; set; }
        public System.DateTime ProductDate { get; set; }
        public Nullable<int> ProductNumberOfSales { get; set; }
        public Nullable<decimal> ProductDiscountedRate { get; set; }
        public Nullable<int> ProductNumberOfEvaluate { get; set; }
    }
}
