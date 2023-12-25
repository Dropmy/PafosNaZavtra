using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test01
{
    public static class ImagesController
    {
        public const string imageFolder = "Images";
        public const string defaultImage = "default.png";

        public static Image GetImageByPath(string path)
        {
            if (path is null)
                path = defaultImage;
            var fullPath = Path.Combine(Environment.CurrentDirectory, imageFolder, path);

            return Image.FromFile(fullPath);
        }
    }
}
