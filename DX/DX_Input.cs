using SharpDX;
using System;
using System.Diagnostics;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Input;

namespace DX
{
    public enum Manipulation
    {
        Started,Delta,Compeleted
    }
    /// <summary>
    /// main thread
    /// </summary>
    public class DX_Input
    {
        public static Vector2 Position;
        public static Vector2 Motion;
        public static float VelocitiesX { get; private set; }
        public static float VelocitiesY { get; private set; }
        public static bool IsMoved { get; private set; }
        public static int MouseWheelDelta { get; private set; }
        public static bool IsBarrelButtonPressed { get; private set; }
        public static bool IsCanceled { get; private set; }
        public static bool IsEraser { get; private set; }
        public static bool IsHorizontalMouseWheel { get; private set; }
        public static bool IsInRange { get; private set; }
        public static bool IsInverted { get; private set; }
        public static bool IsLeftButtonPressed { get; private set; }
        public static bool IsMiddleButtonPressed { get; private set; }
        public static bool IsPrimary { get; private set; }
        public static bool IsRightButtonPressed { get; private set; }
        public static bool IsXButton1Pressed { get; private set; }
        public static bool IsXButton2Pressed { get; private set; }
        public static PointerUpdateKind PointerUpdateKind { get; private set; }
        public static bool TouchConfidence { get; private set; }
        public static bool IsInContact { get; private set; }
        public static uint FrameId { get; private set; }
        public static bool PointerHandled { get; set; }
        public static long EventTicks { get; private set; }
        public static int EventTime { get; private set; }
        public static int TimeSlice { get; private set; }
        public static void CopyPointer(PointerPoint p)
        {
            EventTicks = DateTime.Now.Ticks;
            int t = DateTime.Now.Millisecond;
            float s = t - EventTime;
            if (s < 0)
                s += 1000;
            EventTime = t;
            FrameId = p.FrameId;
            IsInContact = p.IsInContact;
            IsMoved = false;
            float x = (float) p.Position.X;
            Motion.X = x - Position.X;
            if (s == 0)
                s = TimeSlice;
            if (Motion.X!=0)
            {
                IsMoved = true;
                VelocitiesX = (x - Position.X) / s;
                Position.X = x;
            }
            else VelocitiesX = 0;
            float y = (float) p.Position.Y;
            Motion.Y = y - Position.Y;
            if (Motion.Y!=0)
            {
                IsMoved = true;
                VelocitiesY = (y - Position.Y) / s;
                Position.Y = y;
            }
            else VelocitiesY = 0;
            TimeSlice = (int)s;
            var pp = p.Properties;
            IsBarrelButtonPressed = pp.IsBarrelButtonPressed;
            IsCanceled = pp.IsCanceled;
            IsEraser = pp.IsEraser;
            IsHorizontalMouseWheel = pp.IsHorizontalMouseWheel;
            IsInRange = pp.IsInRange;
            IsInverted = pp.IsInverted;
            IsLeftButtonPressed = pp.IsLeftButtonPressed;
            IsMiddleButtonPressed = pp.IsMiddleButtonPressed;
            IsRightButtonPressed = pp.IsRightButtonPressed;
            IsPrimary = pp.IsPrimary;
            IsXButton1Pressed = pp.IsXButton1Pressed;
            IsXButton2Pressed = pp.IsXButton2Pressed;
            PointerUpdateKind = pp.PointerUpdateKind;
            TouchConfidence = pp.TouchConfidence;
            MouseWheelDelta = pp.MouseWheelDelta/2;
            PointerHandled = false;
        }

        public static VirtualKey Key { get; private set; }
        public static VirtualKey OriginalKey { get; private set; }
        public static string DeviceId { get; private set; }
        public static CorePhysicalKeyStatus KeyStatus;
        public static void CopyKey(KeyRoutedEventArgs e)
        {
            DeviceId = e.DeviceId;
            Key = e.Key;
            OriginalKey = e.OriginalKey;
            KeyStatus = e.KeyStatus;
        }
    }
}
