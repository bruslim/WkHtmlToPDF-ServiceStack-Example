using ServiceStack.DataAnnotations;

namespace SsWkPdf.Model
{
    public interface IHasFile : IHasFileMetadata
    {
        [Required]
        byte[] File { get; set; }
    }
}