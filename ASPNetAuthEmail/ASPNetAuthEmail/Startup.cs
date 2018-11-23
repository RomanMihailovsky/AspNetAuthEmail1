using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;

using ASPNetAuthEmail.Hubs;


[assembly: OwinStartup(typeof(ASPNetAuthEmail.Startup))]

namespace ASPNetAuthEmail
{

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Дополнительные сведения о настройке приложения см. по адресу: http://go.microsoft.com/fwlink/?LinkID=316888

            ConfigureAuth(app);

            //GlobalHost.DependencyResolver = new AutofacDependencyResolver(container);
            //IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<nHub>();

            //app.UseSignalR(routes =>
            //{
            //    routes.MapHub<nHub>("nHub");
            //});

            //GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new ChatHub(new ChatMessageRepository()));

            app.MapSignalR();
            
        }
    }
}
