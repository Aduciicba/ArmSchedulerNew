//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KodeksArmScheduler
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    
    public partial class EventEmail
    {
        [Key]
        public long id_event_email { get; set; }
        public Nullable<long> fid_event { get; set; }
        public string email { get; set; }
    }
}