using System;
using System.Windows;
using System.Windows.Media;
using User.SoftWare;

namespace User.UI
{
    public struct ColorP: IUSettingsConvertArray
    {
        public static readonly ColorP Empty = new ColorP();

        Point location;
        byte light;
        byte alpha;

        public Point Location { get => location; }
        public byte Light { get => light; }
        public byte Alpha { get => alpha; }

        public ColorP(Point location, byte light, byte alpha)
        {
            if (location.X >= 0.0 && location.X <= 6.0 && location.Y >= 0.0 && location.Y <= 1.0)
            {
                this.location = location;
                this.light = light;
                this.alpha = alpha;
            }
            else
            {
                throw new ArgumentException("location参数无效.");
            }
        }
        public ColorP(Color color)
        {
            Color colormax = GetMaxColor(color, out double bi);
            Console.WriteLine(bi);
            this.alpha = color.A;
            this.light = Convert.ToByte(255 * bi);
            double[] lo = GetLocation(colormax);
            this.location = new Point(lo[0], lo[1]);
        }

        public Color[] GetColorPickers()
        {
            Color[] colors = new Color[6];
            colors[0] = Color.FromArgb(Alpha, Light, 0, 0);
            colors[1] = Color.FromArgb(Alpha, Light, Light, 0);
            colors[2] = Color.FromArgb(Alpha, 0, Light, 0);
            colors[3] = Color.FromArgb(Alpha, 0, Light, Light);
            colors[4] = Color.FromArgb(Alpha, 0, 0, Light);
            colors[5] = Color.FromArgb(Alpha, Light, 0, Light);
            return colors;
        }
        public Color[,] GetColorSquares()
        {
            Color[] colorpickers = GetColorPickers();
            Color colorgray = GetGrayColor();
            Color[,] colorsquares = new Color[20, 6];
            for (int i = 0; i <= colorsquares.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= colorsquares.GetUpperBound(1); j++)
                {
                    colorsquares[i, j] = GetMediumColor(colorpickers[j % 6], colorgray, i / 20.0);
                }
            }
            return colorsquares;
        }
        public Color GetGrayColor()
        {
            return Color.FromArgb(Alpha, Light, Light, Light);
        }
        public Color GetColor()
        {
            Color[] colorpickers = GetColorPickers();
            int x = (int)Location.X % 6;
            double xf = (Location.X - (double)x) % 6;
            Color colorX = ColorP.GetMediumColor(colorpickers[x], colorpickers[(x + 1) % 6], xf);
            Color colorB = GetGrayColor();
            return ColorP.GetMediumColor(colorX, colorB, Location.Y);
        }
        private static double[] GetLocation(Color color)
        {
            if (color.R == 255 && color.G == 255 & color.B == 255)
            {
                return new double[] { 0, 1 };
            }
            else
            {
                int[] colororder = GetColorOrder(color);
                if (colororder[0] == 1 && colororder[1] == 2)
                {
                    //return color.G / 255.0;
                    return new double[] { Tools.GetBi(color.B, 255, color.G), color.B / 255.0 };
                }
                else if (colororder[0] == 1 && colororder[1] == 3)
                {
                    //return 6.0 - color.B / 255.0;
                    return new double[] { 6.0 - Tools.GetBi(color.G, 255, color.B), color.G / 255.0 };
                }
                else if (colororder[0] == 2 && colororder[1] == 1)
                {
                    //return 2.0 - color.R / 255.0;
                    return new double[] { 2.0 - Tools.GetBi(color.B, 255, color.R), color.B / 255.0 };
                }
                else if (colororder[0] == 2 && colororder[1] == 3)
                {
                    //return 2.0 + color.B / 255.0;
                    return new double[] { 2.0 + Tools.GetBi(color.R, 255, color.B), color.R / 255.0 };
                }
                else if (colororder[0] == 3 && colororder[1] == 1)
                {
                    //return 4.0 + color.R / 255.0;
                    return new double[] { 4.0 + Tools.GetBi(color.G, 255, color.R), color.G / 255.0 };
                }
                else
                {
                    //return 4.0 - color.G / 255.0;
                    return new double[] { 4.0 - Tools.GetBi(color.R, 255, color.G), color.R / 255.0 };
                }
            }
        }
        public static Color GetMediumColor(Color color1, Color color2, double value)
        {
            return Color.FromArgb(
                 Convert.ToByte(color1.A + (color2.A - color1.A) * value),
                Convert.ToByte(color1.R + (color2.R - color1.R) * value),
                Convert.ToByte(color1.G + (color2.G - color1.G) * value),
                Convert.ToByte(color1.B + (color2.B - color1.B) * value));
        }
        public static Color GetMaxColor(Color color, out double bi)
        {
            if (color.R == 0 && color.G == 0 && color.B == 0)
            {
                bi = 0.0;
                return Colors.White;
            }
            else
            {
                int[] ti = ColorP.GetColorOrder(color);
                bi = 0.0;
                if (ti[0] == 1)
                {
                    bi = color.R / 255.0;
                }
                else if (ti[0] == 2)
                {
                    bi = color.G / 255.0;
                }
                else
                {
                    bi = color.B / 255.0;
                }
                //Console.WriteLine(color.R / bi);
                //Console.WriteLine(color.G / bi);
                //Console.WriteLine(color.B / bi);
                return Color.FromRgb(Convert.ToByte(color.R / bi), Convert.ToByte(color.G / bi), Convert.ToByte(color.B / bi));
            }
        }
        public static int[] GetColorOrder(Color color)
        {
            if (color.R >= color.G)
            {
                if (color.G >= color.B)
                {
                    //R>=G>=B
                    return new int[] { 1, 2 };
                }
                else
                {
                    if (color.R >= color.B)
                    {
                        //R>=G,G<B,R>=B
                        return new int[] { 1, 3 };
                    }
                    else
                    {
                        //R>=G,G<B,R<B
                        return new int[] { 3, 1 };
                    }
                }
            }
            else
            {
                if (color.G < color.B)
                {
                    //R<G<B
                    return new int[] { 3, 2 };
                }
                else
                {
                    if (color.R >= color.B)
                    {
                        //R<G,G>=B,R>=B
                        return new int[] { 2, 1 };
                    }
                    else
                    {
                        //R<G,G=>B,R<B
                        return new int[] { 2, 3 };
                    }
                }
            }
        }

        public static explicit operator ColorP(Color color)
        {
            return new ColorP(color);
        }
        public static explicit operator Color(ColorP colorP)
        {
            return colorP.GetColor();
        }

        IUSettingsConvertArray IUSettingsConvertArray.USettingsConvertArray(object[] contents)
        {
            return new ColorP((Point)contents[0], (byte)contents[1], (byte)contents[2]);
        }
        object[] IUSettingsConvertArray.USettingsConvertArray()
        {
            return new object[]
            {
                Location,Light,Alpha
            };
        }
    }
}