using System;

namespace SsWkPdf.FluentValidation
{
    public interface IUriValidator
    {
        bool ValidUri(string url, UriKind urkKind = UriKind.RelativeOrAbsolute);
    }
}