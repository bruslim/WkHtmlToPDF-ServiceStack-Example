using System;
using System.Web;
using Funq;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Data;
using ServiceStack.MiniProfiler;
using ServiceStack.MiniProfiler.Data;
using ServiceStack.OrmLite;
using SsWkPdf.ServiceInterface;
using SsWkPdf.ServiceModel.Type;

namespace SsWkPdf
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            new AppHost().Init();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Request.IsLocal)
            {
                Profiler.Start();
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            Profiler.Stop();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }

        public class AppHost : AppHostBase
        {
            //Tell Service Stack the name of your application and where to find your web services
            public AppHost()
                : base("WkToPdf Web Services", typeof (WebDocumentsService).Assembly)
            {
            }

            public override void Configure(Container container)
            {
                AppConfig.EnablePlugins(Plugins);

                //register any dependencies your services use, e.g:
                AppConfig.Config(container);

                // register db factory with IoC
                container.Register<IDbConnectionFactory>(
                    new OrmLiteConnectionFactory(ConfigUtils.GetConnectionString("AppDb"), SqlServerDialect.Provider)
                    {
                        ConnectionFilter = filter => new ProfiledDbConnection(filter, Profiler.Current)
                    });


                // create table if not exists
                using (var db = container.Resolve<IDbConnectionFactory>().Open())
                {
                    db.CreateTableIfNotExists<WebDocument>();
                }
            }
        }
    }
}