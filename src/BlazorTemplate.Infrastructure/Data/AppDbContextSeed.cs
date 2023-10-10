using BlazorTemplate.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace BlazorTemplate.Infrastructure
{
    public class AppDbContextSeed
    {
        public static Task Seed(
            AppDbContext appDbContext,
            ILogger logger)
        {
            return Task.CompletedTask;
        }
    }
}
