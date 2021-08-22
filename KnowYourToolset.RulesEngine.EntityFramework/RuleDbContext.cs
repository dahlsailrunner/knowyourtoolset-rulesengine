using Microsoft.EntityFrameworkCore;

namespace KnowYourToolset.RulesEngine.EntityFramework
{
    public class RuleDbContext : DbContext
    {
        public DbSet<Rule> Rules { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=RulesEngine.db;");
        }
    }
}
