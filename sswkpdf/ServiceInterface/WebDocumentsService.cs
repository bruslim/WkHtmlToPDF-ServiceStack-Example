using System;
using System.Linq;
using System.Net;
using ServiceStack;
using ServiceStack.FluentValidation;
using ServiceStack.OrmLite;
using SsWkPdf.Common;
using SsWkPdf.Common.Util;
using SsWkPdf.ServiceModel;
using SsWkPdf.ServiceModel.Type;
using WkHtmlToXSharp;

namespace SsWkPdf.ServiceInterface
{
    public class WebDocumentsService : Service
    {
        public const int MaxPageSize = 1000;
        public const int DefaultPageSize = 100;

        // Injected public properties
        public IValidator<WebDocument> WebDocumentValidator { get; set; }

        public IValidator<WebDocuments.FindByIdRequest> FindByIdValidator { get; set; }

        public IValidator<WebDocuments.CreateRequest> CreateValidator { get; set; }

        public IValidator<WebDocuments.UpdateRequest> UpdateValidator { get; set; }

        public IHtmlToPdfConverter Converter { get; set; }

        /// <summary>
        ///     Converts the specified URL into a PDF. This call is synchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="usePrintMediaType">if set to <c>true</c> [use print media type].</param>
        /// <param name="marginTop">The margin top.</param>
        /// <param name="marginBottom">The margin bottom.</param>
        /// <param name="marginLeft">The margin left.</param>
        /// <param name="marginRight">The margin right.</param>
        /// <returns></returns>
        private byte[] Convert(
            string url,
            bool usePrintMediaType = false,
            string marginTop = null,
            string marginBottom = null,
            string marginLeft = null,
            string marginRight = null)
        {
            Converter.ObjectSettings.Page = url;
            Converter.ObjectSettings.Web.PrintMediaType = usePrintMediaType;

            // setup margins
            var margin = Converter.GlobalSettings.Margin;
            margin.Bottom = marginBottom ?? margin.Bottom;
            margin.Left = marginLeft ?? margin.Left;
            margin.Right = marginRight ?? margin.Right;
            margin.Top = marginTop ?? margin.Top;

            return Converter.Convert();
        }

        /// <summary>
        ///     Gets the web document from the database. Throws an exception when document is not found.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="HttpError">When document is not found</exception>
        private WebDocument GetWebDocument(Guid id)
        {
            var record = Db.Select<WebDocument>(r => r.Id == id).FirstOrDefault();
            if (record == null)
            {
                // if not found throw 404
                throw HttpError.NotFound("Web Document {0} does not exist.".Fmt(id));
            }
            return record;
        }

        public IPagedResponse<WebDocuments.MetadataResponse> Any(WebDocuments.FindRequest request)
        {
            // normalize paging with max and min
            var page = Math.Max(1, request.Page);
            var pageSize = Math.Min(MaxPageSize, Math.Max(1, request.PageSize > 0 ? request.PageSize : DefaultPageSize));
            // upper bounds of skip is int.Max, do not compute if page is int.max, just set to upper bound
            var skip = page == int.MaxValue ? int.MaxValue : pageSize*(page - 1);

            // use the WebDocumentMetadata type to avoid loading the 'File VARBINARY(MAX)' field
            return new PagedResponse<WebDocuments.MetadataResponse>
            {
                Data = Db.Select<WebDocumentMetadata>(
                    q =>
                        q.Where(
                            r =>
                                (!request.From.HasValue || r.CreatedOn >= request.From)
                                && (!request.To.HasValue || r.CreatedOn <= request.To))
                            .OrderBy(r => r.Id) // always order by when paging
                            .Limit(skip, pageSize)).Select(r => r.ToMetadataResponse()),
                Meta = new PagingMetadata
                {
                    Total = Db.Count<WebDocumentMetadata>(),
                    PageSize = pageSize,
                    CurrentPage = page
                }
            };
        }

        public object Any(WebDocuments.CreateRequest request)
        {
            // validate request
            CreateValidator.ValidateAndThrow(request);

            // default the optional flag
            var usePrintMediaType = request.UsePrintMediaType ?? true;

            // convert the html to pdf
            var file = Convert(
                request.SourceUrl,
                usePrintMediaType,
                request.MarginTop,
                request.MarginBottom,
                request.MarginLeft,
                request.MarginRight);

            // create the new record
            var record = new WebDocument
            {
                FileName = string.IsNullOrWhiteSpace(request.FileName) ? "Document.pdf" : request.FileName,
                File = file,
                FileLength = file.LongLength,
                ContentType = "application/pdf",
                Md5Sum = file.ToMd5Sum(),
                SourceUrl = request.SourceUrl,
                UsePrintMediaType = usePrintMediaType,
                MarginBottom = request.MarginBottom,
                MarginLeft = request.MarginLeft,
                MarginRight = request.MarginRight,
                MarginTop = request.MarginTop
            };

            // validate the changes
            WebDocumentValidator.ValidateAndThrow(record);

            // Use insert to save the lookup query
            Db.Insert(record);

            // use ToMetadata() to remove byte[] from response
            return HttpResult.Status201Created(
                record.ToMetadataResponse(),
                (new WebDocuments.FindByIdRequest {Id = record.Id}).ToGetUrl());
        }

        public object Any(WebDocuments.UpdateRequest request)
        {
            // use validator to validate request
            UpdateValidator.ValidateAndThrow(request);

            // get the record from the db
            var record = GetWebDocument(request.Id);

            // concurrency check
            if (record.RecordVersion != request.RecordVersion)
            {
                return new HttpError(
                    record.ToMetadataResponse(),
                    HttpStatusCode.Conflict,
                    "Conflict",
                    "Record has been updated by someone else.");
            }

            // only update if not empty or whitespace
            if (!string.IsNullOrWhiteSpace(request.SourceUrl))
            {
                record.SourceUrl = request.SourceUrl;
            }

            // only update if not empty or whitespace
            if (!string.IsNullOrWhiteSpace(request.FileName))
            {
                record.FileName = request.FileName;
            }

            // update UsePrintMediaType flag if provided
            record.UsePrintMediaType = request.UsePrintMediaType ?? record.UsePrintMediaType;

            // update margins
            record.MarginTop = request.MarginTop ?? record.MarginTop;
            record.MarginBottom = request.MarginBottom ?? record.MarginBottom;
            record.MarginLeft = request.MarginLeft ?? record.MarginLeft;
            record.MarginRight = request.MarginRight ?? record.MarginRight;

            // convert the html to pdf
            var file = Convert(
                record.SourceUrl,
                record.UsePrintMediaType,
                record.MarginTop,
                record.MarginBottom,
                record.MarginLeft,
                record.MarginRight);

            // Update fhe File, and corresponding fields
            record.File = file;
            record.FileLength = file.LongLength;
            record.Md5Sum = file.ToMd5Sum();
            record.UpdatedOn = DateTimeOffset.UtcNow;
            record.RecordVersion += 1;

            // validate the changes
            WebDocumentValidator.ValidateAndThrow(record);

            // Use Update to save the lookup query
            Db.Update(record);

            // use ToMeta() to remove byte[] from response
            return new HttpResult(record.ToMetadataResponse());
        }

        public object Get(WebDocuments.FindByIdRequest request)
        {
            // use validator to validate request
            FindByIdValidator.ValidateAndThrow(request);

            return new FileResult(GetWebDocument(request.Id), false);
        }

        public object Any(WebDocuments.DeleteRequest request)
        {
            return Delete(request);
        }

        public object Delete(WebDocuments.FindByIdRequest request)
        {
            // use validator to validate request
            FindByIdValidator.ValidateAndThrow(request);

            // delete the record by id (use ByIds for more consistent performance)
            // testing shows that with sqlite(:memory:) db, DeleteById does not work
            var count = Db.DeleteByIds<WebDocument>(new[] {request.Id});

            if (count == 0)
            {
                // nothing deleted, id not found
                throw HttpError.NotFound("Web Document {0} does not exist.".Fmt(request.Id));
            }

            return new HttpResult(HttpStatusCode.NoContent, "Deleted Web Document {0}".Fmt(request.Id));
        }

        public object Any(WebDocuments.DownloadRequest request)
        {
            FindByIdValidator.ValidateAndThrow(request);

            return new FileResult(GetWebDocument(request.Id));
        }

        public WebDocuments.MetadataResponse Any(WebDocuments.MetadataRequest request)
        {
            FindByIdValidator.ValidateAndThrow(request);

            // use the WebDocumentMetadata type to avoid loading the 'File VARBINARY(MAX)' field
            var record = Db.SelectByIds<WebDocumentMetadata>(new[] {request.Id}).FirstOrDefault();
            if (record == null)
            {
                // if not found throw 404
                throw HttpError.NotFound("Web Document {0} does not exist.".Fmt(request.Id));
            }

            return record.ToMetadataResponse();
        }
    }
}