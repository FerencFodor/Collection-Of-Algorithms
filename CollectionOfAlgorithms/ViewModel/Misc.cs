using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPF_App1.ViewModel
{
    public static class Misc
    {
        public static void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        public static void SetValidValue(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                ((TextBox)sender).Text = "0";
            }
            else if(((TextBox)sender).Text.Length > 1 && ((TextBox)sender).Text.StartsWith("0"))
            {
                ((TextBox)sender).Text = ((TextBox)sender).Text.Substring(1);
            }
        }

        public static void OnLostFocus(ref TextBox box, int value, int max)
        {
            value = value == 0 ? 1 : value;
            
            value = Math.Min(value, max);

            var lo = CheckLowerDivider(max, value);
            var hi = CheckHigherDivider(max, value);
            
            
            if (lo != value || hi != value)
                value = value - lo < hi - value ? lo : hi;
            

            box.Text = value.ToString();
        }

        private static int CheckHigherDivider(int numerator, int divider)
        {
            var p = (float)numerator / divider;

            while (p - Math.Floor(p) > 0f && divider <= numerator)
            {
                divider++;
                p = (float)numerator / divider;
            }

            return divider;
        }

        private static int CheckLowerDivider(int numerator, int divider)
        {
            var p = (float)numerator / divider;

            while (p - Math.Floor(p) > 0f && divider > 0)
            {
                divider--;
                p = (float)numerator / divider;
            }

            return divider;
        }

        public static int RandomWeightedValue(int[] probabilities, Random random)
        {
            var r = random.Next(probabilities.Sum());

            for (var i = 0; i < probabilities.Length; i++)
            {
                r -= probabilities[i];
                if (r <= 0)
                {
                    return i;
                }
            }

            return 0;
        } 
    }
}