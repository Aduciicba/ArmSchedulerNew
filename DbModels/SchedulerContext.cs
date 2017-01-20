namespace KodeksArmScheduler
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.SQLite;

    public partial class SchedulerContext : DbContext
    {
        public SchedulerContext(string connectionString)
            : base(new SQLiteConnection() { ConnectionString = connectionString }, true)
        {
        }

        public virtual DbSet<DbVersion> DbVersions { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<EventEmail> EventEmails { get; set; }
        public virtual DbSet<EventLog> EventLogs { get; set; }
        public virtual DbSet<EventType> EventTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
