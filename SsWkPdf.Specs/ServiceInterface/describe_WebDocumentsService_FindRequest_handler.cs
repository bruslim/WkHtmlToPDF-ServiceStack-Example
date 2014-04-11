using System.Linq;
using NSpec;
using SsWkPdf.ServiceModel;

namespace SsWkPdf.Specs.ServiceInterface
{
    internal class describe_WebDocumentsService_FindRequest_handler : describe_WebDocumentsService
    {
        private void given_the_fixture_data()
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
    }
}