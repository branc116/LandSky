using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: OwinStartup(typeof(Game.Server.Startup))]

namespace Game.Server
{
    
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
            app.MapSignalR();
            ConfigureAuth(app);
        }
    }
 
}