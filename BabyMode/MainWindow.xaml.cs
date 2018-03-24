using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BabyMode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr _handle;
        private Random _random;
        private SpeechSynthesizer _speech;
        private Timer _timer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _handle = new WindowInteropHelper(this).Handle;
            _random = new Random();
            _speech = new SpeechSynthesizer();
            _timer = null;

            Left = SystemParameters.WorkArea.Right - Width;
            Top = SystemParameters.WorkArea.Bottom - Height;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_speech != null)
            {
                _speech.Dispose();
                _speech = null;
            }

            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {
            Button button;
            Point point;

            foreach (char c in e.Text)
            {
                if (c == '1') button = button1;
                else if (c == '2') button = button2;
                else if (c == '3') button = button3;
                else if (c == '4') button = button4;
                else if (c == '5') button = button5;
                else if (c == '6') button = button6;
                else if (c == '7') button = button7;
                else if (c == '8') button = button8;
                else if (c == '9') button = button9;
                else if (c == '0') button = button0;
                else button = null;

                if (IsLocked() && button != null)
                {
                    point = new Point(button.Width / 2, button.Height / 2);
                    point = button.PointToScreen(point);
                    User32.SetCursorPos((int)point.X, (int)point.Y);
                }

                Unlock(c);
            }
        }

        private void Timer_Elapsed(object state)
        {
            if (User32.GetWindowRect(_handle, out User32.RECT rect))
            {
                User32.ClipCursor(ref rect);
                User32.SetForegroundWindow(_handle);
            }
        }

        private void Lock()
        {
            string text = string.Empty;

            if (_timer == null)
            {
                _timer = new Timer(Timer_Elapsed, null, 0, 1);
            }

            for (int i = 0; i < 4; i++)
            {
                text += _random.Next(0, 10);
            }

            label.Content = text;
        }

        public void Unlock(char c)
        {
            string text = label.Content as string;

            if (IsLocked())
            {
                if (_speech != null)
                {
                    _speech.SpeakAsyncCancelAll();
                    _speech.SpeakAsync(c.ToString());
                }

                if (text[0] != c)
                {
                    Lock();
                    return;
                }

                label.Content = text.Substring(1);
            }

            if (!IsLocked())
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;

                    User32.ClipCursor(IntPtr.Zero);
                }
            }
        }

        public bool IsLocked()
        {
            string text = label.Content as string;
            return !string.IsNullOrEmpty(text);
        }

        private void ButtonL_Click(object sender, RoutedEventArgs e)
        {
            Lock();
        }

        private void ButtonX_Click(object sender, RoutedEventArgs e)
        {
            string text = label.Content as string;

            if (string.IsNullOrEmpty(text))
            {
                Close();
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            Unlock('1');
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            Unlock('2');
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            Unlock('3');
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            Unlock('4');
        }

        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            Unlock('5');
        }

        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            Unlock('6');
        }

        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            Unlock('7');
        }

        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            Unlock('8');
        }

        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            Unlock('9');
        }

        private void Button0_Click(object sender, RoutedEventArgs e)
        {
            Unlock('0');
        }
    }
}
