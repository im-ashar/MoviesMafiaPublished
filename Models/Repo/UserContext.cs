using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace MoviesMafia.Models.Repo
{
    public class UserContext : IdentityDbContext<ExtendedIdentityUser>
    {
        

        public UserContext(DbContextOptions<UserContext> options) : base(options)
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
