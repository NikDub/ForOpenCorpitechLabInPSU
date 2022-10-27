using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp2
{
    internal static class ImageChange
    {
        public static Image resizeImage(String imgToResize, Size size)
        {
            return new Bitmap(new Bitmap(imgToResize), size);
        }
    }
}
