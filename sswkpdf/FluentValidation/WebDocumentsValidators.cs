using ServiceStack.FluentValidation;
using SsWkPdf.ServiceModel;

namespace SsWkPdf.FluentValidation
{
    public class WebDocumentsValidators
    {
        public class Create : AbstractValidator<WebDocuments.CreateRequest>
        {
            public Create()
            {
                RuleFor(r => r.SourceUrl)
                    .NotEmpty()
                    .Length(1, 512)
                    .Must(r => UriValidator.ValidUri(r));
            }

            public IUriValidator UriValidator { get; set; }
        }

        public class FindById : AbstractValidator<WebDocuments.FindByIdRequest>
        {
            public FindById()
            {
                RuleFor(r => r.Id).NotEmpty();
            }
        }

        public class Update : AbstractValidator<WebDocuments.UpdateRequest>
        {
            public Update()
            {
                RuleFor(r => r.Id).NotEmpty();

                RuleFor(r => r.RecordVersion).NotEmpty();

                RuleFor(r => r.SourceUrl)
                    .Must(v => string.IsNullOrWhiteSpace(v) || UriValidator.ValidUri(v));
            }

            public IUriValidator UriValidator { get; set; }
        }
    }
}