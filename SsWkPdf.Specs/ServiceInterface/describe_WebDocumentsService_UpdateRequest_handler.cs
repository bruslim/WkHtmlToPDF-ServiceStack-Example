using System;
using System.Linq;
using System.Net;
using NSpec;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.FluentValidation;
using ServiceStack.OrmLite;
using SsWkPdf.ServiceModel;
using SsWkPdf.ServiceModel.Type;

namespace SsWkPdf.Specs.ServiceInterface
{
    internal class describe_WebDocumentsService_UpdateRequest_handler : describe_WebDocumentsService
    {
        private void given_the_fixture_data()
        {
            context["when used with default request object"] = () =>
            {
                it["should throw a ValidationException"] =
                    expect<ValidationException>(() => Service.Any(new WebDocuments.UpdateRequest()));
            };
            context["when used with invalid request object"] = () =>
            {
                it["should throw a ValidationException"] =
                    expect<ValidationException>(
                        () => Service.Any(
                            new WebDocuments.UpdateRequest
                            {
                                Id = Guid.NewGuid(),
                                SourceUrl = "google.com/?q=" + LoremIpsum + LoremIpsum,
                                FileName = LoremIpsum
                            }));
            };
            context["when used with unknown guid"] = () =>
            {
                it["should throw a HttpError"] =
                    expect<HttpError>(
                        () => Service.Any(
                            new WebDocuments.UpdateRequest
                            {
                                Id = Guid.NewGuid(),
                                RecordVersion = 1
                            }));
            };
            context["when updating the first record"] = () =>
            {
                WebDocument originalRecord = null, updatedRecord = null;
                object result = null;
                before = () =>
                {
                    using (var db = AppHost.Container.Resolve<IDbConnectionFactory>().Open())
                    {
                        originalRecord = db.Select<WebDocument>().First();
                    }
                    result = Service.Any(
                        new WebDocuments.UpdateRequest
                        {
                            Id = originalRecord.Id,
                            RecordVersion = originalRecord.RecordVersion
                        });
                    using (var db = AppHost.Container.Resolve<IDbConnectionFactory>().Open())
                    {
                        updatedRecord = db.Select<WebDocument>(r => r.Id == originalRecord.Id).First();
                    }
                };
                it["should return a HttpResult with a status of 200 (OK)"] =
                    () => result.should_cast_to<HttpResult>().StatusCode.should_be(HttpStatusCode.OK);
                it["should not have an empty file field"] = () => updatedRecord.File.should_not_be_empty();
                it["should update the file field"] = () => updatedRecord.File.should_not_be(originalRecord.File);
                it["should not have an empty md5sum field"] = () => updatedRecord.Md5Sum.should_not_be_empty();
                it["should update the md5sum field"] = () => updatedRecord.Md5Sum.should_not_be(originalRecord.Md5Sum);
                it["should update the RecordVersion"] = () => updatedRecord.RecordVersion.should_be(2);
            };
            context["when updating the first record with options"] = () =>
            {
                WebDocument originalRecord = null, updatedRecord = null;
                object result = null;
                before = () =>
                {
                    using (var db = AppHost.Container.Resolve<IDbConnectionFactory>().Open())
                    {
                        originalRecord = db.Select<WebDocument>().First();
                    }
                    result = Service.Any(
                        new WebDocuments.UpdateRequest
                        {
                            Id = originalRecord.Id,
                            RecordVersion = originalRecord.RecordVersion,
                            FileName = "test option",
                            MarginBottom = "1px",
                            MarginLeft = "1px",
                            MarginRight = "1px",
                            MarginTop = "1px",
                            SourceUrl = "bing.com",
                            UsePrintMediaType = false
                        });
                    using (var db = AppHost.Container.Resolve<IDbConnectionFactory>().Open())
                    {
                        updatedRecord = db.Select<WebDocument>(r => r.Id == originalRecord.Id).First();
                    }
                };
                it["should return a HttpResult with a status of 200 (OK)"] =
                    () => result.should_cast_to<HttpResult>().StatusCode.should_be(HttpStatusCode.OK);
                it["should not have an empty file field"] = () => updatedRecord.File.should_not_be_empty();
                it["should update the file field"] = () => updatedRecord.File.should_not_be(originalRecord.File);
                it["should not have an empty md5sum field"] = () => updatedRecord.Md5Sum.should_not_be_empty();
                it["should update the md5sum field"] = () => updatedRecord.Md5Sum.should_not_be(originalRecord.Md5Sum);
                it["should update the RecordVersion"] = () => updatedRecord.RecordVersion.should_be(2);

                it["should update the FileName to 'test option'"] =
                    () => updatedRecord.FileName.should_be("test option");

                it["should update the MarginBottom to '1px'"] =
                    () => updatedRecord.MarginBottom.should_be("1px");

                it["should update the MarginLeft to '1px'"] =
                    () => updatedRecord.MarginLeft.should_be("1px");

                it["should update the MarginRight to '1px'"] =
                    () => updatedRecord.MarginRight.should_be("1px");

                it["should update the MarginTop to '1px'"] =
                    () => updatedRecord.MarginTop.should_be("1px");

                it["should update the SourceUrl to 'bing.com'"] =
                    () => updatedRecord.SourceUrl.should_be("bing.com");

                it["should update the UsePrintMediaType to 'false'"] =
                    () => updatedRecord.UsePrintMediaType.should_be(false);
            };
        }
    }
}