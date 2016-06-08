using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BibliotekaPubliczna.Startup))]
namespace BibliotekaPubliczna
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
