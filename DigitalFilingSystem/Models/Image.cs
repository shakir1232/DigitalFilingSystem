//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DigitalFilingSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Image
    {
        public System.Guid Id { get; set; }
        public System.Guid IndexId { get; set; }
        public string Photo { get; set; }
        public string FileName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
    
        public virtual ImageIndex ImageIndex { get; set; }
    }
}
