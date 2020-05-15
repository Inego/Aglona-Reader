using System;
using System.Drawing;

namespace AglonaReader
{

    // Code taken from:
    // http://www.geekymonkey.com/Programming/CSharp/RGB2HSL_HSL2RGB.htm

    public struct ColorRGB
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
       
        public ColorRGB(Color value) : this()
        {
            
            Red = value.R;
            Green = value.G;
            Blue = value.B;
        }

        public static implicit operator Color(ColorRGB rgb)
        {
            var c = Color.FromArgb(rgb.Red, rgb.Green, rgb.Blue);
            return c;
        }

        public static explicit operator ColorRGB(Color color)
        {
            return new ColorRGB(color);
        }

        // Given H,S,L in range of 0-1
        // Returns a Color (RGB struct) in range of 0-255
        public static ColorRGB HSL2RGB(double hue, double saturation, double lighting)
        {
            double v;
            double r, g, b;

            r = lighting;   // default to gray
            g = lighting;
            b = lighting;
            v = lighting <= 0.5 ? lighting * (1.0 + saturation) : lighting + saturation - lighting * saturation;
            
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = lighting + lighting - v;
                sv = (v - m) / v;
                hue *= 6.0;
                sextant = (int)hue;
                fract = hue - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
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


            var rgb = new ColorRGB();
            
            rgb.Red = Convert.ToByte(r * 255.0f);
            rgb.Green = Convert.ToByte(g * 255.0f);
            rgb.Blue = Convert.ToByte(b * 255.0f);
            
            return rgb;
        }
    }


    


}
