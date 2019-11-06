using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DigitalFilingSystem.Startup))]
namespace DigitalFilingSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
