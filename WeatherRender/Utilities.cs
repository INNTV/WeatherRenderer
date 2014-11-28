using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;

namespace WeatherRender
{
    class Utilities
    {
        public static void Txt2Img(TxtObject Obj)
        {
            try
            {
                Bitmap bmp = new Bitmap(Obj.ImgWidth, Obj.ImgHeight);
                Graphics g = Graphics.FromImage(bmp);

                StringFormat sf = new StringFormat();
                // sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                Font f = new Font(Obj.FntName, Obj.FntSize, Obj.FntStyle, GraphicsUnit.Point);

                Pen p = new Pen(ColorTranslator.FromHtml(Obj.ForeColorCode), Obj.BrushWidth);
                p.LineJoin = LineJoin.Round;
                Rectangle fr = new Rectangle(0, 0, bmp.Width, f.Height);
                LinearGradientBrush b = new LinearGradientBrush(fr,
                                                                ColorTranslator.FromHtml(Obj.BrushGradientStartColor),
                                                                ColorTranslator.FromHtml(Obj.BrushGradientEndColor),
                                                                Obj.GradientAngel);
                Rectangle r = new Rectangle(Obj.MarginX, Obj.MarginY, bmp.Width, bmp.Height);

                GraphicsPath gp = new GraphicsPath();
                gp.AddString(Obj.Text,
                             f.FontFamily, (int)f.Style, Obj.FntSize, r, sf);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;


                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.TextContrast = 0;

                g.DrawPath(p, gp);
                g.FillPath(b, gp);


                bmp.Save(Obj.CompleteFileName, Obj.ImgFormat);
                bmp.Dispose();
                gp.Dispose();
                b.Dispose();
                f.Dispose();
                sf.Dispose();
                g.Dispose();
            }
            catch (Exception Exp)
            {
                System.Windows.Forms.MessageBox.Show(Exp.Message);
            }
            
        }
        public static StreamReader Renderer(RenderObject Obj)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "\"" + Obj.AeRenderPath + "\"";
           
            DirectoryInfo Dir = new DirectoryInfo(Obj.DestDirectory);
            if (!Dir.Exists)
            {
                Dir.Create();
            }


            proc.StartInfo.Arguments = " -project " + "\"" + Obj.AeProjectPath + "\"" + "   -comp   \"" + Obj.CompositionName + "\" -output " + "\"" + Obj.DestFullFileName+ "\"";
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.EnableRaisingEvents = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;

            if (!proc.Start())
            {
                return null;
            }

            StreamReader reader = proc.StandardOutput;
            return reader;
          
        }

    }
   public  class TxtObject
    {
        public float FntSize { get; set; }
        public string FntName { get; set; }
        public FontStyle FntStyle { get; set; }
        public string ForeColorCode { get; set; }
        public float BrushWidth { get; set; }
        public string BrushGradientStartColor { get; set; }
        public string BrushGradientEndColor { get; set; }
        public float GradientAngel { get; set; }
        public int MarginX { get; set; }
        public int MarginY { get; set; }
        public string CompleteFileName { get; set; }
        public System.Drawing.Imaging.ImageFormat ImgFormat { get; set; }
        public int ImgWidth { get; set; }
        public int ImgHeight { get; set; }
        public string Text { get; set; }
    }
   public  class RenderObject
    {
        public string AeRenderPath { get; set; }
        public string DestDirectory { get; set; }
        public string DestFullFileName { get; set; }
        public string AeProjectPath { get; set; }
        public string CompositionName { get; set; }
    }
}
