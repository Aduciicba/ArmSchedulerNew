namespace KodeksArmScheduler
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EventType")]
    public partial class EventType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long id_event_type { get; set; }

        public long? fid_event_type { get; set; }

        [StringLength(2147483647)]
        public string name { get; set; }
    }
}
