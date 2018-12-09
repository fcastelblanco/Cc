using System;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(Cc.Upt.Web.Startup))]

namespace Cc.Upt.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888

            var theOptions = new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Login/Index"),
                LogoutPath = new PathString("/Login/LogOut"),
                CookieName = "UptAuth",
                ExpireTimeSpan = TimeSpan.FromDays(1),
                CookieHttpOnly = true,
                CookiePath = "/",
                CookieSecure = CookieSecureOption.SameAsRequest
            };

            app.UseCookieAuthentication(theOptions);
        }
    }
}
