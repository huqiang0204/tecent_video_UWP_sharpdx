using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DX;
using System.Diagnostics;
using Windows.UI;
using SharpDX.Mathematics.Interop;
using Windows.UI.ViewManagement;

namespace TVWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            ThreadManage.Inital();
            ThreadManage.AsyncDelegate(() => { MathF.InitalTabel(); });
            ThreadManage.AsyncDelegate(()=> { Class.WebClass.Initial(); } );
        }
        public static Canvas Main;
        public SwapChain Swapchain;
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            //Debug.WriteLine(Colors.LightPink);
            //Debug.WriteLine(Colors.GhostWhite);
            
            var t= FontFamily.XamlAutoFontFamily;
            double width = Window.Current.Bounds.Width;
            double height = Window.Current.Bounds.Height;
            Class.Component.screenX = (float)width;
            Class.Component.screenY = (float)height;
            DX.DX_Core.Initial();
            Swapchain = new DX.SwapChain(width, height);
            Main = new Canvas();
            Main.Width = width;
            Main.Height = height;
            Main.Background = new SolidColorBrush(Colors.DimGray);
            Window.Current.Content = Main;
            Main.Children.Add(Swapchain);
            Window.Current.Activate();
            Window.Current.SizeChanged += (o, a) =>
            {
                double w = Window.Current.Bounds.Width;
                double h = Window.Current.Bounds.Height;
                Main.Width = w;
                Main.Height = h;
                Class.Component.screenX =(float) w;
                Class.Component.screenY =(float) h;
                Swapchain.ReSize(w, h);
                Class.PageManageEx.ReSize();
            };
            //delegate sub thread create ui
            ThreadManage.BindThreadResource(3);
            ThreadManage.AsyncDelegate(() =>{ Class.Main.Initial(Swapchain); });
#if phone
            StatusBar.GetForCurrentView().BackgroundColor = Colors.Black;
            StatusBar s = StatusBar.GetForCurrentView();
            s.BackgroundOpacity = 1;
            s.ForegroundColor = Colors.White;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
#endif
            Window.Current.CoreWindow.KeyUp += KeyUp;
        }
        void KeyUp(object sender, KeyEventArgs k)
        {
            if(k.VirtualKey==Windows.System.VirtualKey.Escape)
            {
                if (Class.PageManageEx.Back())
                {
                    Exit();
                }
            }
        }
        void OnBackRequested(Object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
           if( Class.PageManageEx.Back())
            Exit();
        }
        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            
            deferral.Complete();
        }
    }
}
