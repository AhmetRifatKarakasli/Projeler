//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace My_Project.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Input
    {
        public int input_id { get; set; }
        public int product_id { get; set; }
        public int product_quantity { get; set; }
        public System.DateTime input_date { get; set; }
        public int per_id { get; set; }
    
        public virtual Personnel Personnel { get; set; }
        public virtual Product Product { get; set; }
    }
}
