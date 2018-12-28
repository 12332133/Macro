using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.CombinationKey
{
    public static class KeyboardHooker
    {
        public static Action<RawKey> OnKeyDown;
        public static Action<RawKey> OnKeyUp;

        private static IntPtr hookHandle = IntPtr.Zero;
        private static HashSet<RawKey> pressedKeys = new HashSet<RawKey>();
        private static bool interceptMessages { get; set; }

        public static bool IsRunning()
        {
            return hookHandle != IntPtr.Zero;
        }

        public static bool Start()
        {
            if (hookHandle != IntPtr.Zero) 
                return false;
            return SetHook();
        }

        public static void Stop()
        {
            RemoveHook();
            pressedKeys.Clear();
        }

        public static bool IsKeyDown(RawKey key)
        {
            return pressedKeys.Contains(key);
        }

        private static bool SetHook()
        {
            if (hookHandle == IntPtr.Zero)
                hookHandle = Win32API.SetWindowsHookEx(
                                    HookType.WH_KEYBOARD_LL, 
                                    HandleLowLevelHookProc, IntPtr.Zero, 0);

            if (hookHandle == IntPtr.Zero)
                return false;

            return true;
        }

        private static void RemoveHook()
        {
            if (hookHandle != IntPtr.Zero)
            {
                Win32API.UnhookWindowsHookEx(hookHandle);
                hookHandle = IntPtr.Zero;
            }
        }

        private static int HandleLowLevelHookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
                return Win32API.CallNextHookEx(hookHandle, code, wParam, lParam);

            var kdbHookStruct = KBDLLHOOKSTRUCT.Create(lParam);
            var state = (RawKeyState)wParam;
            var key = (RawKey)kdbHookStruct.vkCode;

            if (state == RawKeyState.KeyDown || state == RawKeyState.SysKeyDown)
                HandleKeyDown(key);
            else
                HandleKeyUp(key);

            return interceptMessages ? 1 : Win32API.CallNextHookEx(hookHandle, 0, wParam, lParam);
        }

        private static void HandleKeyDown(RawKey key)
        {
            var added = pressedKeys.Add(key);
            if (added && OnKeyDown != null)
                OnKeyDown.Invoke(key);
        }

        private static void HandleKeyUp(RawKey key)
        {
            pressedKeys.Remove(key);
            if (OnKeyUp != null)
                OnKeyUp.Invoke(key);
        }
    }
}
