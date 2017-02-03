using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VikasIoTController.Startup))]
namespace VikasIoTController
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
