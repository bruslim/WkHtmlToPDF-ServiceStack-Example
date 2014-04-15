using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SsWkPdf.Common.Util
{
    public static class ByteArrayExtensions
    {
        public static byte[] ComputeMd5(this byte[] data)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(data);
            }
        }

        public static byte[] ComputeSha512(this byte[] data)
        {
            using (var sha512 = SHA512.Create())
            {
                return sha512.ComputeHash(data);
            }
        }

        public static string ToBase64(this byte[] data)
        {
            return HttpServerUtility.UrlTokenEncode(data);
        }

        public static byte[] FromBase64(this string data)
        {
            return HttpServerUtility.UrlTokenDecode(data);
        }

        /// <summary>
        ///     Gets the MD5 Sum.
        /// </summary>
        /// <param name="data">The data to compute the hash from.</param>
        /// <returns></returns>
        public static string ToMd5Sum(this string data)
        {
            return Stringify(ComputeMd5(Encoding.UTF8.GetBytes(data)));
        }

        /// <summary>
        ///     Gets the MD5 Sum.
        /// </summary>
        /// <param name="data">The data to compute the hash from.</param>
        /// <returns></returns>
        public static string ToMd5Sum(this byte[] data)
        {
            return Stringify(ComputeMd5(data));
        }

        /// <summary>
        ///     Gets the sha512.
        /// </summary>
        /// <param name="data">The data to compute the hash from.</param>
        /// <returns></returns>
        public static string ToSha512Sum(this string data)
        {
            return ToSha512Sum(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        ///     Gets the sha512.
        /// </summary>
        /// <param name="data">The data to compute the hash from.</param>
        /// <returns></returns>
        public static string ToSha512Sum(this byte[] data)
        {
            return Stringify(ComputeSha512(data));
        }

        /// <summary>
        ///     Stringify the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private static string Stringify(this byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", "").ToLower();
        }
    }
}