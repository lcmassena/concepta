using System.Collections.Generic;
using System.IO;

namespace Massena.Infrastructure.Images
{
    public class UploadedImage
    {
        public static IEnumerable<string> AllowedExtensions = new List<string>() { "jpg", "png", "gif" };
        public static int MaxContentLengthInBytes = 1024 * 1024 * 1; //Size = 1 MB

        public UploadedImage(Stream imageData, string fileName, int contentLength)
        {
            FileName = fileName;
            ImageStream = imageData;
            ContentLength = contentLength;

            var ext = fileName.Substring(fileName.LastIndexOf('.') + 1);
            Extension = ext.ToLower();
        }

        public string Extension { get; internal set; }

        public Stream ImageStream { get; internal set; }

        public int ContentLength { get; internal set; }

        public string FileName { get; internal set; }

        public void ChangeFileName(string fileName)
        {
            FileName = fileName;
        }
    }
}
