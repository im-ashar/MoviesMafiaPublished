using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace MoviesMafia.Models.GenericRepo
{
    public class RecordsDBContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public RecordsDBContext()
        {
            try
            {
                var databaseCreater = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                if (databaseCreater != null)
                {
                    if (!databaseCreater.CanConnect())
                    {
                        databaseCreater.Create();
                    }
                    if (!databaseCreater.HasTables())
                    {
                        databaseCreater.CreateTables();
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public DbSet<Records> Records { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server=containers-us-west-16.railway.app;Port=5625;Database=Records;User Id=postgres;Password=MgxZaCFWJo7FJr3xgcqQ");
        }
        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is Records && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    ((Records)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                    ((Records)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;

                }
                else if (entityEntry.State == EntityState.Modified)
                {
                    ((Records)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChanges();
        }
    }
}
