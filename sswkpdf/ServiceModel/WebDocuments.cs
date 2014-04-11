using System;
using System.Collections;
using ServiceStack;
using ServiceStack.DataAnnotations;
using SsWkPdf.Common.Util;
using SsWkPdf.ServiceModel.Type;

namespace SsWkPdf.ServiceModel
{
    public class WebDocuments
    {
        [Route("/webdocument", "POST", Summary = "Creates the document.")]
        public class CreateRequest
        {
            public string SourceUrl { get; set; }

            public string FileName { get; set; }

            public bool? UsePrintMediaType { get; set; }

            public string MarginTop { get; set; }
            public string MarginBottom { get; set; }
            public string MarginLeft { get; set; }
            public string MarginRight { get; set; }
        }

        [Route("/webdocument/{Id}/delete", "GET DELETE", Summary = "Deletes the document by id.")]
        public class DeleteRequest : FindByIdRequest
        {
        }

        [Route("/webdocument/{Id}/download", "GET", Summary = "Downloads a document by id.")]
        public class DownloadRequest : FindByIdRequest
        {
        }


        [Route("/webdocuments", "GET", Summary = "Page through all the documents.")]
        [Route("/webdocuments/{Page}", "GET", Summary = "Page through all the documents.")]
        public class FindRequest : IReturn<PagedResponse<MetadataResponse>>
        {
            public int Page { get; set; }
            public int PageSize { get; set; }

            public DateTimeOffset? From { get; set; }

            public DateTimeOffset? To { get; set; }
        }

        [Route("/webdocument/{Id}", "GET", Summary = "View a document by id.")]
        [Route("/webdocument/{Id}", "DELETE", Summary = "Deletes the document by id.")]
        public class FindByIdRequest
        {
            public Guid Id { get; set; }
        }

        [Route("/webdocument/{Id}/metadata", "GET", Summary = "View the document metadata by id.")]
        public class MetadataRequest : FindByIdRequest, IReturn<MetadataResponse>
        {
        }

        [Route("/webdocument", "POST", Summary = "Updates the document.")]
        [Route("/webdocument/{Id}", "POST PUT", Summary = "Updates the document.")]
        public class UpdateRequest : CreateRequest
        {
            public Guid Id { get; set; }

            [Description("Record Version when first retrieved, for concurrency checking.")]
            public int RecordVersion { get; set; }
        }

        public class MetadataResponse : WebDocumentMetadata
        {
            public string FileSize
            {
                get { return Calculator.BytesToString(FileLength); }
            }

            public bool IsUpdated
            {
                get { return RecordVersion > 1; }
            }
        }
    }
}