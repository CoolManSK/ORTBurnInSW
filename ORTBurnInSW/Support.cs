using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PowerOne.Support
{
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form();
            prompt.Width = 260;
            prompt.Height = 150;
            prompt.Text = caption;
            prompt.StartPosition = FormStartPosition.CenterScreen;
            prompt.MinimizeBox = false;
            prompt.MaximizeBox = false;
            prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
            Label textLabel = new Label() { Left = 25, Top = 25, Text = text };
            TextBox textBox = new TextBox() { Left = 25, Top = 50, Width = 200 };
            //textBox.RightToLeft = RightToLeft.Yes;
            textBox.PasswordChar = '*';
            Button confirmation = new Button() { Text = "OK", Left = 150, Width = 75, Top = 85 };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            prompt.ShowDialog();
            return textBox.Text;
        }
    }

    public class ErrorQueue
    {
        public string Message = String.Empty;
        public bool Critical = false;
    }

    public static class Functions
    {
        public static double AdjustValue(double value, double lowLimit, double highLimit)
        {
            if (value < lowLimit) value = lowLimit;
            if (value > highLimit) value = highLimit;
            return value;
        }

        public static double TrimDownValue(double value, double level)
        {
            if (value < level) value = 0;
            return value;
        }

        public static bool EvaluateValue(double value, double lowLimit, double highLimit)
        {
            if ((value >= lowLimit) && (value <= highLimit)) return true;
            else return false;
        }

        public static string ComputeTimeString(int ticks)
        {
            return ((int)(ticks / 3600)).ToString("00") + "h:" + ((int)(ticks / 60) % 60).ToString("00") + "m:" + (ticks % 60).ToString("00") + "s";
        }
    }

    public class TestVarDouble
    {
        public double value = 0;
        public int failCount = 0;
        public int failCountMax = 0;
    }

    public class TestVarBool
    {
        public bool value = false;
        public int failCount = 0;
        public int failCountMax = 0;
    }

    public enum TestState
    {
        Standby,
        PreTest,
        MainTest,
        PostTest,
    };
}
