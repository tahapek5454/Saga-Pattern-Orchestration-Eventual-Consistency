using Microsoft.EntityFrameworkCore;

namespace Stock.API.Context
{
    public class StockDbContext: DbContext
    {
        public StockDbContext(DbContextOptions options):base(options)
        {
            
        }

        public DbSet<Models.Stock> Stocks { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.Stock>().HasData(new List<Models.Stock>()
            {
                new Models.Stock()
                {
                    Id = 1,
                    Count = 10,
                    ProductId = 1,
                },
                new Models.Stock()
                {
                    Id = 2,
                    Count = 10,
                    ProductId = 2,
                },
                new Models.Stock()
                {
                    Id = 3,
                    Count = 10,
                    ProductId = 3,
                },
                new Models.Stock()
                {
                    Id = 4,
                    Count = 10,
                    ProductId = 4,
                },
                new Models.Stock()
                {
                    Id = 5,
                    Count = 10,
                    ProductId = 5,
                },
            });
        }
    }
}
