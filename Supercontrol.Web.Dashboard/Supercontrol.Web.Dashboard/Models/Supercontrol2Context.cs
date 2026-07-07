using System.Data.Entity;
using MySql.Data.EntityFramework;

namespace Supercontrol.Web.Dashboard.Models
{
    /// <summary>
    /// EF6 Code-First context for the "supercontrol2" MySQL database, using the
    /// read-only "Supercontrol2Reader" connection string defined in Web.config.
    /// </summary>
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class Supercontrol2Context : DbContext
    {
        public Supercontrol2Context() : base("name=Supercontrol2Reader")
        {
        }

        // Add DbSet<TEntity> properties here for each table you want to query, e.g.:
        // public DbSet<Device> Devices { get; set; }
    }
}
