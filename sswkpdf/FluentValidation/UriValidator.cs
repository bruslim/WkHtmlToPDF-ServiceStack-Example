using System;

namespace SsWkPdf.FluentValidation
{
    public class UriValidator : IUriValidator
    {
        public bool ValidUri(string url, UriKind urkKind = UriKind.RelativeOrAbsolute)
        {
            return Uri.IsWellFormedUriString(url, urkKind);
        }
    }
}