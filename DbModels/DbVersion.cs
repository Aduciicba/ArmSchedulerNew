namespace KodeksArmScheduler
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DbVersion")]
    public partial class DbVersion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long id { get; set; }

        [StringLength(2147483647)]
        public string version { get; set; }

        [StringLength(2147483647)]
        public string build { get; set; }

        [StringLength(2147483647)]
        public string last_update_time { get; set; }

        
    }
}
