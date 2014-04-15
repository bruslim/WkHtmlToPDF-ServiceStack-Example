using System;
using ServiceStack.DataAnnotations;
using ServiceStack.Model;

namespace SsWkPdf.Model
{
    public interface IRecord : IHasGuidId
    {
        [Required, Default(typeof (DateTimeOffset), "SYSDATETIMEOFFSET()")]
        DateTimeOffset CreatedOn { get; set; }
    }
}