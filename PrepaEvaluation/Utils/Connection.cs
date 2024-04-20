using Microsoft.EntityFrameworkCore;
using PrepaEvaluation.Models;
using PrepaEvaluation.Models.Temporary;
using PrepaEvaluation.Models.View;

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
    public DbSet<Sceance> sceance { get; set; }
    public DbSet<Film> film { get; set; }
    public DbSet<Categorie> categorie { get; set; }
    public DbSet<Salle> salle { get; set; }
    public DbSet<SceanceTemp> sceancetemp { get; set; }
    public DbSet<SceanceLib> sceancelib { get; set; }
}