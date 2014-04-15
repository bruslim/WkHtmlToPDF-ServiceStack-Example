using ServiceStack.FluentValidation;
using SsWkPdf.FluentValidation;
using SsWkPdf.ServiceModel;

namespace SsWkPdf.ServiceInterface.FluentValidation
{
    public class WebDocumentsValidators
    {
        public class Create : CreateAbstractValidator<WebDocuments.CreateRequest>
        {
            public Create()
            {
                RuleFor(r => r.SourceUrl)
                    .NotEmpty()
                    .Length(1, 2047)
                    .Must(r => UriValidator.ValidUri(r));
            }

            public IUriValidator UriValidator { get; set; }
        }

        public abstract class CreateAbstractValidator<T> : AbstractValidator<T> where T : WebDocuments.CreateRequest
        {
            protected CreateAbstractValidator()
            {
                RuleFor(r => r.FileName).Length(0, 255);

                RuleFor(r => r.MarginBottom).Length(0, 31);
                RuleFor(r => r.MarginLeft).Length(0, 31);
                RuleFor(r => r.MarginRight).Length(0, 31);
                RuleFor(r => r.MarginTop).Length(0, 31);
            }
        }

        public class FindById : AbstractValidator<WebDocuments.FindByIdRequest>
        {
            public FindById()
            {
                RuleFor(r => r.Id).NotEmpty();
            }
        }

        public class Update : CreateAbstractValidator<WebDocuments.UpdateRequest>
        {
            public Update()
            {
                RuleFor(r => r.Id).NotEmpty();

                RuleFor(r => r.RecordVersion).NotEmpty();

                RuleFor(r => r.SourceUrl)
                    .Length(0, 2047)
                    .Must(v => string.IsNullOrWhiteSpace(v) || UriValidator.ValidUri(v));
            }

            public IUriValidator UriValidator { get; set; }
        }
    }
}