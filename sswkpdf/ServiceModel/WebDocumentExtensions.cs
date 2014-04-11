using ServiceStack;
using SsWkPdf.ServiceModel.Type;

namespace SsWkPdf.ServiceModel
{
    public static class WebDocumentExtensions
    {
        public static WebDocuments.MetadataResponse ToMetadataResponse(this WebDocumentMetadata document)
        {
            return (new WebDocuments.MetadataResponse()).PopulateWith(document);
        }
    }
}