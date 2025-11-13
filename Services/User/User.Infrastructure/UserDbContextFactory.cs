using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Base.Logging;
using Base.Session;
using Serilog;

namespace User.Infrastructure
{
    public class UserDbContextFactory : IDesignTimeDbContextFactory<UserDb>
    {
        public UserDb CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserDb>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=userdb;Username=postgres;Password=postgres");

            // Create minimal dependencies for design time
            var sessionManager = new SessionManager(null!);
            var serilogLogger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            var debugLogger = new DebugLogger(serilogLogger);

            return new UserDb(optionsBuilder.Options, sessionManager, debugLogger, initializeDatabase: false);
        }
    }
}
