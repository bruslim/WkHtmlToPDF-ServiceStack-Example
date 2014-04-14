using System;
using System.Linq;
using System.Net;
using NSpec;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.FluentValidation;
using ServiceStack.Model;
using ServiceStack.OrmLite;
using SsWkPdf.ServiceModel;
using SsWkPdf.ServiceModel.Type;

namespace SsWkPdf.Specs.ServiceInterface
{
    internal class describe_WebDocumentsService_DeleteRequest_handler : describe_WebDocumentsService
    {
        private void given_the_fixture_data()
        {
            context["when used with default request object"] = () =>
            {
                it["should throw a ValidationException"] =
                    expect<ValidationException>(() => Service.Any(new WebDocuments.DeleteRequest()));
            };
            context["when used with invalid request object"] = () =>
            {
                it["should throw a ValidationException"] =
                    expect<ValidationException>(
                        () => Service.Any(
                            new WebDocuments.DeleteRequest
                            {
                                Id = Guid.Empty
                            }));
            };
            context["when used with unknown guid"] = () =>
            {
                it["should throw a HttpError"] =
                    expect<HttpError>(
                        () => Service.Any(
                            new WebDocuments.DeleteRequest
                            {
                                Id = Guid.NewGuid()
                            }));
            };
            context["when used with the first record id"] = () =>
            {
                it["should return a HttpResult"] = () =>
                {
                    IHasGuidId originalRecord;
                    using (var db = AppHost.Container.Resolve<IDbConnectionFactory>().Open())
                    {
                        originalRecord = db.Select<WebDocumentMetadata>().First();
                    }
                    var result = Service.Any(
                        new WebDocuments.DeleteRequest
                        {
                            Id = originalRecord.Id
                        });
                    result.should_cast_to<HttpResult>().StatusCode.should_be(HttpStatusCode.NoContent);
                };
            };
        }
    }
}