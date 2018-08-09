using IdentitySample;
using Logistic.Base.DataContext;
using LogisticSystem.Controllers;
using LogisticSystem.Scheduling;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace LogisticSystem
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            JobScheduler.Start();
            //Database.SetInitializer<LogisticsConnection>(null);
        }
        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            //Check if user is authenticated
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (!authTicket.Expired)
                {
                    if (Session["UserName"] != null)
                    {
                        //Session is null, redirect to login page
                        Session.Abandon();
                        Session.Clear();
                        FormsAuthentication.SignOut();
                        Response.Redirect(FormsAuthentication.LoginUrl, true);
                        return;
                    }
                }
            }
        }
    }
}