using System;
using System.Collections.Generic;
using System.IO;
using ServiceStack;
using ServiceStack.Web;
using SsWkPdf.Model;

namespace SsWkPdf.Common
{
    public class FileResult : IHasOptions, IStreamWriter
    {
        public FileResult(IHasFile record, bool isAttachment = true)
        {
            if (record == null)
            {
                throw new ArgumentException("record");
            }

            Stream = new MemoryStream(record.File);
            Options = new Dictionary<string, string>
            {
                {HttpHeaders.ContentType, record.ContentType},
                {
                    HttpHeaders.ContentDisposition, string.Format(
                        @"{4}filename=""{0}""; size={1}; creation-date={2}; read-date={3}",
                        record.FileName,
                        record.File.LongLength,
                        record.CreatedOn.UtcDateTime.ToString("R").Replace(",", ""),
                        DateTimeOffset.UtcNow.ToString("R").Replace(",", ""),
                        isAttachment ? "attachment; " : string.Empty)
                }
            };
        }

        public FileResult(byte[] bytes, string fileName, string contentType, bool isAttachment = true) :
            this(new MemoryStream(bytes), fileName, contentType, isAttachment)
        {
        }

        public FileResult(Stream stream, string fileName, string contentType, bool isAttachment = true)
        {
            Stream = stream;

            Options = new Dictionary<string, string>
            {
                {HttpHeaders.ContentType, contentType},
                {
                    HttpHeaders.ContentDisposition, string.Format(
                        @"{1}filename=""{0}"";",
                        fileName,
                        isAttachment ? "attachment; " : string.Empty)
                }
            };
        }

        public Stream Stream { get; set; }

        public IDictionary<string, string> Options { get; private set; }

        public void WriteTo(Stream responseStream)
        {
            if (Stream == null)
            {
                return;
            }

            Stream.WriteTo(responseStream);
            responseStream.Flush();
        }
    }
}