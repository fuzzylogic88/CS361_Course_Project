﻿/*
 * HealthApp - Daniel Green (greend5@oregonstate.edu)
 * CS361, Spring 2025
 * 
 * An extended MessageBox
 */

using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

public partial class MessageBoxEx
{
    private static IntPtr _owner;
    private static HookProc _hookProc;
    private static IntPtr _hHook;

    public static MessageBoxResult Show(string text)
    {
        Initialize();
        return System.Windows.MessageBox.Show(text);
    }

    public static MessageBoxResult Show(string text, string caption)
    {
        Initialize();
        return System.Windows.MessageBox.Show(text, caption);
    }

    public static MessageBoxResult Show(string text, string caption, MessageBoxButton buttons)
    {
        Initialize();
        return System.Windows.MessageBox.Show(text, caption, buttons);
    }

    public static MessageBoxResult Show(string text, string caption, MessageBoxButton buttons, MessageBoxImage icon)
    {
        Initialize();
        return System.Windows.MessageBox.Show(text, caption, buttons, icon);
    }

    public static MessageBoxResult Show(string text, string caption, MessageBoxButton buttons, MessageBoxImage icon, MessageBoxResult defResult)
    {
        Initialize();
        return System.Windows.MessageBox.Show(text, caption, buttons, icon, defResult);
    }

    public static MessageBoxResult Show(string text, string caption, MessageBoxButton buttons, MessageBoxImage icon, MessageBoxResult defResult, System.Windows.MessageBoxOptions options)
    {
        Initialize();
        return System.Windows.MessageBox.Show(text, caption, buttons, icon, defResult, options);
    }

    public static MessageBoxResult Show(Window owner, string text)
    {
        _owner = new WindowInteropHelper(owner).Handle;
        Initialize();
        return System.Windows.MessageBox.Show(owner, text);
    }

    public static MessageBoxResult Show(Window owner, string text, string caption)
    {
        _owner = new WindowInteropHelper(owner).Handle;
        Initialize();
        return System.Windows.MessageBox.Show(owner, text, caption);
    }

    public static MessageBoxResult Show(Window owner, string text, string caption, MessageBoxButton buttons)
    {
        _owner = new WindowInteropHelper(owner).Handle;
        Initialize();
        return System.Windows.MessageBox.Show(owner, text, caption, buttons);
    }

    public static MessageBoxResult Show(Window owner, string text, string caption, MessageBoxButton buttons, MessageBoxImage icon)
    {
        _owner = new WindowInteropHelper(owner).Handle;
        Initialize();
        return System.Windows.MessageBox.Show(owner, text, caption, buttons, icon);
    }

    public static MessageBoxResult Show(Window owner, string text, string caption, MessageBoxButton buttons, MessageBoxImage icon, MessageBoxResult defResult)
    {
        _owner = new WindowInteropHelper(owner).Handle;
        Initialize();
        return System.Windows.MessageBox.Show(owner, text, caption, buttons, icon, defResult);
    }

    public static MessageBoxResult Show(Window owner, string text, string caption, MessageBoxButton buttons, MessageBoxImage icon, MessageBoxResult defResult, System.Windows.MessageBoxOptions options)
    {
        _owner = new WindowInteropHelper(owner).Handle;
        Initialize();
        return System.Windows.MessageBox.Show(owner, text, caption, buttons, icon,
                                defResult, options);
    }

    private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    public delegate void TimerProc(IntPtr hWnd, uint uMsg, UIntPtr nIDEvent, uint dwTime);

    public const int WH_CALLWNDPROCRET = 12;

    public enum CbtHookAction : int
    {
        HCBT_MOVESIZE = 0,
        HCBT_MINMAX = 1,
        HCBT_QS = 2,
        HCBT_CREATEWND = 3,
        HCBT_DESTROYWND = 4,
        HCBT_ACTIVATE = 5,
        HCBT_CLICKSKIPPED = 6,
        HCBT_KEYSKIPPED = 7,
        HCBT_SYSCOMMAND = 8,
        HCBT_SETFOCUS = 9
    }

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, ref Rectangle lpRect);

    [DllImport("user32.dll")]
    private static extern int MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    [DllImport("User32.dll")]
    public static extern UIntPtr SetTimer(IntPtr hWnd, UIntPtr nIDEvent, uint uElapse, TimerProc lpTimerFunc);

    [DllImport("User32.dll")]
    public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);


    [DllImport("user32.dll")]
    public static extern int UnhookWindowsHookEx(IntPtr idHook);

    [DllImport("user32.dll")]
    public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxLength);

    [DllImport("user32.dll")]
    public static extern int EndDialog(IntPtr hDlg, IntPtr nResult);

    [DllImport("kernel32.dll")]
    public static extern uint GetCurrentThreadId();


    [StructLayout(LayoutKind.Sequential)]
    public struct CWPRETSTRUCT
    {
        public IntPtr lResult;
        public IntPtr lParam;
        public IntPtr wParam;
        public uint message;
        public IntPtr hwnd;
    };

    static MessageBoxEx()
    {
        _hookProc = new HookProc(MessageBoxHookProc);
        _hHook = IntPtr.Zero;
    }

    private static void Initialize()
    {
        if (_hHook != IntPtr.Zero)
        {
            throw new NotSupportedException("multiple calls are not supported");
        }

        if (_owner != null)
        {
            _hHook = SetWindowsHookEx(WH_CALLWNDPROCRET, _hookProc, IntPtr.Zero, (int)GetCurrentThreadId());
        }
    }

    private static IntPtr MessageBoxHookProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode < 0)
        {
            return CallNextHookEx(_hHook, nCode, wParam, lParam);
        }

        CWPRETSTRUCT msg = (CWPRETSTRUCT)Marshal.PtrToStructure(lParam, typeof(CWPRETSTRUCT));
        IntPtr hook = _hHook;

        if (msg.message == (int)CbtHookAction.HCBT_ACTIVATE)
        {
            try
            {
                CenterWindow(msg.hwnd);
            }
            finally
            {
                UnhookWindowsHookEx(_hHook);
                _hHook = IntPtr.Zero;
            }
        }

        return CallNextHookEx(hook, nCode, wParam, lParam);
    }

    private static void CenterWindow(IntPtr hChildWnd)
    {
        Rectangle recChild = new(0, 0, 0, 0);
        bool getrect = GetWindowRect(hChildWnd, ref recChild);

        int width = recChild.Width - recChild.X;
        int height = recChild.Height - recChild.Y;

        Rectangle recParent = new(0, 0, 0, 0);
        getrect = GetWindowRect(_owner, ref recParent);

        System.Drawing.Point ptCenter = new(0, 0)
        {
            X = recParent.X + ((recParent.Width - recParent.X) / 2),
            Y = recParent.Y + ((recParent.Height - recParent.Y) / 2)
        };


        System.Drawing.Point ptStart = new(0, 0)
        {
            X = (ptCenter.X - (width / 2)),
            Y = (ptCenter.Y - (height / 2))
        };

        ptStart.X = (ptStart.X < 0) ? 0 : ptStart.X;
        ptStart.Y = (ptStart.Y < 0) ? 0 : ptStart.Y;

        int m = MoveWindow(hChildWnd, ptStart.X, ptStart.Y, width,
                                height, false);
    }
}