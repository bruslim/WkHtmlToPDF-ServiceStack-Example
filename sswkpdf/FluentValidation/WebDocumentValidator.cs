using ServiceStack.FluentValidation;
using SsWkPdf.ServiceModel.Type;

namespace SsWkPdf.FluentValidation
{
    public class WebDocumentValidator : AbstractValidator<WebDocument>
    {
        public WebDocumentValidator()
        {
            RuleFor(r => r.ContentType).NotEmpty().Length(1, 255);
            RuleFor(r => r.CreatedOn).NotEmpty();
            RuleFor(r => r.File).NotEmpty();
            RuleFor(r => r.FileLength).GreaterThan(0);
            RuleFor(r => r.FileName).NotEmpty().Length(1, 255);
            RuleFor(r => r.Id).NotEmpty();
            RuleFor(r => r.Md5Sum).NotEmpty().Length(32);
            RuleFor(r => r.RecordVersion).GreaterThan(0);
            RuleFor(r => r.SourceUrl).NotEmpty().Length(1, 2047);

            RuleFor(r => r.MarginBottom).Length(0,31);
            RuleFor(r => r.MarginLeft).Length(0,31);
            RuleFor(r => r.MarginRight).Length(0,31);
            RuleFor(r => r.MarginTop).Length(0,31);
        }
    }
}