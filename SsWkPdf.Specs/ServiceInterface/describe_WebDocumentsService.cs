using System.Text;
using NSpec;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.Testing;
using SsWkPdf.Common;
using SsWkPdf.Common.Util;
using SsWkPdf.ServiceInterface;
using SsWkPdf.ServiceModel.Type;

namespace SsWkPdf.Specs.ServiceInterface
{
    abstract class describe_WebDocumentsService : nspec
    {
        protected const string LoremIpsum =
            @"Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur? At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita distinctio. Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae. Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat.";

        protected ServiceStackHost AppHost { get; set; }
        protected WebDocumentsService Service { get; set; }

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

        private void describe_the_fixture_data()
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

        /// <summary>
        ///     After all specs, cleanup
        /// </summary>
        private void after_all()
        {
            AppHost.Dispose();
        }
    }
}