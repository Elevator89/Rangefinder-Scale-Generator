using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace RangefinderScale
{
    class Program
    {
        static void Main(string[] args)
        {
            string leftImageFilePath = args[0];
            string rightImageFilePath = args[1];

            int dpm = 4096;
            float imageWidth = 0.125f;
            float imageHeight = 0.0625f;
            Size imageSizeP = new Size((int)(imageWidth * dpm), (int)(imageHeight * dpm));

            float scaleDistance = 1.0f;
            float eyeBase = 0.07f;

            float minDistance = 1.0f;
            float maxDistance = 10.0f;
            float step = 1.0f;

            float heightProjectionOffset = -1.0f;
            float projectionHeight = 1.9f;

            Bitmap bmpRight = new Bitmap(imageSizeP.Width, imageSizeP.Height, PixelFormat.Format32bppPArgb);
            Bitmap bmpLeft = new Bitmap(imageSizeP.Width, imageSizeP.Height, PixelFormat.Format32bppPArgb);

            Brush backBrush = new SolidBrush(Color.FromArgb(10, Color.Black));
            Brush foreBrush = new SolidBrush(Color.White);


            using (Graphics grpRight = Graphics.FromImage(bmpRight))
            {
                using (Graphics grpLeft = Graphics.FromImage(bmpLeft))
                {
                    grpRight.SmoothingMode = SmoothingMode.HighQuality;
                    grpRight.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                    grpLeft.SmoothingMode = SmoothingMode.HighQuality;
                    grpLeft.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                    for (float dist = minDistance; dist <= maxDistance; dist += step)
                    {
                        float t = dist / maxDistance;

                        float markProjY = heightProjectionOffset + t * projectionHeight;
                        float markImageY = (1.0f - 0.5f * (markProjY + 1.0f)) * imageSizeP.Height;

                        float scaleValue = GetScaleValue(dist, scaleDistance, eyeBase);
                        float scaleValueP = ToFPixels(scaleValue, dpm);

                        PointF leftMarkPos = new PointF(0.5f * imageSizeP.Width - scaleValueP, markImageY);
                        PointF rightMarkPos = new PointF(0.5f * imageSizeP.Width + scaleValueP, markImageY);

                        DrawMark(leftMarkPos, 6.0f, backBrush, grpLeft);
                        DrawMark(leftMarkPos, 3.0f, foreBrush, grpLeft);
                        DrawLabel(leftMarkPos, dist.ToString("0"), 10.0f, foreBrush, backBrush, grpLeft);

                        DrawMark(rightMarkPos, 6.0f, backBrush, grpRight);
                        DrawMark(rightMarkPos, 3.0f, foreBrush, grpRight);
                        DrawLabel(rightMarkPos, dist.ToString("0"), 10.0f, foreBrush, backBrush, grpRight);
                    }
                }
            }

            bmpLeft.Save(leftImageFilePath, ImageFormat.Png);
            bmpRight.Save(rightImageFilePath, ImageFormat.Png);
        }

        static void DrawMark(PointF coords, float size, Brush brush, Graphics graphics)
        {
            SizeF sizeF = new SizeF(size, size);
            SizeF halfSizeF = new SizeF(0.5f * size, 0.5f * size);

            graphics.FillEllipse(brush, new RectangleF(coords - halfSizeF, sizeF));
        }

        static void DrawLabel(PointF coords, string label, float size, Brush foreBrush, Brush backBrush, Graphics graphics)
        {
            //GraphicsPath

            //using (PathGradientBrush brush = new PathGradientBrush(pathShadow))
            //{
            //    ColorBlend blend = new ColorBlend();
            //    blend.Colors = new Color[] { Color.Transparent, Color.Black };
            //    blend.Positions = new float[] { 0.0f, 1.0f };
            //    brush.InterpolationColors = blend;
            //    graph.FillPath(brush, pathShadow);
            //}

            Font font = new Font(FontFamily.GenericSansSerif, size, FontStyle.Regular, GraphicsUnit.Pixel);

            SizeF sizeF = new SizeF(size, size);
            SizeF halfSizeF = new SizeF(0.5f * size, 0.5f * size);

            PointF labelcoords = new PointF(coords.X + 4f, coords.Y - 0.7f * size);

            SizeF labelSize = graphics.MeasureString(label, font, labelcoords, StringFormat.GenericDefault);
            graphics.FillRectangle(backBrush, new RectangleF(labelcoords, labelSize));
            graphics.DrawString(label, font, foreBrush, labelcoords, StringFormat.GenericDefault);
        }

        static float ToFPixels(float meters, int dotsPerMeter)
        {
            return meters * dotsPerMeter;
        }

        static float GetScaleValue(float distance, float scaleDistance, float eyeBase)
        {
            return 0.5f * eyeBase * scaleDistance / distance;
        }
    }
}
