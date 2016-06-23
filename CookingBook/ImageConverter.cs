using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CookingBook
{
   public static class ImageConverter
    {
        public static byte[] ConvertFileToByte(string filename)
        {
            var bytes = File.ReadAllBytes(filename);
            return bytes;
        }
    }
}
