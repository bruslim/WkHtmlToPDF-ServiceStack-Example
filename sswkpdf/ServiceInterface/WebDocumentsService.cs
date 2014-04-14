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
        /// <param name="orientation"></param>
        /// <returns></returns>
        private byte[] Convert(
            string url,
            bool usePrintMediaType = false,
            string marginTop = null,
            string marginBottom = null,
            string marginLeft = null,
            string marginRight = null,
            PdfOrientation? orientation = PdfOrientation.Portrait)
        {
            Converter.ObjectSettings.Page = url;
            Converter.ObjectSettings.Web.PrintMediaType = usePrintMediaType;
            Converter.GlobalSettings.Orientation = orientation ?? Converter.GlobalSettings.Orientation;

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
                request.MarginRight,
                request.Orientation);

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
                MarginTop = request.MarginTop,
                Orientation = request.Orientation ?? PdfOrientation.Portrait
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

            // get the record from the db (use metadata, to avoid querying file)
            var metaRecord = Db.Select<WebDocumentMetadata>(r => r.Id == request.Id).FirstOrDefault();
            if (metaRecord == null)
            {
                // if not found throw 404
                throw HttpError.NotFound("Web Document {0} does not exist.".Fmt(request.Id));
            }

            // concurrency check, record versions must match
            if (metaRecord.RecordVersion != request.RecordVersion)
            {
                return new HttpError(
                    metaRecord.ToMetadataResponse(),
                    HttpStatusCode.Conflict,
                    "Conflict",
                    "Record has been updated by someone else.");
            }

            // only update if not empty or whitespace
            if (!string.IsNullOrWhiteSpace(request.SourceUrl))
            {
                metaRecord.SourceUrl = request.SourceUrl;
            }

            // only update if not empty or whitespace
            if (!string.IsNullOrWhiteSpace(request.FileName))
            {
                metaRecord.FileName = request.FileName;
            }

            // update UsePrintMediaType flag if provided
            metaRecord.UsePrintMediaType = request.UsePrintMediaType ?? metaRecord.UsePrintMediaType;

            // update margins
            metaRecord.MarginTop = request.MarginTop ?? metaRecord.MarginTop;
            metaRecord.MarginBottom = request.MarginBottom ?? metaRecord.MarginBottom;
            metaRecord.MarginLeft = request.MarginLeft ?? metaRecord.MarginLeft;
            metaRecord.MarginRight = request.MarginRight ?? metaRecord.MarginRight;

            // update orientation setting
            metaRecord.Orientation = request.Orientation ?? metaRecord.Orientation;

            // convert the html to pdf
            var file = Convert(
                metaRecord.SourceUrl,
                metaRecord.UsePrintMediaType,
                metaRecord.MarginTop,
                metaRecord.MarginBottom,
                metaRecord.MarginLeft,
                metaRecord.MarginRight,
                metaRecord.Orientation);

            // update the metadata file info fields
            metaRecord.FileLength = file.LongLength;
            metaRecord.Md5Sum = file.ToMd5Sum();

            // update the updated on date
            metaRecord.UpdatedOn = DateTimeOffset.UtcNow;

            // increase the record version
            metaRecord.RecordVersion += 1;

            // prep a WebDocument object, and populate it with the metadata 
            var record = (new WebDocument()).PopulateWith(metaRecord);

            // Update fhe File, and corresponding fields
            record.File = file;

            // validate the changes
            WebDocumentValidator.ValidateAndThrow(record);

            // Use Update to save the lookup query
            Db.Update(record);

            // use ToMeta() to remove byte[] from response
            return HttpResult.SoftRedirect(
                (new WebDocuments.FindByIdRequest {Id = record.Id}).ToGetUrl(),
                record.ToMetadataResponse());
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