using System;

namespace SsWkPdf.Common.Util
{
    public static class Calculator
    {
        /// <summary>
        ///     http://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net
        /// </summary>
        /// <param name="byteCount">The byte count.</param>
        /// <returns></returns>
        public static String BytesToString(long byteCount)
        {
            string[] sizeSuffixes = {"B", "KB", "MB", "GB", "TB", "PB", "EB"}; //Longs run out around EB
            if (byteCount == 0)
            {
                return "0" + sizeSuffixes[0];
            }
            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes/Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount)*num) + sizeSuffixes[place];
        }
    }
}