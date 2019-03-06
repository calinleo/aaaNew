using aaaNew.Models;
using Microsoft.EntityFrameworkCore;

namespace aaaNew.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {}
        public DbSet<Value> Values { get; set; }
    }
}