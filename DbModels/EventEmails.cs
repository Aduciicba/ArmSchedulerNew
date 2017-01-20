namespace KodeksArmScheduler
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EventEmails")]
    public partial class EventEmail
    {
        [Key]
        public long id_event_email { get; set; }

        public long? fid_event { get; set; }

        [StringLength(2147483647)]
        public string email { get; set; }
    }
}
