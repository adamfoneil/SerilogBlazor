﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SerilogBlazor.Abstractions.IndexedLogContext;
using SerilogBlazor.Abstractions.SavedSearches;

namespace SampleApp.Data;

public partial class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options),
	IIndexedLogContext, ISerilogSavedSearches
{
	public DbSet<ExceptionTemplate> ExceptionTemplates { get; set; }
	public DbSet<ExceptionInstance> ExceptionInstances { get; set; }
	public DbSet<SerilogTableMarker> SerilogTableMarkers { get; set; }
	public DbSet<SerilogSavedSearch> SerilogSavedSearches { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(IIndexedLogContext).Assembly);
	}
}

public class AppDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
	public const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=SerilogViewerDemo;Integrated Security=true;TrustServerCertificate=True;";

	public ApplicationDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
		optionsBuilder.UseSqlServer(ConnectionString);
		return new ApplicationDbContext(optionsBuilder.Options);
	}
}