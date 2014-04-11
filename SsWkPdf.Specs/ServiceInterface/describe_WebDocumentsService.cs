using System;
using System.Linq;
using System.Net;
using System.Text;
using NSpec;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.FluentValidation;
using ServiceStack.Model;
using ServiceStack.OrmLite;
using ServiceStack.Testing;
using SsWkPdf.Common;
using SsWkPdf.Common.Util;
using SsWkPdf.ServiceInterface;
using SsWkPdf.ServiceModel;
using SsWkPdf.ServiceModel.Type;

namespace SsWkPdf.Specs.ServiceInterface
{
    internal class describe_WebDocumentsService : nspec
    {
        private const string loremIpsum =
            @"Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur? At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita distinctio. Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae. Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat.";

        private ServiceStackHost AppHost { get; set; }
        private WebDocumentsService Service { get; set; }

        /// <summary>
        ///     Before all specs, setup
        /// </summary>
        private void before_all()
        {
            AppHost = new BasicAppHost().Init();

            AppConfig.Config(AppHost.Container);

            AppHost.Container.RegisterAutoWired<WebDocumentsService>();

            Service = AppHost.Container.Resolve<WebDocumentsService>();

            AppHost.Container.Register<IDbConnectionFactory>(
                new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider));
        }

        /// <summary>
        ///     Before each spec, drop and create the WebDocument table
        /// </summary>
        private void before_each()
        {
            using (var db = AppHost.Container.Resolve<IDbConnectionFactory>().Open())
            {
                db.DropAndCreateTable<WebDocument>();
                db.InsertAll(
                    new[]
                    {
                        new WebDocument
                        {
                            ContentType = @"text/plain; charset=""UTF-8""",
                            File = Encoding.UTF8.GetBytes("test document 1"),
                            FileLength = Encoding.UTF8.GetBytes("test document 1").LongLength,
                            FileName = "Test Document 1.txt",
                            Md5Sum = Encoding.UTF8.GetBytes("test document 1").ToMd5Sum(),
                            SourceUrl = "http://google.com",
                            UsePrintMediaType = true
                        },
                        new WebDocument
                        {
                            ContentType = @"text/plain; charset=""UTF-8""",
                            File = Encoding.UTF8.GetBytes("test document 2"),
                            FileLength = Encoding.UTF8.GetBytes("test document 2").LongLength,
                            FileName = "Test Document 2.txt",
                            Md5Sum = Encoding.UTF8.GetBytes("test document 2").ToMd5Sum(),
                            SourceUrl = "http://localhost/test2"
                        },
                        new WebDocument
                        {
                            ContentType = @"text/plain; charset=""UTF-8""",
                            File = Encoding.UTF8.GetBytes("test document 3"),
                            FileLength = Encoding.UTF8.GetBytes("test document 3").LongLength,
                            FileName = "Test Document 3.txt",
                            Md5Sum = Encoding.UTF8.GetBytes("test document 3").ToMd5Sum(),
                            SourceUrl = "http://localhost/test3"
                        }
                    });
            }
        }


        private void describe_in_memory_test_database()
        {
            it["should have the WebDocument table"] = () =>
            {
                using (var db = AppHost.Container.Resolve<IDbConnectionFactory>().Open())
                {
                    db.TableExists("WebDocument").should_be_true();
                }
            };
            it["should have 3 records"] = () =>
            {
                using (var db = AppHost.Container.Resolve<IDbConnectionFactory>().Open())
                {
                    db.Count<WebDocument>().should_be(3);
                }
            };
        }

        private void describe_find_request_handler()
        {
            context["when used with default options"] = () =>
            {
                IPagedResponse<WebDocuments.MetadataResponse> result = null;
                beforeAll = () => result = Service.Any(new WebDocuments.FindRequest());
                it["should return all 3 records"] = () => result.Data.Count().should_be(3);
                it["should have a total of 3 records"] = () => result.Meta.Total.should_be(3);
                it["should have a current page of 1"] = () => result.Meta.CurrentPage.should_be(1);
                it["should have a page size of 100"] = () => result.Meta.PageSize.should_be(100);
            };

            context["when used with int.min for page"] = () =>
            {
                IPagedResponse<WebDocuments.MetadataResponse> result = null;
                beforeAll = () => result = Service.Any(new WebDocuments.FindRequest {Page = int.MinValue});
                it["should return all 3 records"] = () => result.Data.Count().should_be(3);
                it["should have a total of 3 records"] = () => result.Meta.Total.should_be(3);
                it["should have a current page of 1"] = () => result.Meta.CurrentPage.should_be(1);
                it["should have a page size of 100"] = () => result.Meta.PageSize.should_be(100);
            };

            context["when used with int.max for page"] = () =>
            {
                IPagedResponse<WebDocuments.MetadataResponse> result = null;
                beforeAll = () => result = Service.Any(new WebDocuments.FindRequest {Page = int.MaxValue});
                it["should return 0 records"] = () => result.Data.Count().should_be(0);
                it["should have a total of 3 records"] = () => result.Meta.Total.should_be(3);
                it["should have a current page of int.max"] = () => result.Meta.CurrentPage.should_be(int.MaxValue);
                it["should have a page size of 100"] = () => result.Meta.PageSize.should_be(100);
            };

            context["when used with int.min for page size"] = () =>
            {
                IPagedResponse<WebDocuments.MetadataResponse> result = null;
                beforeAll = () => result = Service.Any(new WebDocuments.FindRequest {PageSize = int.MinValue});
                it["should return 3 records"] = () => result.Data.Count().should_be(3);
                it["should have a total of 3 records"] = () => result.Meta.Total.should_be(3);
                it["should have a current page of 1"] = () => result.Meta.CurrentPage.should_be(1);
                it["should have a page size of 100"] = () => result.Meta.PageSize.should_be(100);
            };

            context["when used with int.max for page size"] = () =>
            {
                IPagedResponse<WebDocuments.MetadataResponse> result = null;
                beforeAll = () => result = Service.Any(new WebDocuments.FindRequest {PageSize = int.MaxValue});
                it["should return 3 records"] = () => result.Data.Count().should_be(3);
                it["should have a total of 3 records"] = () => result.Meta.Total.should_be(3);
                it["should have a current page of 1"] = () => result.Meta.CurrentPage.should_be(1);
                it["should have a page size of 1000"] = () => result.Meta.PageSize.should_be(1000);
            };

            context["when used with a page size of 1"] = () =>
            {
                IPagedResponse<WebDocuments.MetadataResponse> result = null;
                beforeAll = () => result = Service.Any(new WebDocuments.FindRequest {PageSize = 1});
                it["should return 1 record"] = () => result.Data.Count().should_be(1);
                it["should have a total of 3 records"] = () => result.Meta.Total.should_be(3);
                it["should have a current page of 1"] = () => result.Meta.CurrentPage.should_be(1);
                it["should have a page size of 1"] = () => result.Meta.PageSize.should_be(1);
            };

            context["when used with a page size of 1, and page of 2"] = () =>
            {
                IPagedResponse<WebDocuments.MetadataResponse> result = null;
                beforeAll = () => result = Service.Any(new WebDocuments.FindRequest {PageSize = 1, Page = 2});
                it["should return 1 record"] = () => result.Data.Count().should_be(1);
                it["should have a total of 3 records"] = () => result.Meta.Total.should_be(3);
                it["should have a current page of 2"] = () => result.Meta.CurrentPage.should_be(2);
                it["should have a page size of 1"] = () => result.Meta.PageSize.should_be(1);
            };

            context["when used with a page size of 1, and page of 4"] = () =>
            {
                IPagedResponse<WebDocuments.MetadataResponse> result = null;
                beforeAll = () => result = Service.Any(new WebDocuments.FindRequest {PageSize = 1, Page = 4});
                it["should return 0 records"] = () => result.Data.Count().should_be(0);
                it["should have a total of 3 records"] = () => result.Meta.Total.should_be(3);
                it["should have a current page of 4"] = () => result.Meta.CurrentPage.should_be(4);
                it["should have a page size of 1"] = () => result.Meta.PageSize.should_be(1);
            };
        }

        private void describe_create_request_handler()
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
                                SourceUrl = "google.com/?q=" + loremIpsum + loremIpsum,
                                FileName = loremIpsum
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
                        it["should have its IsUpdated flag set to false"] = () => response.IsUpdated.should_be(false);
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
                        it["should have its UsePrintMediaType flag set to false"] =
                            () => response.UsePrintMediaType.should_be(false);
                        it["should have a FileLength greater than 0"] =
                            () => response.FileLength.should_be_greater_than(0);
                        it["should have a FileName of test"] = () => response.FileName.should_be("test");
                        it["should have a ContentType of application/pdf"] =
                            () => response.ContentType.should_be("application/pdf");
                        it["should have a RecordVersion of 1"] = () => response.RecordVersion.should_be(1);
                        it["should have its IsUpdated flag set to false"] = () => response.IsUpdated.should_be(false);
                        it["should not have its Md5Sum empty"] = () => response.Md5Sum.should_not_be_empty();
                        it["should have a MarginBottom = 0"] = () => response.MarginBottom.should_be("0");
                        it["should have a MarginLeft = 0"] = () => response.MarginLeft.should_be("0");
                        it["should have a MarginRight = 0"] = () => response.MarginRight.should_be("0");
                        it["should have a MarginTop = 0"] = () => response.MarginTop.should_be("0");
                    };
                };
            };
        }

        private void describe_update_request_handler()
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
                                SourceUrl = "google.com/?q=" + loremIpsum + loremIpsum,
                                FileName = loremIpsum
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

        private void describe_find_by_id_request_handler()
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

        private void describe_delete_request_handler()
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

        private void describe_download_request_handler()
        {
            context["when used with default request object"] = () =>
            {
                it["should throw a ValidationException"] =
                    expect<ValidationException>(() => Service.Any(new WebDocuments.DownloadRequest()));
            };
            context["when used with invalid request object"] = () =>
            {
                it["should throw a ValidationException"] =
                    expect<ValidationException>(
                        () => Service.Any(
                            new WebDocuments.DownloadRequest
                            {
                                Id = Guid.Empty
                            }));
            };
            context["when used with unknown guid"] = () =>
            {
                it["should throw a HttpError"] =
                    expect<HttpError>(
                        () => Service.Any(
                            new WebDocuments.DownloadRequest
                            {
                                Id = Guid.NewGuid()
                            }));
            };
            context["when used with the first record id"] = () =>
            {
                it["should return a FileResult"] = () =>
                {
                    IHasGuidId originalRecord;
                    using (var db = AppHost.Container.Resolve<IDbConnectionFactory>().Open())
                    {
                        originalRecord = db.Select<WebDocumentMetadata>().First();
                    }
                    var result = Service.Any(
                        new WebDocuments.DownloadRequest
                        {
                            Id = originalRecord.Id
                        });
                    result.should_cast_to<FileResult>();
                };
            };
        }

        private void describe_metadata_request_handler()
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

        /// <summary>
        ///     After all specs, cleanup
        /// </summary>
        private void after_all()
        {
            AppHost.Dispose();
        }
    }
}