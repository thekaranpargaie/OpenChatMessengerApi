using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Chat.Infrastructure
{
    public class ChatDbContextFactory : IDesignTimeDbContextFactory<ChatDbContext>
    {
        public ChatDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ChatDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=chatdb;Username=postgres;Password=postgres");

            return new ChatDbContext(optionsBuilder.Options);
        }
    }
}
