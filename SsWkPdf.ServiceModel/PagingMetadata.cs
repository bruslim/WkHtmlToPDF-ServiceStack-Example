namespace SsWkPdf.ServiceModel
{
    public class PagingMetadata : IPagingMetadata
    {
        public long Total { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }
    }
}