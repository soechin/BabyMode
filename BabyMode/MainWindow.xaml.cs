using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
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
        private bool _locked;
        private bool _shift;
        private IntPtr _handle;
        private IntPtr _hookPtr;
        private User32.HookProc _hookProc;
        private Random _random;
        private SpeechSynthesizer _speech;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _locked = false;
            _shift = false;
            _handle = new WindowInteropHelper(this).Handle;
            _hookProc = new User32.HookProc(Window_HookProc);
            _hookPtr = User32.SetWindowsHookEx(13/*WH_KEYBOARD_LL*/, _hookProc,
                    User32.GetModuleHandle(null), 0);
            _random = new Random();
            _speech = new SpeechSynthesizer();

            Left = SystemParameters.WorkArea.Right - Width;
            Top = SystemParameters.WorkArea.Bottom - Height;
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
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            MouseClip(_locked && !_shift);
        }

        public IntPtr Window_HookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            bool skip = false;

            if (wParam == (IntPtr)0x0100/*WM_KEYDOWN*/)
            {
                int ascii = User32.KeybdGetAscii(lParam, out int vkcode);

                if (vkcode == 0x70/*VK_F1*/)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        Lock();
                    }));
                }
                else if (!_locked)
                {
                    skip = true;
                }
                else if (vkcode == 0xa0/*VK_LSHIFT*/ || vkcode == 0xa1/*VK_RSHIFT*/)
                {
                    _shift = true;
                }
                else if (ascii != 0)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        KeybdInput((char)ascii);
                    }));
                }
            }
            else if (wParam == (IntPtr)0x0101/*WM_KEYUP*/)
            {
                int ascii = User32.KeybdGetAscii(lParam, out int vkcode);

                if (!_locked)
                {
                    skip = true;
                }
                else if (vkcode == 0xa0/*VK_LSHIFT*/ || vkcode == 0xa1/*VK_RSHIFT*/)
                {
                    _shift = false;
                }
            }
            else if (!_locked)
            {
                skip = true;
            }

            if (nCode < 0 || skip)
            {
                return User32.CallNextHookEx(_hookPtr, nCode, wParam, lParam);
            }

            return (IntPtr)1;
        }

        private void Lock()
        {
            string text = string.Empty;

            MouseClip(_locked = true);

            for (int i = 0; i < 4; i++)
            {
                text += _random.Next(0, 10);
            }

            label.Content = text;
        }

        public void Unlock()
        {
            string text = label.Content as string;

            if (string.IsNullOrEmpty(text))
            {
                MouseClip(_locked = false);
            }
        }

        public void KeybdInput(char c)
        {
            string text = label.Content as string;

            if (!_locked)
            {
                return;
            }

            if (_speech != null)
            {
                _speech.SpeakAsyncCancelAll();
                _speech.SpeakAsync(c.ToString());
            }

            if (!string.IsNullOrEmpty(text))
            {
                if (text[0] == c)
                {
                    label.Content = text.Substring(1);
                    Unlock();
                }
                else
                {
                    Lock();
                }
            }
        }

        public void MouseClip(bool clip)
        {
            if (clip)
            {
                if (User32.GetWindowRect(_handle, out User32.RECT rect))
                {
                    rect.Left += 5;
                    rect.Top += 5;
                    rect.Right -= 5;
                    rect.Bottom -= 5;

                    User32.ClipCursor(ref rect);
                }
            }
            else
            {
                User32.ClipCursor(IntPtr.Zero);
            }
        }

        private void ButtonL_Click(object sender, RoutedEventArgs e)
        {
            Lock();
        }

        private void ButtonX_Click(object sender, RoutedEventArgs e)
        {
            if (!_locked)
            {
                Close();
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            KeybdInput('1');
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            KeybdInput('2');
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            KeybdInput('3');
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            KeybdInput('4');
        }

        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            KeybdInput('5');
        }

        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            KeybdInput('6');
        }

        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            KeybdInput('7');
        }

        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            KeybdInput('8');
        }

        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            KeybdInput('9');
        }

        private void Button0_Click(object sender, RoutedEventArgs e)
        {
            KeybdInput('0');
        }
    }
}
