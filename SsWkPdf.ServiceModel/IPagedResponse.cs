using System.Collections.Generic;

namespace SsWkPdf.ServiceModel
{
    public interface IPagedResponse<T>
    {
        IEnumerable<T> Data { get; set; }

        IPagingMetadata Meta { get; set; }
    }
}