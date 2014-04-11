using System;
using System.Linq;
using NSpec;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.FluentValidation;
using ServiceStack.Model;
using ServiceStack.OrmLite;
using SsWkPdf.Common;
using SsWkPdf.ServiceModel;
using SsWkPdf.ServiceModel.Type;

namespace SsWkPdf.Specs.ServiceInterface
{
    internal class describe_WebDocumentsService_FindByIdRequest_handler : describe_WebDocumentsService
    {
        private void given_the_fixture_data()
        {
            context["when used with default request object"] = () =>
            {
                it["should throw a ValidationException"] =
                    expect<ValidationException>(() => Service.Get(new WebDocuments.FindByIdRequest()));
            };
            context["when used with invalid request object"] = () =>
            {
                it["should throw a ValidationException"] =
                    expect<ValidationException>(
                        () => Service.Get(
                            new WebDocuments.FindByIdRequest
                            {
                                Id = Guid.Empty
                            }));
            };
            context["when used with unknown guid"] = () =>
            {
                it["should throw a HttpError"] =
                    expect<HttpError>(
                        () => Service.Get(
                            new WebDocuments.FindByIdRequest
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
                it["should return a FileResult"] = () => Service.Get(
                    new WebDocuments.FindByIdRequest
                    {
                        Id = originalRecord.Id
                    }).should_cast_to<FileResult>();
            };
        }
    }
}