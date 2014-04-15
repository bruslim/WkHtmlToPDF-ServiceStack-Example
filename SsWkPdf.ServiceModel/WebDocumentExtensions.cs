using ServiceStack;
using SsWkPdf.Common.Util;
using SsWkPdf.ServiceModel.Type;

namespace SsWkPdf.ServiceModel
{
    public static class WebDocumentExtensions
    {
        public static WebDocuments.MetadataResponse ToMetadataResponse(this WebDocumentMetadata document)
        {
            return (new WebDocuments.MetadataResponse()).PopulateWith(document);
        }

        public static bool IsUpdated(this WebDocumentMetadata record)
        {
            return record.RecordVersion > 1;
        }

        public static string FileSize(this WebDocumentMetadata record)
        {
            return Calculator.BytesToString(record.FileLength);
        }
    }
}