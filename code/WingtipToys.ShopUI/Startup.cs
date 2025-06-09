using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WingtipToys.ShopUI.Startup))]
namespace WingtipToys.ShopUI
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
