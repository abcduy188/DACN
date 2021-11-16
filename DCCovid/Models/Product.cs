//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DCCovid.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string SeoTitle { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public Nullable<long> CategoryID { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> PromotionPrice { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string Detail { get; set; }
        public bool Status { get; set; }
        public string CreateBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.DateTime> TopHot { get; set; }
        public int ViewCount { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    
        public virtual Category Category { get; set; }
    }
}
