using NSpec;
using ServiceStack;
using ServiceStack.MsgPack;
using SsWkPdf.ServiceModel;

namespace SsWkPdf.Specs.Common
{
    public class describe_WebDocumentsClient :nspec
    {
        void describe_Create()
        {
            it["should just work"] = () =>
            {
                IServiceClient client = new JsonServiceClient("http://localhost:14872");
                var result = client.Post(
                    new WebDocuments.CreateRequest
                    {
                        SourceUrl = "google.com"
                    });
                result.should_not_be_null();
            };
        }
    }
}