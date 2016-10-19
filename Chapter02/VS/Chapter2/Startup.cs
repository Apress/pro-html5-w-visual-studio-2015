using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Chapter2.Startup))]
namespace Chapter2
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
