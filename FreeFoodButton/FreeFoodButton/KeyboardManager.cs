using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace FreeFoodButton
{
    sealed class KeyboardManager
    {
        #region Private Members

        private delegate IntPtr HookHandlerDelegate(int nCode, IntPtr wParam, ref KBHookStruct lParam);
        private static HookHandlerDelegate callbackPtr;
        private static IntPtr hookPtr = IntPtr.Zero;
        private const int LowLevelKeyboardHook = 13;
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookHandlerDelegate callbackPtr, IntPtr hInstance, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KBHookStruct lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        #endregion

        public KeyboardManager() { }

        public void DisableSystemKeys()
        {
            if (callbackPtr == null)
            {
                callbackPtr = new HookHandlerDelegate(KeyboardHookHandler);
            }

            if (hookPtr == IntPtr.Zero)
            {
                // Note: This does not work in the VS host environment.  To run in debug mode:
                // Project -> Properties -> Debug -> Uncheck "Enable the Visual Studio hosting process"
                IntPtr hInstance = Marshal.GetHINSTANCE(Application.Current.GetType().Module);
                hookPtr = SetWindowsHookEx(LowLevelKeyboardHook, callbackPtr, hInstance, 0);
            }
        }

        public void EnableSystemKeys()
        {
            if (hookPtr != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookPtr);
                hookPtr = IntPtr.Zero;
            }
        }

        public event EventHandler KeyPressed;

        public void OnKeyPressed(EventArgs e)
        {
            EventHandler handler = KeyPressed;
            if (handler != null) handler(null, e);
        }

        private IntPtr KeyboardHookHandler(int nCode, IntPtr wParam, ref KBHookStruct lParam)
        {
            if (nCode == 0)
            {
                if (
                ((lParam.vkCode == 0x5B) && (lParam.flags == 0x01)) ||      // Left Windows Key
                ((lParam.vkCode == 0x5C) && (lParam.flags == 0x01))       // Right Windows Key
                )      
                {
                    //TODO add link
                    
                    Email.SendEmail(Settings.Email, "Free Food", Settings.Message);
                    OnKeyPressed(new EventArgs());
                    return new IntPtr(1);
                }
            }

            return CallNextHookEx(hookPtr, nCode, wParam, ref lParam);
        }

        public static void ActivateWindow(Window window)
        {
            var interopHelper = new WindowInteropHelper(window);
            var currentForegroundWindow = GetForegroundWindow();
            var thisWindowThreadId = GetWindowThreadProcessId(interopHelper.Handle, IntPtr.Zero);
            var currentForegroundWindowThreadId = GetWindowThreadProcessId(currentForegroundWindow, IntPtr.Zero);
            AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, true);
            SetWindowPos(interopHelper.Handle, new IntPtr(0), 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW);
            AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, false);
            window.Show();
            window.Activate();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KBHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
    }
}
