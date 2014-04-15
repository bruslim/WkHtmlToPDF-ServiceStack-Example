namespace SsWkPdf.ServiceModel
{
    public interface IPagingMetadata
    {
        long Total { get; set; }

        int CurrentPage { get; set; }

        int PageSize { get; set; }
    }
}