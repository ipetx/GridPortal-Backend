using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class PsseDbContextFactory : IDesignTimeDbContextFactory<PsseDbContext>
{
    public PsseDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PsseDbContext>();
       // optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CimDb;Trusted_Connection=True;");
                optionsBuilder.UseNpgsql("Host=localhost;Database=powerflow;Username=postgres;Password=2025");

        return new PsseDbContext(optionsBuilder.Options);
    }
}