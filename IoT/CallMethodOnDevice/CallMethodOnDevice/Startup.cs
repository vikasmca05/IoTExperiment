using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CallMethodOnDevice.Startup))]
namespace CallMethodOnDevice
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
