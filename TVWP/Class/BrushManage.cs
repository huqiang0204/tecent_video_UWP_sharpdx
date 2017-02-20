using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DX;
using SharpDX.Mathematics.Interop;
using SharpDX;
using D2D1 = SharpDX.Direct2D1;
namespace TVWP.Class
{
    class BrushManage
    {
        #region data
        static LinearData ld1 = new LinearData()
        {
            Point = new D2D1.LinearGradientBrushProperties()
            {
                StartPoint = new RawVector2(0.5f, 0),
                EndPoint = new RawVector2(0.5f, 1),
            },
            data = new D2D1.GradientStop[]
                    {
                        new D2D1.GradientStop() { Color = new RawColor4(0.8627f, 0.8627f, 0.8627f, 0.5f) },
                        new D2D1.GradientStop() { Color = new RawColor4(0.2627f, 0.3027f, 0.3627f, 1f),Position=0.5f },
                        new D2D1.GradientStop() { Color = new RawColor4(0.8627f, 0.8627f, 0.8627f, 0.5f),Position=1}
                    }

        };
        static LinearData ld2 = new LinearData()
        {
            Point = new D2D1.LinearGradientBrushProperties()
            {
                StartPoint = new RawVector2(0.5f, 0),
                EndPoint = new RawVector2(0.5f, 1),
            },
            data = new D2D1.GradientStop[]
                   {
                        new D2D1.GradientStop() { Color = new RawColor4(0.8627f, 0.8627f, 0.8627f, 0.5f) },
                        new D2D1.GradientStop() { Color = new RawColor4(0.32f, 0.44f, 0.6f, 1f),Position=0.5f },
                        new D2D1.GradientStop() { Color = new RawColor4(0.8627f, 0.8627f, 0.8627f, 0.5f),Position=1}
                   }

        };
        static LinearData ldR = new LinearData()
        {
            Point = new D2D1.LinearGradientBrushProperties()
            {
                StartPoint = new RawVector2(0.5f, 0),
                EndPoint = new RawVector2(0.5f, 1),
            },
            data = new D2D1.GradientStop[]
                    {
                        new D2D1.GradientStop() { Color = new RawColor4(0.8627f, 0.8627f, 0.8627f, 0.8f) },
                        new D2D1.GradientStop() { Color = new RawColor4(1f, 0f, 0f, 1f),Position=0.5f },
                        new D2D1.GradientStop() { Color = new RawColor4(0.8627f, 0.8627f, 0.8627f, 0.8f),Position=1}
                    }

        };
        static LinearData ldG = new LinearData()
        {
            Point = new D2D1.LinearGradientBrushProperties()
            {
                StartPoint = new RawVector2(0.5f, 0),
                EndPoint = new RawVector2(0.5f, 1),
            },
            data = new D2D1.GradientStop[]
                   {
                        new D2D1.GradientStop() { Color = new RawColor4(0.8627f, 0.8627f, 0.8627f, 0.5f) },
                        new D2D1.GradientStop() { Color = new RawColor4(0, 1, 0, 1f),Position=0.5f },
                        new D2D1.GradientStop() { Color = new RawColor4(0.8627f, 0.8627f, 0.8627f, 0.5f),Position=1}
                   }

        };
        static RadialData rd1 = new RadialData()
        {
            Point = new D2D1.RadialGradientBrushProperties()
            {
                Center = new RawVector2(0.5f, 0.5f),
                RadiusX = 1,
                RadiusY = 1
            },
            data = new D2D1.GradientStop[]
               {
                    new D2D1.GradientStop() { Color=new RawColor4(0.8627f, 0.8627f, 0.8627f, 0.5f)},
                    new D2D1.GradientStop() {Color=new RawColor4(0.2627f, 0.3027f, 0.3627f, 1f) ,Position=1}
               }
        };
        static RadialData rd2 = new RadialData()
        {
            Point = new D2D1.RadialGradientBrushProperties()
            {
                Center = new RawVector2(0.5f, 0.5f),
                RadiusX = 1,
                RadiusY = 1
            },
            data = new D2D1.GradientStop[]
                {
                    new D2D1.GradientStop() { Color=new DX.Color(252, 226,124, 255)},
                    new D2D1.GradientStop() {Color=new DX.Color(202, 198, 181,128) ,Position=1}
                }
        };
        static RadialData rdc1 = new RadialData()
        {
            Point = new D2D1.RadialGradientBrushProperties()
            {
                Center = new RawVector2(0.5f, 0.5f),
                RadiusX = 1,
                RadiusY = 1
            },
            data = new D2D1.GradientStop[]
               {
                    new D2D1.GradientStop() { },
                    new D2D1.GradientStop() { Color=new RawColor4(0.8627f, 0.8627f, 0.8627f, 0.5f),Position=0.8f},
                    new D2D1.GradientStop() {Color=new RawColor4(0.2627f, 0.3027f, 0.3627f, 1f) ,Position=1}
               }
        };
        static RadialData rdc2 = new RadialData()
        {
            Point = new D2D1.RadialGradientBrushProperties()
            {
                Center = new RawVector2(0.5f, 0.5f),
                RadiusX = 1,
                RadiusY = 1
            },
            data = new D2D1.GradientStop[]
                {
                     new D2D1.GradientStop() { },
                     new D2D1.GradientStop() {Color=new DX.Color(46, 114, 254,128) ,Position=0.8f},
                    new D2D1.GradientStop() { Color=new DX.Color(46, 254,114, 180),Position=1}
                    
                }
        };
        static RadialData rdR = new RadialData()
        {
            Point = new D2D1.RadialGradientBrushProperties()
            {
                Center = new RawVector2(0.5f, 0.5f),
                RadiusX = 1,
                RadiusY = 1
            },
            data = new D2D1.GradientStop[]
            {
                    new D2D1.GradientStop() {Color=new DX.Color(255, 0,0, 255)},
                    new D2D1.GradientStop() {Color=new DX.Color(255, 128, 128,160) ,Position=1}
            }
        };
        #endregion
        static LinearBrush Lbrush_A;
        static LinearBrush Lbrush_B;
        static LinearBrush Lbrush_R;
        static LinearBrush Lbrush_G;
        static RadialBrush Rbrush_A;
        static RadialBrush Rbrush_B;
        static RadialBrush Rbrush_C;
        static RadialBrush Rbrush_D;
        static RadialBrush Rbrush_R;
        public static LinearBrush GetLinearA()
        {
            if(Lbrush_A==null)
                Lbrush_A = DX_Core.CreateLinearBrush(ld1);
            return Lbrush_A;
        }
        public static LinearBrush GetLinearB()
        {
            if (Lbrush_B == null)
                Lbrush_B = DX_Core.CreateLinearBrush(ld2);
            return Lbrush_B;
        }
        public static LinearBrush GetLinearG()
        {
            if (Lbrush_G == null)
                Lbrush_G = DX_Core.CreateLinearBrush(ldG);
            return Lbrush_G;
        }
        public static LinearBrush GetLinearR()
        {
            if (Lbrush_R == null)
                Lbrush_R = DX_Core.CreateLinearBrush(ldR);
            return Lbrush_R;
        }
        public static RadialBrush GetRadia_A()
        {
            if(Rbrush_A==null)
                Rbrush_A = DX_Core.CreateRadailBrush(rd1);
            return Rbrush_A;
        }
        public static RadialBrush GetRadia_B()
        {
            if (Rbrush_B == null)
                Rbrush_B = DX_Core.CreateRadailBrush(rd2);
            return Rbrush_B;
        }
        public static RadialBrush GetRadia_C()
        {
            if (Rbrush_C == null)
                Rbrush_C = DX_Core.CreateRadailBrush(rdc1);
            return Rbrush_C;
        }
        public static RadialBrush GetRadia_D()
        {
            if (Rbrush_D == null)
                Rbrush_D = DX_Core.CreateRadailBrush(rdc2);
            return Rbrush_D;
        }
        public static RadialBrush GetRadia_R()
        {
            if (Rbrush_R == null)
                Rbrush_R = DX_Core.CreateRadailBrush(rdR);
            return Rbrush_R;
        }
    }
}
