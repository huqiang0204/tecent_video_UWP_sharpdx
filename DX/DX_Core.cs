using System;
using Windows.UI.Xaml.Controls;
using D3D11 = SharpDX.Direct3D11;
using D2D1 = SharpDX.Direct2D1;
using Dxgi = SharpDX.DXGI;
using Wic = SharpDX.WIC;
using SharpDX.Direct3D;
using SharpDX;
using SharpDX.Mathematics.Interop;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml.Media;

namespace DX
{
    public interface IGradientBrush
    {
        void SetRect(RawRectangleF rect);
    }
    public struct LinearData
    {
        public D2D1.LinearGradientBrushProperties Point;
        public D2D1.GradientStop[] data;
    }
    public struct RadialData
    {
        public D2D1.RadialGradientBrushProperties Point;
        public D2D1.GradientStop[] data;
    }

    internal struct SwapChainComponent
    {
        internal float dpi;
        internal SwapChainPanel panel;
        internal Dxgi.ISwapChainPanelNative NativePanel;
        internal Dxgi.SwapChain1 swapChain;
        internal D2D1.Bitmap1 Bitmap;
        internal D3D11.RenderTargetView TargetView;
        internal D3D11.DepthStencilView StencilView;
        internal D3D11.Texture2D backbuffer;
        internal D3D11.Texture2D depthbuffer;
        internal Viewport port;
    }
    public class D2DEnviroment
    {
        public D3D11.Device d3dDevice;
        public D3D11.DeviceContext d3dContext;
        public D2D1.Device d2dDevice;
        public D2D1.DeviceContext d2dContext;
        public D2D1.SolidColorBrush ColorBrush;
        public D2D1.ImageBrush ImgBrush;
    }
    public class DX_Core
    {
        #region Environment
        internal static int dpi;
        internal static FeatureLevel[] featureLevels = new FeatureLevel[]
            {
               FeatureLevel.Level_11_1,FeatureLevel.Level_11_0,
               FeatureLevel.Level_10_1,FeatureLevel.Level_10_0,
               FeatureLevel.Level_9_3,FeatureLevel.Level_9_2,FeatureLevel.Level_9_1
            };
        static D2D1.CreationProperties cp = new D2D1.CreationProperties()
        {
            DebugLevel = D2D1.DebugLevel.Information,
            Options = D2D1.DeviceContextOptions.EnableMultithreadedOptimizations,
            ThreadingMode = D2D1.ThreadingMode.SingleThreaded
        };
        internal static D2DEnviroment D2D { get; private set; }
        #endregion
        /// <summary>
        /// main thread
        /// </summary>
        public static void Initial()
        {
            D2D = new D2DEnviroment();
            using (var defaultDevice = new D3D11.Device(DriverType.Hardware, D3D11.DeviceCreationFlags.BgraSupport, featureLevels))
            {
                D2D.d3dDevice = defaultDevice.QueryInterface<D3D11.Device>();
            }

            D2D.d3dContext = D2D.d3dDevice.ImmediateContext.QueryInterface<D3D11.DeviceContext>();
            // Create the Direct2D device.
            using (var dxgiDevice = D2D.d3dDevice.QueryInterface<Dxgi.Device>())
            {
                D2D.d2dDevice = new D2D1.Device(dxgiDevice,cp);
            }
            // Create Direct2D context
            D2D.d2dContext = new D2D1.DeviceContext(D2D.d2dDevice, D2D1.DeviceContextOptions.None);
            D2D.d3dContext.OutputMerger.SetTargets((D3D11.DepthStencilView)null,
                                                (D3D11.RenderTargetView)null);
            D2D.d3dContext.Flush();

            D2D.ColorBrush = new D2D1.SolidColorBrush(D2D.d2dContext, new RawColor4(0, 0, 0, 0));
        }
        /// <summary>
        /// main thread
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="dpi"></param>
        internal static void CreateD3DInstance(ref SwapChainComponent com)
        {
            int width = (int)com.panel.Width;
            int height = (int)com.panel.Height;
            dpi =(int) com.dpi;
            com.port = new Viewport(0, 0, width, height, 0, 1);
            D2D.d3dContext.Rasterizer.SetViewport(com.port);

            var multisampleDesc = new Dxgi.SampleDescription(1, 0);
            var desc = new Dxgi.SwapChainDescription1()
            {
                // Automatic sizing
                AlphaMode=Dxgi.AlphaMode.Premultiplied,
                Width = width,
                Height = height,
                Format = Dxgi.Format.B8G8R8A8_UNorm,
                Stereo = false,
                SampleDescription = multisampleDesc,
                Usage = Dxgi.Usage.RenderTargetOutput,
                BufferCount = 2,
                SwapEffect = Dxgi.SwapEffect.FlipSequential,
                Scaling = Dxgi.Scaling.Stretch
            };

            using (var dxgiDevice2 = D2D.d3dDevice.QueryInterface<Dxgi.Device2>())
            using (var dxgiAdapter = dxgiDevice2.Adapter)
            using (var dxgiFactory2 = dxgiAdapter.GetParent<Dxgi.Factory2>())
            using (com.NativePanel = ComObject.As<Dxgi.ISwapChainPanelNative>(com.panel))
            {
                com.swapChain = new Dxgi.SwapChain1(dxgiFactory2, dxgiDevice2, ref desc, null);
                com.NativePanel.SwapChain =com.swapChain;
                dxgiDevice2.MaximumFrameLatency = 1;
            }
            using (com.backbuffer = D3D11.Texture2D.FromSwapChain<D3D11.Texture2D>(com.swapChain, 0))
                com.TargetView = new D3D11.RenderTargetView(D2D.d3dDevice, com.backbuffer);

            using ( com.depthbuffer = new D3D11.Texture2D(D2D.d3dDevice, new D3D11.Texture2DDescription()
            {
                Format = Dxgi.Format.D24_UNorm_S8_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = width,
                Height = height,
                SampleDescription = multisampleDesc,
                Usage = D3D11.ResourceUsage.Default,
                BindFlags = D3D11.BindFlags.DepthStencil,
            }))
                com.StencilView = new D3D11.DepthStencilView(D2D.d3dDevice, com.depthbuffer);
            D2D.d3dContext.OutputMerger.SetTargets(com.StencilView, com.TargetView);
            var bitmapProperties = new D2D1.BitmapProperties1(
               new D2D1.PixelFormat(Dxgi.Format.B8G8R8A8_UNorm, D2D1.AlphaMode.Premultiplied),
               com.dpi, com.dpi,
               D2D1.BitmapOptions.Target | D2D1.BitmapOptions.CannotDraw);

            using (var dxgiBackBuffer = com.swapChain.GetBackBuffer<Dxgi.Surface>(0))
            {
                com.Bitmap = new D2D1.Bitmap1(D2D.d2dContext, dxgiBackBuffer, bitmapProperties);
            }
            D2D.d2dContext.Target = com.Bitmap;
            D2D.d2dContext.TextAntialiasMode = D2D1.TextAntialiasMode.Cleartype;
        }
        internal static void DisposeSwapChain(ref SwapChainComponent com)
        {
            com.backbuffer.Dispose();
            com.depthbuffer.Dispose();
            com.TargetView.Dispose();
            com.StencilView.Dispose();
            com.Bitmap.Dispose();
            com.swapChain.Dispose();
        }
        public static void Dispose()
        {
            if(D2D.d3dDevice !=null)
            {
                D2D.d3dDevice.Dispose();
                D2D.d3dDevice = null;
            }
            if(D2D.d3dContext !=null)
            {
                D2D.d3dContext.Dispose();
                D2D.d3dContext = null;
            }
            if(D2D.d2dDevice !=null)
            {
                D2D.d2dDevice.Dispose();
                D2D.d2dDevice = null;
            }
            if(D2D.d2dContext !=null)
            {
                D2D.d2dContext.Dispose();
                D2D.d2dContext = null;
            }
        }
        #region MainThread Brush
        public static LinearBrush CreateLinearBrush(LinearData l)
        {
            LinearBrush lgb;
            lock (D2D)
            {
                D2D1.GradientStopCollection gsc = new D2D1.GradientStopCollection(D2D.d2dContext, l.data);
                lgb = new LinearBrush(D2D.d2dContext, l.Point, gsc);
            }
            return lgb;
        }
        public static RadialBrush CreateRadailBrush(RadialData r)
        {
            RadialBrush rgb;
            lock (D2D)
            {
                D2D1.GradientStopCollection gsc = new D2D1.GradientStopCollection(D2D.d2dContext, r.data, D2D1.ExtendMode.Clamp);
                rgb = new RadialBrush(D2D.d2dContext, r.Point, gsc);
            }
            return rgb;
        }
        #endregion
    }
    internal delegate ComObject CreateResource(object o);
    public class ThreadManage
    {
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public extern static int GetCurrentThreadId();
        struct Resource
        {
            public int? threadId;
            public object[] buffer;
        }
        static Resource[] Dic_buffer;
        
        internal static int GetNullResourceAddress()
        {
            int id = GetCurrentThreadId();
            int c = Dic_buffer.Length;
            for (int i = 0; i < c; i++)
                if (Dic_buffer[i].threadId == id)
                {
                    var b = Dic_buffer[i].buffer;
                    for(int t=256;t<1024;t++)
                        if(b[t]==null)
                            return t;
                }
            return -1;
        }
        internal static object QurayResource(Key_Class key)
        {
            int id = GetCurrentThreadId();
            int c = Dic_buffer.Length;
            for (int i = 0; i < c; i++)
                if (Dic_buffer[i].threadId == id)
                    return Dic_buffer[i].buffer[(int)key];
            return null;
        }
        internal static void AddResource(Key_Class k, object o)
        {
            int id = GetCurrentThreadId();
            int c = Dic_buffer.Length;
            for (int i = 0; i < c; i++)
                if (Dic_buffer[i].threadId == id)
                    Dic_buffer[i].buffer[(int)k]=o;
                    return;
        }
        internal static object DeleteResource(Key_Class key)
        {
            int id = GetCurrentThreadId();
            int c = Dic_buffer.Length;
            for (int i = 0; i < c; i++)
                if (Dic_buffer[i].threadId == id)
                {
                    var o = Dic_buffer[i].buffer[(int)key];
                    Dic_buffer[i].buffer[(int)key] = null;
                    return o;
                }
            return null;
        }
        internal static int GetBindThreadSourceID()
        {
            int id = GetCurrentThreadId();
            int c = Dic_buffer.Length;
            for (int i = 0; i < c; i++)
                if (Dic_buffer[i].threadId == id)
                    return i;
            return -1;
        }
        public static void BindThreadResource(int index)
        {
            if (Dic_buffer == null)
                return;
            int id = GetCurrentThreadId();
            Dic_buffer[index].threadId = id;
            if (Dic_buffer[index].buffer == null)
                Dic_buffer[index].buffer = new object[1024];
        }
        static int wait;
        public static void MissionToMain(Action mission)
        {
            int i;
            int id = GetCurrentThreadId();
            for (i = 0; i < 3; i++)
            {
                if (Dic_buffer[i].threadId == id)
                {
                    int s = pool[i].MissionEnd;
                    pool[i].MainMission[s] = mission;
                    if (s >= 255)
                        pool[i].MissionEnd = 0;
                    else pool[i].MissionEnd++;
                    return;
                }
            }
            wait = 0;
            label:;
            if(pool[3].threadid<0)
            {
                pool[3].threadid = id;
            }
            wait++;
            if(pool[3].threadid!=id)
            {
                Task.Delay(1).Wait();
                goto label;
            }
            int ss = pool[3].MissionEnd;
            pool[3].MainMission[ss] = mission;
            if (ss >= 255)
                pool[3].MissionEnd = 0;
            else pool[3].MissionEnd++;
            pool[3].threadid = -1;
        }
        public static void ReleaseMainSource(IDisposable d)
        {
            int id = GetCurrentThreadId();
            wait = 0;
            label:;
            if (ReleaseThread<0)
            {
                ReleaseThread = id;
            }
            wait++;
            if(ReleaseThread!=id)
            {
                Task.Delay(1).Wait();
                goto label;
            }
            ReleaseResource[ReleaseEnd] = d;
            ReleaseEnd++;
            if (ReleaseEnd >= 1024)
                ReleaseEnd = 0;
        }
        static void ReleaseMainSource()
        {
            int s = ReleaseStart;
            int e = ReleaseEnd;
            if(s!=e)
            {
                for (int i = 0; i < 1024; i++)
                {
                    if (ReleaseResource[s] != null)
                        ReleaseResource[s].Dispose();
                    s++;
                    if (s >= 1024)
                        s = 0;
                    if (s == e)
                        break;
                }
            }
            ReleaseStart = e;
        }
        public static void RegTick(Action mission)
        {
            int id = GetCurrentThreadId();
            int i;
            for (i = 0; i < 4; i++)
            {
                if (Dic_buffer[i].threadId == id)
                {
                    goto label1;
                }
            }
            return;
            label1:;
            pool[i].SubMission.Add( mission);
        }
        public static void ReMoveTick(Action mission)
        {
            int id = GetCurrentThreadId();
            int i;
            for (i = 0; i < 4; i++)
            {
                if (Dic_buffer[i].threadId == id)
                {
                    goto label1;
                }
            }
            return;
            label1:;
            pool[i].SubMission.Remove(mission);
        }
        static void ExecuteInSub()
        {
            int id = GetCurrentThreadId();
            int i;
            for (i = 0; i < 3; i++)
            {
                if (Dic_buffer[i].threadId == id)
                {
                    goto label1;
                }
            }
            return;
            label1:;
            Action[] a = pool[i].SubMission.ToArray();
            int c = a.Length;
            for(int t=0;t<c;t++)
                a[t]();
        }

        internal static void ExecuteInMain()
        {
            for(int i=0;i<3;i++)
            {
                int s = pool[i].MissionStart;
                int e = pool[i].MissionEnd;
                if(s!=e)
                {
                    var temp = pool[i].MainMission;
                    for(int t=0;t<256;t++)
                    {
                        if (temp[s] != null)
                            temp[s]();
                        if (s >= 255)
                            s = 0;
                        else s++;
                        if(s==e)
                        {
                            pool[i].MissionStart = e;
                            break;
                        }
                    }
                }
            }
            int id = GetCurrentThreadId();
            wait = 0;
            label:;
            if (pool[3].threadid < 0)
            {
                pool[3].threadid = id;
            }
            wait++;
            if (pool[3].threadid != id)
            {
                if (wait > 3)
                    return;
                Task.Delay(1).Wait();
                goto label;
            }
            int ss = pool[3].MissionStart;
            int ee = pool[3].MissionEnd;
            if (ss != ee)
            {
                var temp = pool[3].MainMission;
                for (int t = 0; t < 256; t++)
                {
                    if (temp[ss] != null)
                        temp[ss]();
                    if (ss >= 255)
                        ss = 0;
                    else ss++;
                    if (ss == ee)
                    {
                        pool[3].MissionStart = ee;
                        break;
                    }
                }
            }
            pool[3].threadid = -1;
        }
        internal static void ExecuteTick()
        {
            int s;
            for (int i = 0; i < 3; i++)
            {
                if (pool[i].SubMission.Count>0 )
                {
                    s = i;
                    for (int c = 0; c < 2; c++)
                        if (buff_async[s] == null)
                        {
                            buff_async[s] = ExecuteInSub;
                            if (!pool[i].run)
                                pool[i].are.Set();
                            break;
                        }
                        else s += 3;
                }
            }
        }
        struct ThreadInfo
        {
            public List<Action> SubMission;
            public Action[] MainMission;
            public int MissionStart;
            public int MissionEnd;
            public AutoResetEvent are;
            public bool run;
            public int threadid;
            public int[] ReleaseIndex;
        }
        static ThreadInfo[] pool = new ThreadInfo[4];
        static int ReleaseStart;
        static int ReleaseEnd;
        static int ReleaseThread=-1;
        static IDisposable[] ReleaseResource=new IDisposable[1024];
        static bool stop;
        static Action[] buff_async = new Action[9];
        static Action<object>[] buff_callback = new Action<object>[9];
        static int waittask = 0;
        public static void Inital()
        {
            Dic_buffer = new Resource[4];
            stop = false;
            for (int i = 0; i < 3; i++)
            {
                pool[i].are = new AutoResetEvent(false);
                pool[i].SubMission = new List<Action>();
                pool[i].MainMission = new Action[256];
                pool[i].ReleaseIndex = new int[256];
            }
            pool[3].threadid = -1;
            pool[3].MainMission = new Action[256];
            Task.Run(() => { AsyncExcute(0); });
            Task.Run(() => { AsyncExcute(1); });
            //Task.Run(() => { AsyncExcute(2); });
        }

        public static void AsyncDelegate(Action a)
        {
            for (int i = 0; i < 6; i++)
            {
                if (a == buff_async[i])
                    return;
            }
            if (waittask >= 6)//pc
                waittask = 0;
            buff_async[waittask] = a;
            int id = waittask % 2;
            if (!pool[id].run)
                pool[id].are.Set();
            waittask++;
        }
        static void AsyncExcute(int s)
        {
            ThreadManage.BindThreadResource(s);
            pool[s].threadid = GetCurrentThreadId();
            try
            {
                while (true)
                {
                    pool[s].are.WaitOne();
                    if (stop)
                        return;
                    pool[s].run = true;
                    for (int i = s; i < buff_async.Length; i += 2)
                    {
                        if (buff_async[i] != null)
                        {
                            buff_async[i]();
                            buff_async[i] = null;
                        }
                    }
                    pool[s].run = false;
                }
                // ...
                throw null;    // 异常会在下面被捕获
                               // ...
            }
            catch (Exception ex)
            {
                // 一般会记录异常， 和/或通知其它线程我们遇到问题了
                // ...
                Debug.WriteLine(ex);
            }
        }

        public static void Stop()
        {
            stop = true;
            if (pool[0].are != null)
            {
                pool[0].are.Set();
                pool[1].are.Set();
                pool[2].are.Set();
            }
        }

        public static int TimeSlice = 16;//1000/16=60fps
        static int time;
        static bool loop;
        static SwapChain swap;
        public static void StartLoop(SwapChain sc)
        {
            swap = sc;
            time = DateTime.Now.Millisecond;
            CompositionTarget.Rendering += Rendering;
            //loop = true;
            //Task.Run(() => { DrawUILoop(); });
        }
        public static void StopLoop()
        {
            CompositionTarget.Rendering -= Rendering;
            loop = false;
        }
        public static bool UpdateUI;
        static void Rendering(object o,object e)
        {
            ExecuteTick();
#if desktop
            if (UpdateUI)
            {
                UpdateUI = false;
                swap.DrawBuffer();
                swap.UpdateUI();
            }
#else
            if (UpdateUI)
            {
                UpdateUI = false;
                swap.DrawBuffer();
            }
            swap.UpdateUI();
#endif
            ExecuteInMain();
        }
        static void DrawUILoop()
        {
            int ct = DateTime.Now.Millisecond;
            while(loop)
            {
                l:;
                int t = DateTime.Now.Millisecond;
                int r = t - ct;
                if (r < 0)
                    r += 1000;
                if (r < TimeSlice)
                {
                    Task.Delay(1).Wait();
                    goto l;
                }
                else
                {
                    ExecuteTick();
                    if (UpdateUI)
                    {
                        UpdateUI = false;
                        swap.DrawBuffer();
                    }
                    ct = t;
                }
            }
        }
    }
    enum Key_Class : int
    {
        WicIFactory, D2dIFactory2
    }
    class ThreadResource
    {
        internal static Wic.ImagingFactory2 GetWicFactory()
        {
            object o = ThreadManage.QurayResource(Key_Class.WicIFactory);
            Wic.ImagingFactory2 fac;
            if (o == null)
            {
                fac = new Wic.ImagingFactory2();
                ThreadManage.AddResource(Key_Class.WicIFactory, fac);
            }
            else fac = o as Wic.ImagingFactory2;
            return fac;
        }
        internal static D2D1.Factory GetD2DFactory()
        {
            object o = ThreadManage.QurayResource(Key_Class.D2dIFactory2);
            D2D1.Factory fac;
            if (o == null)
            {
                fac = new D2D1.Factory();
                ThreadManage.AddResource(Key_Class.D2dIFactory2, fac);
            }
            else fac = o as D2D1.Factory;
            return fac;
        }
    }
    public class LinearBrush:D2D1.LinearGradientBrush,IGradientBrush
    {
        public LinearBrush(D2D1.RenderTarget render, D2D1.LinearGradientBrushProperties radial,
             D2D1.GradientStopCollection g):base(render,radial,g)
        {
            RawStart = radial.StartPoint;
            RawEnd = radial.EndPoint;
        }
        ~LinearBrush()
        {
            DisposeA();
        }
        public void DisposeA()
        {
            GC.SuppressFinalize(this);
            base.Dispose();
        }
        public Vector2 RawStart;
        public Vector2 RawEnd;
        public void SetRect(RawRectangleF rect)
        {
            float x = rect.Left;
            float w = rect.Right - x;
            float y = rect.Top;
            float h = rect.Bottom - y;
            StartPoint = new RawVector2(w * RawStart.X + x, h * RawStart.Y + y);
            EndPoint = new RawVector2(w * RawEnd.X + x, h * RawEnd.Y + y);
        }
    }
    public class RadialBrush:D2D1.RadialGradientBrush,IGradientBrush
    {
        public RadialBrush(D2D1.RenderTarget e, D2D1.RadialGradientBrushProperties a, D2D1.GradientStopCollection g):base(e,a,g)
        {
            Opacity = 1;
            Transform = Matrix3x2.Identity;
            RawCenter = a.Center;
            RawRadiusX = a.RadiusX;
            RawRadiusY = a.RadiusY;
        }
        ~RadialBrush()
        {
            DisPoseA();
        }
        public void DisPoseA()
        {
            GC.SuppressFinalize(this);
            base.Dispose();
        }
        public Vector2 RawCenter;
        public float RawRadiusX;
        public float RawRadiusY;
        public void SetRect(RawRectangleF rect)
        {
            float x = rect.Left;
            float w = rect.Right - x;
            float y = rect.Top;
            float h = rect.Bottom - y;
            Center = new RawVector2(w * RawCenter.X + x, h * RawCenter.Y + y);
            w *= 0.5f;
            RadiusX = w;
            h *= 0.5f;
            RadiusY = h;
        }
    }
}
