namespace KodeksArmScheduler
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EventLog")]
    public partial class EventLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id_event_log { get; set; }

        [StringLength(2147483647)]
        public string event_time { get; set; }

        public long? fid_event { get; set; }

        [StringLength(2147483647)]
        public string event_state { get; set; }

        [StringLength(2147483647)]
        public string event_errors { get; set; }
    }
}
