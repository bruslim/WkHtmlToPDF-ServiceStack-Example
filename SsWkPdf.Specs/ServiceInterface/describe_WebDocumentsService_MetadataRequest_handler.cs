using System;
using System.Linq;
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
    internal class describe_WebDocumentsService_MetadataRequest_handler : describe_WebDocumentsService
    {
        private void given_the_fixture_data()
        {
            context["when used with default request object"] = () =>
            {
                it["should throw a ValidationException"] =
                    expect<ValidationException>(() => Service.Get(new WebDocuments.MetadataRequest()));
            };
            context["when used with invalid request object"] = () =>
            {
                it["should throw a ValidationException"] =
                    expect<ValidationException>(
                        () => Service.Get(
                            new WebDocuments.MetadataRequest
                            {
                                Id = Guid.Empty
                            }));
            };
            context["when used with unknown guid"] = () =>
            {
                it["should throw a HttpError"] =
                    expect<HttpError>(
                        () => Service.Get(
                            new WebDocuments.MetadataRequest
                            {
                                Id = Guid.NewGuid()
                            }));
            };
            context["when used with the first record id"] = () =>
            {
                IHasGuidId originalRecord = null;

                beforeEach = () =>
                {
                    using (var db = AppHost.Container.Resolve<IDbConnectionFactory>().Open())
                    {
                        originalRecord = db.Select<WebDocumentMetadata>().First();
                    }
                };
                it["should return a MetadataResponse with the same id"] = () => Service.Any(
                    new WebDocuments.MetadataRequest
                    {
                        Id = originalRecord.Id
                    }).Id.should_be(originalRecord.Id);
            };
        }
    }
}