using System;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace WPF_App1
{
    public class ColorConverter
    {
        public static Color ColorFromHsl(float h, float s, float l)
        {
            float r = 0, g = 0, b = 0;
            
            if (l == 0) 
                return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
            
            if (s == 0)
                r = g = b = l;
            else
            {
                float colorComp2;
                if (l < 0.5)
                    colorComp2 = l * (1f + s);
                else
                    colorComp2 = l + s - (l * s);

                var colorComp1 = 2f * l - colorComp2;

                r = GetColorComponent(colorComp1, colorComp2, h + 1f / 3f);
                g = GetColorComponent(colorComp1, colorComp2, h );
                b = GetColorComponent(colorComp1, colorComp2, h - 1f / 3f);
            }

            return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
        }

        private static float GetColorComponent(float colorComp1, float colorComp2, float colorComp3)
        {
            switch (colorComp3)
            {
                case < 0f:
                    colorComp3 += 1f;
                    break;
                case > 1f:
                    colorComp3 -= 1f;
                    break;
            }

            return colorComp3 switch
            {
                < 1f / 6f => colorComp1 + (colorComp2 - colorComp1) * 6f * colorComp3,
                < 0.5f => colorComp2,
                < 2f / 3f => colorComp1 + ((colorComp2 - colorComp1) * ((2f / 3f) - colorComp3) * 6f),
                _ => colorComp1
            };
        }
    }
}