using System.Net;
using NSpec;
using ServiceStack;
using ServiceStack.FluentValidation;
using SsWkPdf.ServiceModel;

namespace SsWkPdf.Specs.ServiceInterface
{
    internal class describe_WebDocumentsService_CreateRequest_Handler : describe_WebDocumentsService
    {
        private void given_the_fixture_data()
        {
            context["when used with default request object"] = () =>
            {
                it["should throw a ValidationException"] =
                    expect<ValidationException>(() => Service.Any(new WebDocuments.CreateRequest()));
            };
            context["when used with invalid request object"] = () =>
            {
                it["should throw a ValidationException"] =
                    expect<ValidationException>(
                        () => Service.Any(
                            new WebDocuments.CreateRequest
                            {
                                SourceUrl = "google.com/?q=" + LoremIpsum + LoremIpsum,
                                FileName = LoremIpsum
                            }));
            };
            context["when used with SourceUrl = google.com"] = () =>
            {
                object result = null;
                beforeAll = () =>
                {
                    result = Service.Any(new WebDocuments.CreateRequest {SourceUrl = "google.com"});
                    //result.PrintDump();
                };
                it["should return a HttpResult"] = () => result.should_cast_to<HttpResult>();
                context["describe the HttpResult"] = () =>
                {
                    HttpResult httpResult = null;
                    beforeAll = () => httpResult = (HttpResult) result;
                    it["should have a status of 201"] = () => httpResult.StatusCode.should_be(HttpStatusCode.Created);
                    it["should have a MetadataResponse"] =
                        () => httpResult.Response.should_cast_to<WebDocuments.MetadataResponse>();
                    context["describe the MetadataResponse"] = () =>
                    {
                        WebDocuments.MetadataResponse response = null;
                        beforeAll = () => response = (WebDocuments.MetadataResponse) httpResult.Response;
                        it["should have a SourceUrl of google.com"] = () => response.SourceUrl.should_be("google.com");
                        it["should have its UsePrintMediaType flag set to true"] =
                            () => response.UsePrintMediaType.should_be(true);
                        it["should have a FileLength greater than 0"] =
                            () => response.FileLength.should_be_greater_than(0);
                        it["should have a FileName of Document.pdf"] = () => response.FileName.should_be("Document.pdf");
                        it["should have a ContentType of application/pdf"] =
                            () => response.ContentType.should_be("application/pdf");
                        it["should have a RecordVersion of 1"] = () => response.RecordVersion.should_be(1);
                        it["should have its IsUpdated flag set to false"] = () => response.IsUpdated().should_be(false);
                        it["should not have its Md5Sum empty"] = () => response.Md5Sum.should_not_be_empty();

                        it["should have MarginBottom be empty "] = () => response.MarginBottom.should_be_empty();
                        it["should have MarginLeft be empty"] = () => response.MarginLeft.should_be_empty();
                        it["should have MarginRight be empty"] = () => response.MarginRight.should_be_empty();
                        it["should have MarginTop be empty"] = () => response.MarginTop.should_be_empty();
                    };
                };
            };
            context["when used with SourceUrl = google.com, and FileName = test, and other options"] = () =>
            {
                object result = null;
                beforeAll = () =>
                {
                    result = Service.Any(
                        new WebDocuments.CreateRequest
                        {
                            SourceUrl = "google.com",
                            FileName = "test",
                            MarginBottom = "0",
                            MarginLeft = "0",
                            MarginRight = "0",
                            MarginTop = "0",
                            UsePrintMediaType = false
                        });
                };
                it["should return a HttpResult"] = () => result.should_cast_to<HttpResult>();
                context["describe the HttpResult"] = () =>
                {
                    HttpResult httpResult = null;
                    beforeAll = () => httpResult = (HttpResult) result;
                    it["should have a status of 201"] = () => httpResult.StatusCode.should_be(HttpStatusCode.Created);
                    it["should have a MetadataResponse"] =
                        () => httpResult.Response.should_cast_to<WebDocuments.MetadataResponse>();
                    context["describe the MetadataResponse"] = () =>
                    {
                        WebDocuments.MetadataResponse response = null;
                        beforeAll = () => response = (WebDocuments.MetadataResponse) httpResult.Response;
                        it["should have a SourceUrl of google.com"] = () => response.SourceUrl.should_be("google.com");
                        it["should have its UsePrintMediaType flag set to false"] =
                            () => response.UsePrintMediaType.should_be(false);
                        it["should have a FileLength greater than 0"] =
                            () => response.FileLength.should_be_greater_than(0);
                        it["should have a FileName of test"] = () => response.FileName.should_be("test");
                        it["should have a ContentType of application/pdf"] =
                            () => response.ContentType.should_be("application/pdf");
                        it["should have a RecordVersion of 1"] = () => response.RecordVersion.should_be(1);
                        it["should have its IsUpdated flag set to false"] = () => response.IsUpdated().should_be(false);
                        it["should not have its Md5Sum empty"] = () => response.Md5Sum.should_not_be_empty();
                        it["should have a MarginBottom = 0"] = () => response.MarginBottom.should_be("0");
                        it["should have a MarginLeft = 0"] = () => response.MarginLeft.should_be("0");
                        it["should have a MarginRight = 0"] = () => response.MarginRight.should_be("0");
                        it["should have a MarginTop = 0"] = () => response.MarginTop.should_be("0");
                    };
                };
            };
        }
    }
}