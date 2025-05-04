namespace SandboxCmd;

internal class DemoService(ApplicationDbContext dbContext)
{
	private readonly ApplicationDbContext dbContext = dbContext;
}
