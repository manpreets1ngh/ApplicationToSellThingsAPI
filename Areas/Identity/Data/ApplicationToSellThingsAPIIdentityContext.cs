using ApplicationToSellThings.APIs.Areas.Identity.Data;
using ApplicationToSellThings.APIs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection.Emit;

namespace ApplicationToSellThings.APIs.Data;

public class ApplicationToSellThingsAPIIdentityContext : IdentityDbContext<ApplicationToSellThingsAPIsUser>
{
    public ApplicationToSellThingsAPIIdentityContext(DbContextOptions<ApplicationToSellThingsAPIIdentityContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
/*
        builder.Entity<AddressModel>()
        .HasOne<ApplicationToSellThingsAPIsUser>(a => a.User)
        .WithMany(u => u.Addresses)
        .HasForeignKey(a => a.UserId);*/
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

    public DbSet<AddressModel> Addresses { get; set; }
    public DbSet<CardModel> CardDetails { get; set; }

}
