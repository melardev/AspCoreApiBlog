using Microsoft.Extensions.Configuration;

namespace BlogDotNet.Services
{
    public class LocalJsonBasedSettingsService
    {
        public LocalJsonBasedSettingsService(IConfiguration configuration)
        {
            configuration.GetSection("Blog");
        }
    }
}