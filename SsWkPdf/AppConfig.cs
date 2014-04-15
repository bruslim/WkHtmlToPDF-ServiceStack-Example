using System.Collections.Generic;
using Funq;
using ServiceStack;
using ServiceStack.MsgPack;
using ServiceStack.Validation;
using SsWkPdf.FluentValidation;
using SsWkPdf.ServiceInterface.FluentValidation;
using WkHtmlToXSharp;

namespace SsWkPdf
{
    public static class AppConfig
    {
        public static void Config(Container container)
        {
            // register pdf converter with IoC
            container.Register<IHtmlToPdfConverter>(new MultiplexingConverter());


            // register validators
            container.Register<IUriValidator>(new UriValidator());
            container.RegisterValidators(typeof (WebDocumentsValidators).Assembly);
        }

        public static void EnablePlugins(IList<IPlugin> plugins)
        {
            // register plugins
            plugins.Add(new RequestLogsFeature(5000));

            // enable Msessage Pack Format
            plugins.Add(new MsgPackFormat());

            // enable validation
            plugins.Add(new ValidationFeature());
        }
    }
}