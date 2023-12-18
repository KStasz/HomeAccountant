using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAccountant.Core.Extensions
{
    public static class ColorExtensions
    {
        public static string ConvertToRgb(this Color color)
        {
            return $"rgb({color.R}, {color.G}, {color.B})";
        }
    }
}
