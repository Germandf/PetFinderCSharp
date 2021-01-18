using Microsoft.AspNetCore.Hosting;
using PetFinder.Areas.Identity;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]

namespace PetFinder.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => { });
        }
    }
}