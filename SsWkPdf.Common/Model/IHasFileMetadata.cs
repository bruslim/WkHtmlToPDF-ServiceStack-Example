using ServiceStack.DataAnnotations;

namespace SsWkPdf.Model
{
    public interface IHasFileMetadata : IRecord
    {
        [Required, CustomField("NVARCHAR"), StringLength(256)]
        string FileName { get; set; }

        [Required]
        long FileLength { get; set; }

        [Required, StringLength(127)]
        string ContentType { get; set; }

        [Required, CustomField("CHAR"), StringLength(32, MinimumLength = 32)]
        string Md5Sum { get; set; }
    }
}