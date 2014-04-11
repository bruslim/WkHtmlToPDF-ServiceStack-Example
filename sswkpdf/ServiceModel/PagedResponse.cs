using System.Collections.Generic;

namespace SsWkPdf.ServiceModel
{
    public class PagedResponse<T> : IPagedResponse<T>
    {
        public IEnumerable<T> Data { get; set; }

        public IPagingMetadata Meta { get; set; }
    }
}