using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SandboxCmd;

internal class DemoService(
	ILogger<DemoService> logger,
	IDbContextFactory<ApplicationDbContext> dbFactory)
{
	private readonly ILogger<DemoService> _logger = logger;
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;
}
