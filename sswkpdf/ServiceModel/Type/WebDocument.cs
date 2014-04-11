using ServiceStack.DataAnnotations;
using SsWkPdf.Model;

namespace SsWkPdf.ServiceModel.Type
{
    public class WebDocument : WebDocumentMetadata, IHasFile
    {
        [Required]
        public byte[] File { get; set; }
    }
}