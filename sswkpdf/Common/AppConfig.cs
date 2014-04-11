using System.Collections;
using System.Collections.Generic;
using Funq;
using ServiceStack;
using ServiceStack.Validation;
using SsWkPdf.FluentValidation;
using WkHtmlToXSharp;

namespace SsWkPdf.Common
{
    public static class AppConfig
    {
        public static void Config(Container container)
        {
            // register pdf converter with IoC
            container.Register<IHtmlToPdfConverter>(new MultiplexingConverter());
            
            // register validators
            container.Register<IUriValidator>(new UriValidator());
            container.RegisterValidators(typeof(WebDocumentsValidators).Assembly);
        }

        public static void EnablePlugins(IList<IPlugin> plugins)
        {
            // register plugins
            plugins.Add(new RequestLogsFeature(5000));

            // enable validation
            plugins.Add(new ValidationFeature());
        }
    }
}