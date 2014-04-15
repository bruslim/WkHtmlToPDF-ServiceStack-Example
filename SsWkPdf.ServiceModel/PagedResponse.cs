using System.Collections.Generic;
using ServiceStack;

namespace SsWkPdf.ServiceModel
{
    public class PagedResponse<T> : IPagedResponse<T>, IHasResponseStatus
    {
        public ResponseStatus ResponseStatus { get; set; }
        public IEnumerable<T> Data { get; set; }

        public IPagingMetadata Meta { get; set; }
    }
}