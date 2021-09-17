using SkiaSharp;
using System;
using System.Linq;

namespace Xz.Node.Framework.Common
{
    public class SkiaSharpHlper
    {
        /// <summary>
        /// 基于SkiaSharp动态生成验证码
        /// </summary>
        public static (byte[], string) GetCaptcha()
        {
            #region 反射SK支持的全部颜色
            //List<SKColor> colors = new List<SKColor>();
            //var skcolors = new SKColors();
            //var type = skcolors.GetType();
            //foreach (FieldInfo field in type.GetFields())
            //{
            //    colors.Add( (SKColor)field.GetValue(skcolors));
            //}
            #endregion

            //int maxcolorindex = colors.Count-1;
            string text = GenerateRandomNumber();
            var zu = text.ToList();
            SKBitmap bmp = new SKBitmap(80, 30);
            using (SKCanvas canvas = new SKCanvas(bmp))
            {
                //背景色
                canvas.DrawColor(SKColors.White);

                using (SKPaint sKPaint = new SKPaint())
                {
                    sKPaint.TextSize = 16;//字体大小
                    sKPaint.IsAntialias = true;//开启抗锯齿
                    sKPaint.Typeface = SKTypeface.FromFamilyName("SimSun", (int)SKFontStyleWeight.Normal, (int)SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);//字体
                    SKRect size = new SKRect();
                    sKPaint.MeasureText(zu[0].ToString(), ref size);//计算文字宽度以及高度

                    float temp = (bmp.Width / 4 - size.Size.Width) / 2;
                    float temp1 = bmp.Height - (bmp.Height - size.Size.Height) / 2;
                    Random random = new Random();

                    for (int i = 0; i < zu.Count; i++)
                    {

                        sKPaint.Color = new SKColor((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
                        canvas.DrawText(zu[i].ToString(), temp + 20 * i, temp1, sKPaint);//画文字
                    }
                    //干扰线
                    for (int i = 0; i < 5; i++)
                    {
                        sKPaint.Color = new SKColor((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
                        canvas.DrawLine(random.Next(0, 80), random.Next(1, 29), random.Next(41, 80), random.Next(1, 29), sKPaint);
                    }
                }
                //页面展示图片
                using (SKImage img = SKImage.FromBitmap(bmp))
                {
                    using (SKData p = img.Encode())
                    {
                        return (p.ToArray(), text);
                    }
                }
            }
        }


        private static char[] constant =
        {
            '0','1','2','3','4','5','6','7','8','9',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
        };
        /// <summary>
        /// 生成随机码
        /// </summary>
        /// <param name="Length"></param>
        /// <returns></returns>
        private static string GenerateRandomNumber(int Length = 4)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(62)]);
            }
            return newRandom.ToString();
        }
    }
}
