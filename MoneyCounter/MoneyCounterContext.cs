
using Hangfire;
using Hangfire.Storage.SQLite;
using Microsoft.EntityFrameworkCore;
using MoneyCounter.Models;


namespace MoneyCounter
{
    public class MoneyCounterContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserMessage> UserMessages { get; set; }
        public DbSet<TransactionCategory> TransactionCategories { get; set; }    
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<BalanceMultiplication> BalanceMultiplications { get; set; }
        public DbSet<FoundersTransaction> FoundersTransactions { get; set; }
        public DbSet<TransactionIDForDeletion> TransactionIDsForDeletion { get; set; }
        public DbSet<WhiteUser> WhiteList { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransactionCategory>().HasKey(x => new { x.CategoryKey, x.SubCategoryKey });
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.bM)
                .WithOne(b => b.Transaction)
                .HasForeignKey<BalanceMultiplication>(b => b.TransactionId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Data Source=MoneyCounterDb.db";
            optionsBuilder.UseSqlite(connectionString);
            GlobalConfiguration.Configuration.UseSQLiteStorage(connectionString);
        }
    }
}
