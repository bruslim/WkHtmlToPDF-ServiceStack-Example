using System;
using ServiceStack.DataAnnotations;
using SsWkPdf.Model;

namespace SsWkPdf.ServiceModel.Type
{
    [Alias("WebDocument")]
    public class WebDocumentMetadata : IHasFileMetadata, IUpdateable
    {
        public WebDocumentMetadata()
        {
            Id = GuidUtil.NewSequentialGuid();
            CreatedOn = DateTimeOffset.UtcNow;
            UpdatedOn = CreatedOn;
            RecordVersion = 1;
        }

        [StringLength(31)]
        public string MarginBottom { get; set; }

        [StringLength(31)]
        public string MarginLeft { get; set; }
        
        [StringLength(31)]
        public string MarginRight { get; set; }
        
        [StringLength(31)]
        public string MarginTop { get; set; }

        // use nvarchar for unicode names
        [Required, CustomField("NVARCHAR(2047)"), StringLength(2047)]
        public string SourceUrl { get; set; }
        
        [Required]
        public bool UsePrintMediaType { get; set; }
        
        public DateTimeOffset UpdatedOn { get; set; }

        [PrimaryKey]
        public Guid Id { get; set; }

        // use nvarchar for unicode names
        [Required, CustomField("NVARCHAR(255)"), StringLength(255)]
        public string FileName { get; set; }

        [Required]
        public long FileLength { get; set; }

        [Required, StringLength(255)]
        public string ContentType { get; set; }

        [Required, CustomField("CHAR(32)"), StringLength(32)]
        public string Md5Sum { get; set; }


        [Required, Default(typeof(DateTimeOffset), "SYSDATETIMEOFFSET()")]
        public DateTimeOffset CreatedOn { get; set; }

       
        [Required, Default(1)]
        public int RecordVersion { get; set; }
    }
}