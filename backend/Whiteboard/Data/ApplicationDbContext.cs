using Microsoft.EntityFrameworkCore;
using PresentationApp.Models;
using System.Reflection.Emit;

namespace PresentationApp.Data;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

	public DbSet<Presentation> Presentations { get; set; }
	public DbSet<Slide> Slides { get; set; }
	public DbSet<PresentationUser> PresentationUsers { get; set; }
	public DbSet<ApplicationUser> ApplicationUsers { get; set; }
	public DbSet<Line> Lines { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<PresentationUser>()
			.HasKey(pu => new { pu.PresentationId, pu.UserId });

		builder.Entity<PresentationUser>()
			.HasOne(pu => pu.Presentation)
			.WithMany(p => p.PresentationUsers)
			.HasForeignKey(pu => pu.PresentationId);

		builder.Entity<PresentationUser>()
			.HasOne(pu => pu.User)
			.WithMany(u => u.PresentationUsers)
			.HasForeignKey(pu => pu.UserId);

		builder.Entity<ApplicationUser>().HasKey(u => u.Id);
		builder.Entity<ApplicationUser>()
			.HasIndex(u => u.Nickname);
			//.IsUnique();

		builder.Entity<Slide>()
			.HasOne(s => s.Presentation)
			.WithMany(p => p.Slides)
			.HasForeignKey(s => s.PresentationId);
	}
}
