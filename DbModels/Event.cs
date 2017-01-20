namespace KodeksArmScheduler
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Event")]
    public partial class Event
    {
        [Key]
        public long id_event { get; set; }

        public long fid_event_type { get; set; }

        [StringLength(2147483647)]
        public string start_week_days { get; set; }

        [StringLength(2147483647)]
        public string start_time { get; set; }
    }
}
