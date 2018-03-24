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
        private IntPtr _hookPtr;
        private User32.HookProc _hookProc;
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
            _hookProc = new User32.HookProc(Window_HookProc);
            _hookPtr = IntPtr.Zero;
            _random = new Random();
            _speech = new SpeechSynthesizer();
            _timer = null;

            Left = SystemParameters.WorkArea.Right - Width;
            Top = SystemParameters.WorkArea.Bottom - Height;
        }

        public IntPtr Window_HookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (wParam == (IntPtr)0x0100/*WM_KEYDOWN*/)
            {
                char c = User32.KeybdStructToAscii(lParam);

                if (c != '\0')
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        Unlock(c);
                    }));
                }
            }

            if (nCode < 0)
            {
                return User32.CallNextHookEx(_hookPtr, nCode, wParam, lParam);
            }

            return (IntPtr)1;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_hookPtr != IntPtr.Zero)
            {
                User32.UnhookWindowsHookEx(_hookPtr);
                _hookPtr = IntPtr.Zero;
            }

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

        private void Timer_Elapsed(object state)
        {
            if (User32.GetWindowRect(_handle, out User32.RECT rect))
            {
                User32.ClipCursor(ref rect);
            }
        }

        private void Lock()
        {
            string text = string.Empty;

            if (_hookPtr == IntPtr.Zero)
            {
                _hookPtr = User32.SetWindowsHookEx(13/*WH_KEYBOARD_LL*/, _hookProc,
                    User32.GetModuleHandle(null), 0);
            }

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
                if (_hookPtr != IntPtr.Zero)
                {
                    User32.UnhookWindowsHookEx(_hookPtr);
                    _hookPtr = IntPtr.Zero;
                }

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
