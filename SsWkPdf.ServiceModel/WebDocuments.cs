using System;
using ServiceStack;
using ServiceStack.DataAnnotations;
using SsWkPdf.Common.Util;
using SsWkPdf.ServiceModel.Type;
using WkHtmlToXSharp;

namespace SsWkPdf.ServiceModel
{
    public class WebDocuments
    {
        [Route("/webdocument", "POST", Summary = "Creates the document.")]
        public class CreateRequest : IReturn<WebDocumentMetadata>
        {
            /// <summary>
            /// Gets or sets the source URL.
            /// </summary>
            /// <value>
            /// The source URL.
            /// </value>
            public string SourceUrl { get; set; }

            /// <summary>
            /// Gets or sets the name of the file.
            /// </summary>
            /// <value>
            /// The name of the file.
            /// </value>
            public string FileName { get; set; }

            /// <summary>
            /// Gets or sets the type of the use print media.
            /// </summary>
            /// <value>
            /// The type of the use print media.
            /// </value>
            public bool? UsePrintMediaType { get; set; }

            /// <summary>
            /// Gets or sets the margin top.
            /// </summary>
            /// <value>
            /// The margin top.
            /// </value>
            public string MarginTop { get; set; }

            /// <summary>
            /// Gets or sets the margin bottom.
            /// </summary>
            /// <value>
            /// The margin bottom.
            /// </value>
            public string MarginBottom { get; set; }

            /// <summary>
            /// Gets or sets the margin left.
            /// </summary>
            /// <value>
            /// The margin left.
            /// </value>
            public string MarginLeft { get; set; }

            /// <summary>
            /// Gets or sets the margin right.
            /// </summary>
            /// <value>
            /// The margin right.
            /// </value>
            public string MarginRight { get; set; }

            /// <summary>
            /// Gets or sets the orientation.
            /// </summary>
            /// <value>
            /// The orientation.
            /// </value>
            public PdfOrientation? Orientation { get; set; }
        }

        [Route("/webdocument/{Id}/delete", "GET DELETE", Summary = "Deletes the document by id.")]
        public class DeleteRequest : FindByIdRequest
        {
        }

        [Route("/webdocument/{Id}/download", "GET", Summary = "Downloads a document by id.")]
        public class DownloadRequest : FindByIdRequest
        {
        }


        [Route("/webdocument/{Id}", "GET", Summary = "View a document by id.")]
        [Route("/webdocument/{Id}", "DELETE", Summary = "Deletes the document by id.")]
        public class FindByIdRequest  : IReturn<WebDocumentMetadata>
        {
            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            /// <value>
            /// The identifier.
            /// </value>
            public Guid Id { get; set; }
        }

        [Route("/webdocuments", "GET", Summary = "Page through all the documents.")]
        [Route("/webdocuments/{Page}", "GET", Summary = "Page through all the documents.")]
        public class FindRequest : IReturn<PagedResponse<MetadataResponse>>
        {
            /// <summary>
            /// Gets or sets the page.
            /// </summary>
            /// <value>
            /// The page.
            /// </value>
            public int Page { get; set; }

            /// <summary>
            /// Gets or sets the size of the page.
            /// </summary>
            /// <value>
            /// The size of the page.
            /// </value>
            public int PageSize { get; set; }

            /// <summary>
            /// Gets or sets from.
            /// </summary>
            /// <value>
            /// From.
            /// </value>
            public DateTimeOffset? From { get; set; }

            /// <summary>
            /// Gets or sets to.
            /// </summary>
            /// <value>
            /// To.
            /// </value>
            public DateTimeOffset? To { get; set; }
        }

        [Route("/webdocument/{Id}/metadata", "GET", Summary = "View the document metadata by id.")]
        public class MetadataRequest : FindByIdRequest, IReturn<MetadataResponse>
        {
        }

        public class MetadataResponse : WebDocumentMetadata, IHasResponseStatus
        {
            /// <summary>
            /// Gets or sets the response status.
            /// </summary>
            /// <value>
            /// The response status.
            /// </value>
            public ResponseStatus ResponseStatus { get; set; }
        }

        [Route("/webdocument", "POST", Summary = "Updates the document.")]
        [Route("/webdocument/{Id}", "POST PUT", Summary = "Updates the document.")]
        public class UpdateRequest : CreateRequest
        {
            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            /// <value>
            /// The identifier.
            /// </value>
            public Guid Id { get; set; }

            /// <summary>
            /// Gets or sets the record version.
            /// </summary>
            /// <value>
            /// The record version.
            /// </value>
            [Description("Record Version when first retrieved, for concurrency checking.")]
            public int RecordVersion { get; set; }
        }
    }

}