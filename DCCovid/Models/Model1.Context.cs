﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DCCovidDbcontext : DbContext
    {
        public DCCovidDbcontext()
            : base("name=DCCovidDbcontext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CategoryUser> CategoryUsers { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<Sex> Sexes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<User_Group> User_Group { get; set; }

        public System.Data.Entity.DbSet<DCCovid.ViewModel.RegisterModel> RegisterModels { get; set; }
    }
}
