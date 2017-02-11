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

            int dpm = 8192;
            float imageWidth = 0.05f;
            float imageHeight = 0.05f;
            Size imageSizeP = new Size((int)(imageWidth * dpm), (int)(imageHeight * dpm));

            float scaleDistance = 0.5f;
            float eyeBase = 0.07f;

            float minDistance = 1.0f;
            float maxDistance = 10.0f;
            float step = 1.0f;

            float heightProjectionOffset = -1.0f;
            float projectionHeight = 1.9f;

            Bitmap bmpRight = new Bitmap(imageSizeP.Width, imageSizeP.Height, PixelFormat.Format32bppPArgb);
            Bitmap bmpLeft = new Bitmap(imageSizeP.Width, imageSizeP.Height, PixelFormat.Format32bppPArgb);

            Brush backBrush = new SolidBrush(Color.FromArgb(255, Color.Black));
            Brush foreBrush = new SolidBrush(Color.White);


            using (Graphics grpRight = Graphics.FromImage(bmpRight))
            {
                using (Graphics grpLeft = Graphics.FromImage(bmpLeft))
                {
                    grpRight.SmoothingMode = SmoothingMode.HighQuality;
                    grpRight.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                    grpRight.CompositingQuality = CompositingQuality.HighQuality;
                    grpRight.InterpolationMode = InterpolationMode.High;

                    grpLeft.SmoothingMode = SmoothingMode.HighQuality;
                    grpLeft.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                    grpLeft.CompositingQuality = CompositingQuality.HighQuality;
                    grpLeft.InterpolationMode = InterpolationMode.High;

                    for (float dist = minDistance; dist <= maxDistance; dist += step)
                    {
                        float t = dist / maxDistance;

                        float markProjY = heightProjectionOffset + t * projectionHeight;
                        float markImageY = (1.0f - 0.5f * (markProjY + 1.0f)) * imageSizeP.Height;

                        float scaleValue = GetScaleValue(dist, scaleDistance, eyeBase);
                        float scaleValueP = ToFPixels(scaleValue, dpm);

                        PointF leftMarkPos = new PointF(0.5f * imageSizeP.Width + scaleValueP, markImageY);
                        PointF rightMarkPos = new PointF(0.5f * imageSizeP.Width - scaleValueP, markImageY);

                        DrawMark(leftMarkPos, 3.0f, foreBrush, backBrush, grpLeft);
                        DrawLabel(leftMarkPos, dist.ToString("0"), 12.0f, foreBrush, backBrush, grpLeft);

                        DrawMark(rightMarkPos, 3.0f, foreBrush, backBrush, grpRight);
                        DrawLabel(rightMarkPos, dist.ToString("0"), 12.0f, foreBrush, backBrush, grpRight);
                    }
                }
            }

            bmpLeft.Save(leftImageFilePath, ImageFormat.Png);
            bmpRight.Save(rightImageFilePath, ImageFormat.Png);
        }

        static void DrawMark(PointF coords, float size, Brush foreBrush, Brush backBrush, Graphics graphics)
        {
            SizeF sizeF = new SizeF(size, size);
            SizeF halfSizeF = new SizeF(0.5f * size, 0.5f * size);

            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(new RectangleF(coords - halfSizeF, sizeF));

            Pen pen = new Pen(backBrush, 3.0f);
            graphics.DrawPath(pen, path);
            graphics.FillPath(foreBrush, path);
        }

        static void DrawLabel(PointF coords, string label, float size, Brush foreBrush, Brush backBrush, Graphics graphics)
        {

            Font font = new Font(FontFamily.GenericSansSerif, size, FontStyle.Regular, GraphicsUnit.Pixel);

            SizeF sizeF = new SizeF(size, size);
            SizeF halfSizeF = new SizeF(0.5f * size, 0.5f * size);

            PointF labelcoords = new PointF(coords.X + 4f, coords.Y - 0.6f * size);

            GraphicsPath path = new GraphicsPath();
            path.AddString(label, FontFamily.GenericSansSerif, (int)FontStyle.Regular, size, labelcoords, StringFormat.GenericDefault);

            Pen pen = new Pen(backBrush, 3.0f);
            graphics.DrawPath(pen, path);
            graphics.FillPath(foreBrush, path);
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
