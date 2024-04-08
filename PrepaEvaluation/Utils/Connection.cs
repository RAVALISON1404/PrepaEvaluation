using Microsoft.EntityFrameworkCore;
using PrepaEvaluation.Models;

namespace PrepaEvaluation.Utils;

public class Connection : DbContext
{
    public Connection(DbContextOptions<Connection> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.UseSerialColumns();
    }

    public DbSet<Stock> stock { get; set; }
}