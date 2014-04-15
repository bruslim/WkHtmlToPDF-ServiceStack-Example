using System;
using ServiceStack.DataAnnotations;

namespace SsWkPdf.Model
{
    public interface IUpdateable
    {
        [Required]
        DateTimeOffset UpdatedOn { get; set; }

        [Required, Default(1)]
        int RecordVersion { get; set; }
    }
}