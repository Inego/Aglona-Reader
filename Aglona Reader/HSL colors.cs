using System;
using System.Drawing;

namespace AglonaReader
{

    // Code taken from:
    // http://www.geekymonkey.com/Programming/CSharp/RGB2HSL_HSL2RGB.htm

    public struct ColorRgb
    {
        private byte Red { get; set; }
        private byte Green { get; set; }
        private byte Blue { get; set; }

        private ColorRgb(Color value) : this()
        {
            
            Red = value.R;
            Green = value.G;
            Blue = value.B;
        }

        public static implicit operator Color(ColorRgb rgb)
        {
            var c = Color.FromArgb(rgb.Red, rgb.Green, rgb.Blue);
            return c;
        }

        public static explicit operator ColorRgb(Color color)
        {
            return new ColorRgb(color);
        }

        // Given H,S,L in range of 0-1
        // Returns a Color (RGB struct) in range of 0-255
        public static ColorRgb Hsl2Rgb(double hue, double saturation, double lighting)
        {
            var r = lighting;
            var g = lighting;
            var b = lighting;
            
            var v = lighting <= 0.5 ? lighting * (1.0 + saturation) : lighting + saturation - lighting * saturation;
            
            if (v > 0)
            {
                var m = lighting + lighting - v;
                var sv = (v - m) / v;
                hue *= 6.0;
                var sextant = (int)hue;
                var fraction = hue - sextant;
                var vsf = v * sv * fraction;
                var mid1 = m + vsf;
                var mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }


            var rgb = new ColorRgb
            {
                Red = Convert.ToByte(r * 255.0f),
                Green = Convert.ToByte(g * 255.0f),
                Blue = Convert.ToByte(b * 255.0f)
            };

            return rgb;
        }
    }


    


}
