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
    
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategoryUserPost> CategoryUserPosts { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<PostCMT> PostCMTs { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Sex> Sexes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<User_Group> User_Group { get; set; }
    }
}
