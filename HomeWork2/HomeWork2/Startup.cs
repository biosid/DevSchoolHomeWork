using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HomeWork2.Startup))]
namespace HomeWork2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
