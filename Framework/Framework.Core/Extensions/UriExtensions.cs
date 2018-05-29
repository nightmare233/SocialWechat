using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    public static class UriExtensions
    {
        public static string GetMimeType(this Uri uri)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(uri.AbsolutePath);
                string mimeType = MimeTypeMap.GetMimeType(fileInfo.Extension);
                return mimeType;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
