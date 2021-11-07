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
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class User
    {
        public long ID { get; set; }
        public string Email { get; set; }
        public string PassWord { get; set; }
        public string ConfirmPass { get; set; }
        public string Name { get; set; }
        public string GroupID { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool Status { get; set; }
        public Nullable<int> SexID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string Code { get; set; }
        public Nullable<int> Vacxin { get; set; }
        public Nullable<long> CategoryID { get; set; }
    
        public virtual CategoryUser CategoryUser { get; set; }
        public virtual Sex Sex { get; set; }
        public virtual User_Group User_Group { get; set; }
    }
}
