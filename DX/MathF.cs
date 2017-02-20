using System;
using SharpDX;
using Windows.Storage.Streams;
using System.IO;

namespace DX
{
    public class MathF
    {
        #region angle table 精度1
        public static readonly Vector2[] angle_table = new Vector2[]{new Vector2(0f, 1f),
new Vector2(-0.01745241f, 0.9998477f),new Vector2(-0.0348995f, 0.9993908f),new Vector2(-0.05233596f, 0.9986295f),new Vector2(-0.06975647f, 0.9975641f),new Vector2(-0.08715574f, 0.9961947f),new Vector2(-0.1045285f, 0.9945219f),new Vector2(-0.1218693f, 0.9925461f),new Vector2(-0.1391731f, 0.9902681f),new Vector2(-0.1564345f, 0.9876884f),new Vector2(-0.1736482f, 0.9848077f),
new Vector2(-0.190809f, 0.9816272f),new Vector2(-0.2079117f, 0.9781476f),new Vector2(-0.224951f, 0.9743701f),new Vector2(-0.2419219f, 0.9702957f),new Vector2(-0.258819f, 0.9659258f),new Vector2(-0.2756374f, 0.9612617f),new Vector2(-0.2923717f, 0.9563048f),new Vector2(-0.309017f, 0.9510565f),new Vector2(-0.3255681f, 0.9455186f),new Vector2(-0.3420201f, 0.9396926f),
new Vector2(-0.3583679f, 0.9335804f),new Vector2(-0.3746066f, 0.9271839f),new Vector2(-0.3907311f, 0.9205049f),new Vector2(-0.4067366f, 0.9135455f),new Vector2(-0.4226183f, 0.9063078f),new Vector2(-0.4383712f, 0.8987941f),new Vector2(-0.4539905f, 0.8910065f),new Vector2(-0.4694716f, 0.8829476f),new Vector2(-0.4848096f, 0.8746197f),new Vector2(-0.5f, 0.8660254f),
new Vector2(-0.5150381f, 0.8571673f),new Vector2(-0.5299193f, 0.8480481f),new Vector2(-0.5446391f, 0.8386706f),new Vector2(-0.5591929f, 0.8290376f),new Vector2(-0.5735765f, 0.8191521f),new Vector2(-0.5877852f, 0.809017f),new Vector2(-0.601815f, 0.7986355f),new Vector2(-0.6156615f, 0.7880108f),new Vector2(-0.6293204f, 0.7771459f),new Vector2(-0.6427876f, 0.7660444f),
new Vector2(-0.656059f, 0.7547096f),new Vector2(-0.6691306f, 0.7431448f),new Vector2(-0.6819984f, 0.7313537f),new Vector2(-0.6946584f, 0.7193398f),new Vector2(-0.7071068f, 0.7071068f),new Vector2(-0.7193398f, 0.6946584f),new Vector2(-0.7313537f, 0.6819984f),new Vector2(-0.7431448f, 0.6691306f),new Vector2(-0.7547095f, 0.656059f),new Vector2(-0.7660444f, 0.6427876f),
new Vector2(-0.7771459f, 0.6293204f),new Vector2(-0.7880107f, 0.6156615f),new Vector2(-0.7986355f, 0.601815f),new Vector2(-0.809017f, 0.5877853f),new Vector2(-0.8191521f, 0.5735765f),new Vector2(-0.8290375f, 0.5591929f),new Vector2(-0.8386706f, 0.5446391f),new Vector2(-0.8480481f, 0.5299193f),new Vector2(-0.8571673f, 0.5150381f),new Vector2(-0.8660254f, 0.5f),
new Vector2(-0.8746197f, 0.4848097f),new Vector2(-0.8829476f, 0.4694716f),new Vector2(-0.8910065f, 0.4539905f),new Vector2(-0.8987941f, 0.4383712f),new Vector2(-0.9063078f, 0.4226182f),new Vector2(-0.9135455f, 0.4067366f),new Vector2(-0.9205048f, 0.3907312f),new Vector2(-0.9271839f, 0.3746066f),new Vector2(-0.9335804f, 0.358368f),new Vector2(-0.9396926f, 0.3420202f),
new Vector2(-0.9455186f, 0.3255681f),new Vector2(-0.9510565f, 0.309017f),new Vector2(-0.9563047f, 0.2923718f),new Vector2(-0.9612617f, 0.2756374f),new Vector2(-0.9659258f, 0.2588191f),new Vector2(-0.9702957f, 0.2419219f),new Vector2(-0.9743701f, 0.224951f),new Vector2(-0.9781476f, 0.2079117f),new Vector2(-0.9816272f, 0.1908091f),new Vector2(-0.9848077f, 0.1736482f),
new Vector2(-0.9876884f, 0.1564345f),new Vector2(-0.9902681f, 0.1391731f),new Vector2(-0.9925461f, 0.1218693f),new Vector2(-0.9945219f, 0.1045284f),new Vector2(-0.9961947f, 0.0871558f),new Vector2(-0.9975641f, 0.06975651f),new Vector2(-0.9986295f, 0.05233597f),new Vector2(-0.9993908f, 0.0348995f),new Vector2(-0.9998477f, 0.01745238f),new Vector2(-1f, 0),
new Vector2(-0.9998477f, -0.01745235f),new Vector2(-0.9993908f, -0.03489946f),new Vector2(-0.9986295f, -0.05233594f),new Vector2(-0.9975641f, -0.06975648f),new Vector2(-0.9961947f, -0.08715577f),new Vector2(-0.9945219f, -0.1045284f),new Vector2(-0.9925461f, -0.1218693f),new Vector2(-0.9902681f, -0.1391731f),new Vector2(-0.9876884f, -0.1564344f),new Vector2(-0.9848077f, -0.1736482f),
new Vector2(-0.9816272f, -0.190809f),new Vector2(-0.9781476f, -0.2079116f),new Vector2(-0.9743701f, -0.224951f),new Vector2(-0.9702957f, -0.2419219f),new Vector2(-0.9659258f, -0.258819f),new Vector2(-0.9612617f, -0.2756374f),new Vector2(-0.9563047f, -0.2923717f),new Vector2(-0.9510565f, -0.3090169f),new Vector2(-0.9455186f, -0.3255681f),new Vector2(-0.9396926f, -0.3420201f),
new Vector2(-0.9335805f, -0.3583679f),new Vector2(-0.9271839f, -0.3746066f),new Vector2(-0.9205049f, -0.3907312f),new Vector2(-0.9135455f, -0.4067366f),new Vector2(-0.9063078f, -0.4226183f),new Vector2(-0.8987941f, -0.4383711f),new Vector2(-0.8910066f, -0.4539904f),new Vector2(-0.8829476f, -0.4694716f),new Vector2(-0.8746197f, -0.4848095f),new Vector2(-0.8660254f, -0.5000001f),
new Vector2(-0.8571673f, -0.515038f),new Vector2(-0.8480482f, -0.5299191f),new Vector2(-0.8386706f, -0.5446391f),new Vector2(-0.8290376f, -0.5591928f),new Vector2(-0.819152f, -0.5735765f),new Vector2(-0.809017f, -0.5877852f),new Vector2(-0.7986355f, -0.6018151f),new Vector2(-0.7880108f, -0.6156614f),new Vector2(-0.777146f, -0.6293203f),new Vector2(-0.7660444f, -0.6427876f),
new Vector2(-0.7547096f, -0.656059f),new Vector2(-0.7431448f, -0.6691307f),new Vector2(-0.7313537f, -0.6819983f),new Vector2(-0.7193399f, -0.6946583f),new Vector2(-0.7071068f, -0.7071068f),new Vector2(-0.6946585f, -0.7193397f),new Vector2(-0.6819983f, -0.7313538f),new Vector2(-0.6691306f, -0.7431448f),new Vector2(-0.656059f, -0.7547097f),new Vector2(-0.6427876f, -0.7660444f),
new Vector2(-0.6293205f, -0.7771459f),new Vector2(-0.6156614f, -0.7880108f),new Vector2(-0.6018151f, -0.7986355f),new Vector2(-0.5877852f, -0.8090171f),new Vector2(-0.5735765f, -0.8191521f),new Vector2(-0.559193f, -0.8290375f),new Vector2(-0.5446391f, -0.8386706f),new Vector2(-0.5299193f, -0.848048f),new Vector2(-0.515038f, -0.8571673f),new Vector2(-0.5000001f, -0.8660254f),
new Vector2(-0.4848098f, -0.8746197f),new Vector2(-0.4694716f, -0.8829476f),new Vector2(-0.4539906f, -0.8910065f),new Vector2(-0.4383711f, -0.8987941f),new Vector2(-0.4226183f, -0.9063078f),new Vector2(-0.4067366f, -0.9135455f),new Vector2(-0.3907312f, -0.9205049f),new Vector2(-0.3746067f, -0.9271838f),new Vector2(-0.3583679f, -0.9335805f),new Vector2(-0.3420202f, -0.9396926f),
new Vector2(-0.3255681f, -0.9455186f),new Vector2(-0.309017f, -0.9510565f),new Vector2(-0.2923718f, -0.9563047f),new Vector2(-0.2756374f, -0.9612617f),new Vector2(-0.2588191f, -0.9659258f),new Vector2(-0.2419219f, -0.9702957f),new Vector2(-0.2249511f, -0.9743701f),new Vector2(-0.2079116f, -0.9781476f),new Vector2(-0.190809f, -0.9816272f),new Vector2(-0.1736483f, -0.9848077f),
new Vector2(-0.1564344f, -0.9876884f),new Vector2(-0.1391732f, -0.9902681f),new Vector2(-0.1218693f, -0.9925461f),new Vector2(-0.1045285f, -0.9945219f),new Vector2(-0.08715588f, -0.9961947f),new Vector2(-0.06975647f, -0.9975641f),new Vector2(-0.05233605f, -0.9986295f),new Vector2(-0.03489945f, -0.9993908f),new Vector2(-0.01745246f, -0.9998477f),new Vector2(0, -1f),
new Vector2(0.01745239f, -0.9998477f),new Vector2(0.03489939f, -0.9993908f),new Vector2(0.05233599f, -0.9986295f),new Vector2(0.0697564f, -0.9975641f),new Vector2(0.08715581f, -0.9961947f),new Vector2(0.1045284f, -0.9945219f),new Vector2(0.1218692f, -0.9925461f),new Vector2(0.1391731f, -0.9902681f),new Vector2(0.1564344f, -0.9876884f),new Vector2(0.1736482f, -0.9848077f),
new Vector2(0.190809f, -0.9816272f),new Vector2(0.2079116f, -0.9781476f),new Vector2(0.224951f, -0.9743701f),new Vector2(0.2419218f, -0.9702957f),new Vector2(0.2588191f, -0.9659258f),new Vector2(0.2756373f, -0.9612617f),new Vector2(0.2923718f, -0.9563047f),new Vector2(0.309017f, -0.9510565f),new Vector2(0.3255681f, -0.9455186f),new Vector2(0.3420202f, -0.9396926f),
new Vector2(0.3583679f, -0.9335805f),new Vector2(0.3746066f, -0.9271838f),new Vector2(0.3907311f, -0.9205049f),new Vector2(0.4067365f, -0.9135455f),new Vector2(0.4226183f, -0.9063078f),new Vector2(0.4383711f, -0.8987941f),new Vector2(0.4539905f, -0.8910065f),new Vector2(0.4694715f, -0.8829476f),new Vector2(0.4848095f, -0.8746198f),new Vector2(0.5f, -0.8660254f),
new Vector2(0.515038f, -0.8571674f),new Vector2(0.5299193f, -0.8480481f),new Vector2(0.544639f, -0.8386706f),new Vector2(0.559193f, -0.8290375f),new Vector2(0.5735764f, -0.8191521f),new Vector2(0.5877851f, -0.8090171f),new Vector2(0.601815f, -0.7986355f),new Vector2(0.6156614f, -0.7880108f),new Vector2(0.6293204f, -0.7771459f),new Vector2(0.6427876f, -0.7660445f),
new Vector2(0.6560589f, -0.7547097f),new Vector2(0.6691306f, -0.7431448f),new Vector2(0.6819983f, -0.7313538f),new Vector2(0.6946584f, -0.7193398f),new Vector2(0.7071067f, -0.7071068f),new Vector2(0.7193398f, -0.6946583f),new Vector2(0.7313537f, -0.6819984f),new Vector2(0.7431448f, -0.6691307f),new Vector2(0.7547096f, -0.656059f),new Vector2(0.7660446f, -0.6427875f),
new Vector2(0.777146f, -0.6293203f),new Vector2(0.7880107f, -0.6156615f),new Vector2(0.7986354f, -0.6018152f),new Vector2(0.8090168f, -0.5877854f),new Vector2(0.8191521f, -0.5735763f),new Vector2(0.8290376f, -0.5591929f),new Vector2(0.8386706f, -0.5446391f),new Vector2(0.848048f, -0.5299194f),new Vector2(0.8571672f, -0.5150383f),new Vector2(0.8660254f, -0.4999999f),
new Vector2(0.8746197f, -0.4848096f),new Vector2(0.8829476f, -0.4694716f),new Vector2(0.8910065f, -0.4539907f),new Vector2(0.8987939f, -0.4383714f),new Vector2(0.9063078f, -0.4226182f),new Vector2(0.9135454f, -0.4067366f),new Vector2(0.9205048f, -0.3907312f),new Vector2(0.9271838f, -0.3746068f),new Vector2(0.9335805f, -0.3583678f),new Vector2(0.9396927f, -0.3420201f),
new Vector2(0.9455186f, -0.3255682f),new Vector2(0.9510565f, -0.3090171f),new Vector2(0.9563047f, -0.2923719f),new Vector2(0.9612617f, -0.2756372f),new Vector2(0.9659259f, -0.258819f),new Vector2(0.9702957f, -0.2419219f),new Vector2(0.9743701f, -0.2249512f),new Vector2(0.9781476f, -0.2079119f),new Vector2(0.9816272f, -0.1908088f),new Vector2(0.9848078f, -0.1736481f),
new Vector2(0.9876883f, -0.1564345f),new Vector2(0.9902681f, -0.1391733f),new Vector2(0.9925461f, -0.1218696f),new Vector2(0.9945219f, -0.1045283f),new Vector2(0.9961947f, -0.08715571f),new Vector2(0.997564f, -0.06975655f),new Vector2(0.9986295f, -0.05233612f),new Vector2(0.9993908f, -0.03489976f),new Vector2(0.9998477f, -0.0174523f),new Vector2(1f, 0f),
new Vector2(0.9998477f, 0.01745232f),new Vector2(0.9993908f, 0.03489931f),new Vector2(0.9986296f, 0.05233567f),new Vector2(0.997564f, 0.06975657f),new Vector2(0.9961947f, 0.08715574f),new Vector2(0.9945219f, 0.1045284f),new Vector2(0.9925462f, 0.1218691f),new Vector2(0.9902681f, 0.1391733f),new Vector2(0.9876883f, 0.1564345f),new Vector2(0.9848077f, 0.1736481f),
new Vector2(0.9816272f, 0.1908089f),new Vector2(0.9781476f, 0.2079115f),new Vector2(0.97437f, 0.2249512f),new Vector2(0.9702957f, 0.2419219f),new Vector2(0.9659258f, 0.258819f),new Vector2(0.9612617f, 0.2756372f),new Vector2(0.9563048f, 0.2923715f),new Vector2(0.9510565f, 0.3090171f),new Vector2(0.9455186f, 0.3255682f),new Vector2(0.9396926f, 0.3420201f),
new Vector2(0.9335805f, 0.3583678f),new Vector2(0.9271839f, 0.3746064f),new Vector2(0.9205048f, 0.3907312f),new Vector2(0.9135454f, 0.4067367f),new Vector2(0.9063078f, 0.4226182f),new Vector2(0.8987941f, 0.438371f),new Vector2(0.8910066f, 0.4539903f),new Vector2(0.8829476f, 0.4694717f),new Vector2(0.8746197f, 0.4848096f),new Vector2(0.8660254f, 0.4999999f),
new Vector2(0.8571674f, 0.5150379f),new Vector2(0.8480483f, 0.529919f),new Vector2(0.8386705f, 0.5446391f),new Vector2(0.8290376f, 0.5591929f),new Vector2(0.8191521f, 0.5735763f),new Vector2(0.8090171f, 0.5877851f),new Vector2(0.7986354f, 0.6018152f),new Vector2(0.7880107f, 0.6156615f),new Vector2(0.777146f, 0.6293204f),new Vector2(0.7660445f, 0.6427875f),
new Vector2(0.7547097f, 0.6560588f),new Vector2(0.7431448f, 0.6691307f),new Vector2(0.7313536f, 0.6819984f),new Vector2(0.7193398f, 0.6946583f),new Vector2(0.7071069f, 0.7071066f),new Vector2(0.6946585f, 0.7193396f),new Vector2(0.6819983f, 0.7313538f),new Vector2(0.6691306f, 0.7431449f),new Vector2(0.6560591f, 0.7547095f),new Vector2(0.6427878f, 0.7660443f),
new Vector2(0.6293206f, 0.7771458f),new Vector2(0.6156614f, 0.7880108f),new Vector2(0.601815f, 0.7986355f),new Vector2(0.5877853f, 0.8090169f),new Vector2(0.5735766f, 0.8191519f),new Vector2(0.5591931f, 0.8290374f),new Vector2(0.5446389f, 0.8386706f),new Vector2(0.5299193f, 0.8480481f),new Vector2(0.5150381f, 0.8571672f),new Vector2(0.5000002f, 0.8660253f),
new Vector2(0.4848099f, 0.8746195f),new Vector2(0.4694715f, 0.8829476f),new Vector2(0.4539905f, 0.8910065f),new Vector2(0.4383712f, 0.898794f),new Vector2(0.4226184f, 0.9063077f),new Vector2(0.4067365f, 0.9135455f),new Vector2(0.3907311f, 0.9205049f),new Vector2(0.3746066f, 0.9271839f),new Vector2(0.3583681f, 0.9335804f),new Vector2(0.3420204f, 0.9396926f),
new Vector2(0.325568f, 0.9455186f),new Vector2(0.3090169f, 0.9510565f),new Vector2(0.2923717f, 0.9563047f),new Vector2(0.2756375f, 0.9612616f),new Vector2(0.2588193f, 0.9659258f),new Vector2(0.2419218f, 0.9702958f),new Vector2(0.224951f, 0.9743701f),new Vector2(0.2079118f, 0.9781476f),new Vector2(0.1908092f, 0.9816272f),new Vector2(0.1736484f, 0.9848077f),
new Vector2(0.1564344f, 0.9876884f),new Vector2(0.1391731f, 0.9902681f),new Vector2(0.1218694f, 0.9925461f),new Vector2(0.1045287f, 0.9945219f),new Vector2(0.08715603f, 0.9961947f),new Vector2(0.06975638f, 0.9975641f),new Vector2(0.05233596f, 0.9986295f),new Vector2(0.0348996f, 0.9993908f),new Vector2(0.01745261f, 0.9998477f),new Vector2(0f, 1f),};
        #endregion

        #region angle tabe 精度0.01
        static byte[] anglebuff;
        unsafe static float* fp;
        public static async void InitalTabel()
        {
            anglebuff = new byte[36004];
            Uri u= new Uri("ms-appx:///DX/sin9000.png");
            RandomAccessStreamReference rass = RandomAccessStreamReference.CreateFromUri(u);
            IRandomAccessStream ir = await rass.OpenReadAsync();
            ir.AsStream().Read(anglebuff,0,36004);
            unsafe
            {
                fixed (byte* bp = &anglebuff[0])
                    fp = (float*)bp;
            }
        }
        public unsafe static float Sin(float ax)
        {
            if (ax >= 360)
                ax -= 360;
            if (ax < 0)
                ax += 360;
            if (ax > 270)
            {
                ax = 360 - ax;
                goto label1;
            }
            else if (ax > 180)
            {
                ax -= 180;
                goto label1;
            }
            else if (ax > 90)
            {
                ax = 180 - ax;
            }
            int id = (int)(ax * 100);
            return *(fp + id);
            label1:;
            id = (int)(ax * 100);
            return -*(fp + id);
        }
        public static float Cos(float angle)
        {
            return Sin(angle + 90);
        }
        #endregion

        unsafe public static float SquareRootFloat(float number)
        {
            int i; float x, y;
            const float f = 1.5F;
            x = number * 0.5F;
            y = number;
            i = *(int*)&y;
            i = 0x5f375a86 - (i >> 1); //注意这一行  
            y = *(float*)&i;
            y = y * (f - (x * y * y));
            y = y * (f - (x * y * y));
            y *= number;
            if (y < 0)
                y = 0 - y;
            return y;
        }
        public static float atan(float dx, float dy)
        {
            //ax<ay
            float ax = dx < 0 ? -dx : dx, ay = dy < 0 ? -dy : dy;
            float a;
            if (ax < ay) a = ax / ay; else a = ay / ax;
            float s = a * a;
            float r = ((-0.0464964749f * s + 0.15931422f) * s - 0.327622764f) * s * a + a;
            if (ay > ax) r = 1.57079637f - r;
            r *= 57.32484f;
            if (dx < 0)
            {
                if (dy < 0)
                    r += 90;
                else r = 90 - r;
            }
            else
            {
                if (dy < 0)
                    r = 270 - r;
                else r += 270;
            }
            return r;
        }

        #region collion check

        public static Vector2[] GetPointsOffset(Vector3 location, ref Vector2[] offsest)
        {
            Vector2[] temp = new Vector2[offsest.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i].X = location.X + offsest[i].X;
                temp[i].Y = location.Y + offsest[i].Y;
            }
            return temp;
        }
        public static bool DotToEllipse(Vector2 ell_location, Vector2 dot, float xlen, float ylen)
        {
            float x = ell_location.X - dot.X;
            x *= x;
            float y = ell_location.Y - dot.Y;
            y *= y;
            xlen *= xlen;
            x /= xlen;
            ylen *= ylen;
            y /= ylen;
            return x + y < 1;
        }
        public static bool DotToEllipse(Vector2 ell_location, Vector2 dot, float xlen, float ylen,float angle)
        {
            int a = (int)angle;
            float x = ell_location.X - dot.X;
            x *= angle_table[a].X;
            x *= x;
            float y = ell_location.Y - dot.Y;
            y *= angle_table[a].Y;
            y *= y;
            xlen *= xlen;
            x /= xlen;
            ylen *= ylen;
            y /= ylen;
            return x+y<1;
        }
        public static bool DotToPolygon(Vector2 origion, Vector2[] A, Vector2 B)//offset
        {
            int count = 0;
            for (int i = 0; i < A.Length; i++)
            {
                Vector2 p1 = A[i];
                p1.X += origion.X;
                p1.Y += origion.Y;
                Vector2 p2 = i == A.Length - 1 ? A[0] : A[i + 1];
                p2.X += origion.X;
                p2.Y += origion.Y;
                if (B.Y >= p1.Y & B.Y <= p2.Y | B.Y >= p2.Y & B.Y <= p1.Y)
                {
                    float t = (B.Y - p1.Y) / (p2.Y - p1.Y);
                    float xt = p1.X + t * (p2.X - p1.X);
                    if (B.X == xt)
                        return true;
                    if (B.X < xt)
                        count++;
                }
            }
            return count % 2 > 0 ? true : false;
        }
        public static bool DotToPolygon(Vector2[] A, Vector2 B)//rotate
        {
            int count = 0;
            for (int i = 0; i < A.Length; i++)
            {
                Vector2 p1 = A[i];
                Vector2 p2 = i == A.Length - 1 ? A[0] : A[i + 1];
                if (B.Y >= p1.Y & B.Y <= p2.Y | B.Y >= p2.Y & B.Y <= p1.Y)
                {
                    float t = (B.Y - p1.Y) / (p2.Y - p1.Y);
                    float xt = p1.X + t * (p2.X - p1.X);
                    if (B.X == xt)
                        return true;
                    if (B.X < xt)
                        count++;
                }
            }
            return count % 2 > 0 ? true : false;
        }
        public static bool CircleToCircle(Vector2 A, Vector2 B, float radiusA, float radiusB)
        {
            float x = A.X - B.X;
            x *= x;
            float y = A.Y - B.Y;
            y *= y;
            y += x;
            return radiusA + radiusB > SquareRootFloat(y);
        }
        public static Vector2 RotateVector2(Vector2 p, Vector2 location, float angle)//a=绝对角度 d=直径
        {
            float a = p.X + angle;
            if (a < 0)
                a += 360;
            if (a > 360)
                a -= 360;
            //a *= 0.0174533f;//change angle to radin
            float d = p.Y;
            Vector2 temp = new Vector2();
            temp.X = location.X - Sin(a) * d;
            temp.Y = location.Y + Cos(a) * d;
            return temp;
        }
        public static Vector3 RotateVector3(Vector2 p, ref Vector3 location, float angle)//a=绝对角度 d=直径
        {
            int a = (int)(p.X + angle);
            if (a < 0)
                a += 360;
            if (a > 360)
                a -= 360;
            float d = p.Y;
            Vector3 temp = location;
            temp.X = location.X + angle_table[a].X * d;
            temp.Y = location.Y + angle_table[a].Y * d;
            return temp;
        }
        public static void RotatePoint2(ref Vector2 p, ref Vector2 location, float angle, ref Vector3 o)//a=绝对角度 d=直径
        {
            int a = (int)(p.X + angle);
            if (a < 0)
                a += 360;
            if (a > 360)
                a -= 360;
            float d = p.Y;
            o.X = location.X + angle_table[a].X * d;
            o.Y = location.Y + angle_table[a].Y * d;
        }
        public static Vector2[] RotateVector2(Vector2[] P, Vector2 location, float angle)//p[].x=绝对角度 p[].y=直径
        {
            Vector2[] temp = new Vector2[P.Length];
            for (int i = 0; i < P.Length; i++)
            {
                int a = (int)(P[i].X + angle);//change angle to radin
                if (a < 0)
                    a += 360;
                if (a >= 360)
                    a -= 360;
                temp[i].X = location.X + angle_table[a].X * P[i].Y;
                temp[i].Y = location.Y + angle_table[a].Y * P[i].Y;
            }
            return temp;
        }
        public static void RotatePoint3(ref Vector4[] raw, ref Vector3[] Vertical, Vector3 location, float angle,float scale ,int count)//p[].x=绝对角度 p[].y=直径
        {
            Vector3[] temp = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                int a = (int)(raw[i].X + angle);//change angle to radin
                if (a < 0)
                    a += 360;
                if (a >= 360)
                    a -= 360;
                Vertical[i].X = location.X + angle_table[a].X * raw[i].W;
                Vertical[i].X *= scale;
                Vertical[i].Y = location.Y + angle_table[a].Y * raw[i].W;
                Vertical[i].Y *= scale;
                Vertical[i].Z = raw[i].Z;
            }
        }
        unsafe public static void RotatePoint3(ref Vector4[] raw, byte* bp,int offset,int structsize, Vector3 location, float angle, float scale, int count)
        {
            Vector3[] temp = new Vector3[count];
            bp += offset;
            for (int i = 0; i < count; i++)
            {
                Vector3* vp = (Vector3*)bp;
                int a = (int)(raw[i].X + angle);//change angle to radin
                if (a < 0)
                    a += 360;
                if (a >= 360)
                    a -= 360;
                vp->X = location.X + angle_table[a].X * raw[i].W;
                vp->X *= scale;
                vp->Y = location.Y + angle_table[a].Y * raw[i].W;
                vp->Y *= scale;
                vp->Z = raw[i].Z;
                bp += structsize;
            }
        }
        /// <summary>  
        /// 对一个坐标点按照一个中心进行旋转  
        /// </summary>  
        /// <param name="center">中心点</param>  
        /// <param name="p">要旋转的点</param>  
        /// <param name="angle">旋转角度，笛卡尔直角坐标</param>  
        /// <returns></returns>  
        public static Vector2 RotatePoint2(Vector2 p, Vector2 center, float angle)
        {
            Vector2 tmp = new Vector2();
            float ca = Cos(angle);
            float sa = Sin(angle);
            float dx = p.X - center.X;
            float dy = p.Y - center.Y;
            double x1 = dx * ca + dy* sa + center.X;
            double y1 = -dx * sa + dy * ca + center.Y;
            tmp.X = (int)x1;
            tmp.Y = (int)y1;
            return tmp;
        }
        public static Vector2[] RotatePoint2(Vector2[] p, Vector2 center, float angle)
        {
            int c = p.Length;
            Vector2[] tmp = new Vector2[c];
            float ca = Cos(angle);
            float sa = Sin(angle);
            float x = center.X;
            float y = center.Y;
            for(int i=0;i<c;i++)
            {
                float dx = p[i].X - x;
                float dy = p[i].Y - x;
                double x1 = dx * ca + dy * sa + x;
                double y1 = -dx * sa + dy * ca + y;
                tmp[i].X = (int)x1;
                tmp[i].Y = (int)y1;
            }
            return tmp;
        }

        public static bool PToP2(Vector2[] A, Vector2[] B)
        {
            //Cos A=(b²+c²-a²)/2bc
            float min1 = 0, max1 = 0, min2 = 0, max2 = 0;
            int second = 0;
            Vector2 a, b;
            label1:
            for (int i = 0; i < A.Length; i++)
            {
                int id;
                a = A[i];
                if (i == A.Length - 1)
                {
                    b = A[0];
                    id = 1;
                }
                else
                {
                    b = A[i + 1];
                    id = i + 2;
                }
                float x = a.X - b.X;
                float y = a.Y - b.Y;//向量
                a.X = y;
                a.Y = -x;//法线点a
                b.X = -y;
                b.Y = x;//法线点b
                        // float ab = (x + x) * (x + x) + (y + y) * (y + y);//b 平方
                        //x = c.x - a.x;
                        //y = c.y - a.y;
                float ac;// = x * x + y * y;//c 平方
                //x = b.x - c.x;
                //y = b.y - c.y;
                float bc;// = x * x + y * y;//a 平方
                //float d = Mathf.Sqrt(bc) + Mathf.Sqrt(ac) - Mathf.Sqrt(ab);
                float d;// = ac - bc;
                //min1 = d;
                //max1 = d;
                for (int o = 0; o < A.Length; o++)
                {
                    float x1 = A[o].X - a.X;
                    x1 *= x1;
                    float y1 = A[o].Y - a.Y;
                    ac = x1 + y1 * y1;//ac
                    float x2 = b.X - A[o].X;
                    x2 *= x2;
                    float y2 = b.Y - A[o].Y;
                    bc = x2 + y2 * y2;//bc
                    d = ac - bc;//ab+ac-bc
                    if (o == 0)
                    {
                        min1 = max1 = d;
                    }
                    else
                    {
                        if (d < min1)
                            min1 = d;
                        else if (d > max1)
                            max1 = d;
                    }
                }
                for (int o = 0; o < B.Length; o++)
                {
                    float x1 = B[o].X - a.X;
                    x1 *= x1;
                    float y1 = B[o].Y - a.Y;
                    ac = x1 + y1 * y1;//ac
                    float x2 = b.X - B[o].X;
                    x2 *= x2;
                    float y2 = b.Y - B[o].Y;
                    bc = x2 + y2 * y2;//bc
                    d = ac - bc;//ab+ac-bc
                    if (o == 0)
                        max2 = min2 = d;
                    else
                    {
                        if (d < min2)
                            min2 = d;
                        else if (d > max2)
                            max2 = d;
                    }
                }
                if (min2 > max1 | min1 > max2)
                    return false;
            }
            second++;
            if (second < 2)
            {
                Vector2[] temp = A;
                A = B;
                B = temp;
                goto label1;
            }
            return true;
        }
        public static bool PToP2A(Vector2[] A, Vector2[] B, ref Vector3 location)
        {
            //formule
            //A.x+x1*V1.x=B.x+x2*V2.x
            //x2*V2.x=A.x+x1*V1.x-B.x
            //x2=(A.x+x1*V1.x-B.x)/V2.x
            //A.y+x1*V1.y=B.y+x2*V2.y
            //A.y+x1*V1.y=B.y+(A.x+x1*V1.x-B.x)/V2.x*V2.y
            //x1*V1.y=B.y+(A.x-B.x)/V2.x*V2.y-A.y+x1*V1.x/V2.x*V2.y
            //x1*V1.y-x1*V1.x/V2.x*V2.y=B.y+(A.x-B.x)/V2.x*V2.y-A.y
            //x1*(V1.y-V1.x/V2.x*V2.y)=B.y+(A.x-B.x)/V2.x*V2.y-A.y
            //x1=(B.y-A.y+(A.x-B.x)/V2.x*V2.y)/(V1.y-V1.x/V2.x*V2.y)
            //改除为乘防止除0溢出
            //if((V1.y*V2.x-V1.x*V2.y)==0) 平行
            //x1=((B.y-A.y)*V2.x+(A.x-B.x)*V2.y)/(V1.y*V2.x-V1.x*V2.y)
            //x2=(A.x+x1*V1.x-B.x)/V2.x
            //x2=(A.y+x1*V1.y-B.y)/V2.y
            //if(x1>=0&x1<=1 &x2>=0 &x2<=1) cross =true
            //location.x=A.x+x1*V1.x
            //location.y=A.x+x1*V1.y
            Vector2[] VB = new Vector2[B.Length];
            for (int i = 0; i < B.Length; i++)
            {
                if (i == B.Length - 1)
                {
                    VB[i].X = B[0].X - B[i].X;
                    VB[i].Y = B[0].Y - B[i].Y;
                }
                else
                {
                    VB[i].X = B[i + 1].X - B[i].X;
                    VB[i].Y = B[i + 1].Y - B[i].Y;
                }
            }
            for (int i = 0; i < A.Length; i++)
            {
                Vector2 VA = new Vector2();
                if (i == A.Length - 1)
                {
                    VA.X = A[0].X - A[i].X;
                    VA.Y = A[0].Y - A[i].Y;
                }
                else
                {
                    VA.X = A[i + 1].X - A[i].X;
                    VA.Y = A[i + 1].Y - A[i].Y;
                }
                for (int c = 0; c < B.Length; c++)
                {
                    //(V1.y*V2.x-V1.x*V2.y)
                    float y = VA.Y * VB[c].X - VA.X * VB[c].Y;
                    if (y == 0)
                        break;
                    //((B.y-A.y)*V2.x+(A.x-B.x)*V2.y)
                    float x = (B[c].Y - A[i].Y) * VB[c].X + (A[i].X - B[c].X) * VB[c].Y;
                    float d = x / y;
                    if (d >= 0 & d <= 1)
                    {
                        if (VB[c].X == 0)
                        {
                            //x2=(A.y+x1*V1.y-B.y)/V2.y
                            y = (A[i].Y - B[c].Y + d * VA.Y) / VB[c].Y;
                        }
                        else
                        {
                            //x2=(A.x+x1*V1.x-B.x)/V2.x
                            y = (A[i].X - B[c].X + d * VA.X) / VB[c].X;
                        }
                        //location.x=A.x+x1*V1.x
                        //location.y=A.x+x1*V1.y
                        if (y >= 0 & y <= 1)
                        {
                            location.X = A[i].X + d * VA.X;
                            location.Y = A[i].Y + d * VA.Y;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static bool PToP2A(Vector2[] A, Vector2[] B, ref Vector3 la, ref Vector3 lb)
        {
            //formule
            //A.x+x1*V1.x=B.x+x2*V2.x
            //x2*V2.x=A.x+x1*V1.x-B.x
            //x2=(A.x+x1*V1.x-B.x)/V2.x
            //A.y+x1*V1.y=B.y+x2*V2.y
            //A.y+x1*V1.y=B.y+(A.x+x1*V1.x-B.x)/V2.x*V2.y
            //x1*V1.y=B.y+(A.x-B.x)/V2.x*V2.y-A.y+x1*V1.x/V2.x*V2.y
            //x1*V1.y-x1*V1.x/V2.x*V2.y=B.y+(A.x-B.x)/V2.x*V2.y-A.y
            //x1*(V1.y-V1.x/V2.x*V2.y)=B.y+(A.x-B.x)/V2.x*V2.y-A.y
            //x1=(B.y-A.y+(A.x-B.x)/V2.x*V2.y)/(V1.y-V1.x/V2.x*V2.y)
            //改除为乘防止除0溢出
            //if((V1.y*V2.x-V1.x*V2.y)==0) 平行
            //x1=((B.y-A.y)*V2.x+(A.x-B.x)*V2.y)/(V1.y*V2.x-V1.x*V2.y)
            //x2=(A.x+x1*V1.x-B.x)/V2.x
            //x2=(A.y+x1*V1.y-B.y)/V2.y
            //if(x1>=0&x1<=1 &x2>=0 &x2<=1) cross =true
            //location.x=A.x+x1*V1.x
            //location.y=A.x+x1*V1.y
            bool re = false;
            Vector2[] VB = new Vector2[B.Length];
            for (int i = 0; i < B.Length; i++)
            {
                if (i == B.Length - 1)
                {
                    VB[i].X = B[0].X - B[i].X;
                    VB[i].Y = B[0].Y - B[i].Y;
                }
                else
                {
                    VB[i].X = B[i + 1].X - B[i].X;
                    VB[i].Y = B[i + 1].Y - B[i].Y;
                }
            }
            for (int i = 0; i < A.Length; i++)
            {
                Vector2 VA = new Vector2();
                if (i == A.Length - 1)
                {
                    VA.X = A[0].X - A[i].X;
                    VA.Y = A[0].Y - A[i].Y;
                }
                else
                {
                    VA.X = A[i + 1].X - A[i].X;
                    VA.Y = A[i + 1].Y - A[i].Y;
                }
                for (int c = 0; c < B.Length; c++)
                {
                    //(V1.y*V2.x-V1.x*V2.y)
                    float y = VA.Y * VB[c].X - VA.X * VB[c].Y;
                    if (y == 0)
                        break;
                    //((B.y-A.y)*V2.x+(A.x-B.x)*V2.y)
                    float x = (B[c].Y - A[i].Y) * VB[c].X + (A[i].X - B[c].X) * VB[c].Y;
                    float d = x / y;
                    if (d >= 0 & d <= 1)
                    {
                        if (VB[c].X == 0)
                        {
                            //x2=(A.y+x1*V1.y-B.y)/V2.y
                            y = (A[i].Y - B[c].Y + d * VA.Y) / VB[c].Y;
                        }
                        else
                        {
                            //x2=(A.x+x1*V1.x-B.x)/V2.x
                            y = (A[i].X - B[c].X + d * VA.X) / VB[c].X;
                        }
                        //location.x=A.x+x1*V1.x
                        //location.y=A.x+x1*V1.y
                        if (y >= 0 & y <= 1)
                        {
                            if (re)
                            {
                                lb.X = A[i].X + d * VA.X;
                                lb.Y = A[i].Y + d * VA.Y;
                                return true;
                            }
                            else
                            {
                                la.X = A[i].X + d * VA.X;
                                la.Y = A[i].Y + d * VA.Y;
                                re = true;
                            }
                        }
                    }
                }
            }
            return re;
        }
        public static bool PToP3(Vector3[] A, Vector3[] B)
        {
            //Cos A=(b²+c²-a²)/2bc
            float min1 = 0, max1 = 0, min2 = 0, max2 = 0;
            int second = 0;
            Vector3 a, b;
            label1:
            for (int i = 0; i < A.Length; i++)
            {
                int id;
                a = A[i];
                if (i == A.Length - 1)
                {
                    b = A[0];
                    id = 1;
                }
                else
                {
                    b = A[i + 1];
                    id = i + 2;
                }
                float x = a.X - b.X;
                float y = a.Y - b.Y;//向量
                a.X = y;
                a.Y = -x;//法线点a
                b.X = -y;
                b.Y = x;//法线点b
                        // float ab = (x + x) * (x + x) + (y + y) * (y + y);//b 平方
                        //x = c.x - a.x;
                        //y = c.y - a.y;
                float ac;// = x * x + y * y;//c 平方
                //x = b.x - c.x;
                //y = b.y - c.y;
                float bc;// = x * x + y * y;//a 平方
                //float d = Mathf.Sqrt(bc) + Mathf.Sqrt(ac) - Mathf.Sqrt(ab);
                float d;// = ac - bc;
                //min1 = d;
                //max1 = d;
                for (int o = 0; o < A.Length; o++)
                {
                    float x1 = A[o].X - a.X;
                    x1 *= x1;
                    float y1 = A[o].Y - a.Y;
                    ac = x1 + y1 * y1;//ac
                    float x2 = b.X - A[o].X;
                    x2 *= x2;
                    float y2 = b.Y - A[o].Y;
                    bc = x2 + y2 * y2;//bc
                    d = ac - bc;//ab+ac-bc
                    if (o == 0)
                    {
                        min1 = max1 = d;
                    }
                    else
                    {
                        if (d < min1)
                            min1 = d;
                        else if (d > max1)
                            max1 = d;
                    }
                }
                for (int o = 0; o < B.Length; o++)
                {
                    float x1 = B[o].X - a.X;
                    x1 *= x1;
                    float y1 = B[o].Y - a.Y;
                    ac = x1 + y1 * y1;//ac
                    float x2 = b.X - B[o].X;
                    x2 *= x2;
                    float y2 = b.Y - B[o].Y;
                    bc = x2 + y2 * y2;//bc
                    d = ac - bc;//ab+ac-bc
                    if (o == 0)
                        max2 = min2 = d;
                    else
                    {
                        if (d < min2)
                            min2 = d;
                        else if (d > max2)
                            max2 = d;
                    }
                }
                if (min2 > max1 | min1 > max2)
                    return false;
            }
            second++;
            if (second < 2)
            {
                Vector3[] temp = A;
                A = B;
                B = temp;
                goto label1;
            }
            return true;
        }
        public static bool TriangleToPolygon(Vector3[] A, Vector3[] B)
        {
            Vector3[] a = new Vector3[3]
            {
            new Vector3(A[1].X - A[0].X, A[1].Y - A[0].Y,0),
            new Vector3(A[2].X - A[1].X, A[2].Y - A[1].Y,0),
            new Vector3(A[0].X - A[2].X, A[0].Y - A[2].Y,0)
            };
            int again = 0;
            label1:
            for (int i = 0; i < a.Length; i++)
            {
                float min1 = 1000, min2 = 1000, max1 = 0, max2 = 0;
                float sxy = a[i].X * a[i].X + a[i].Y * a[i].Y;
                for (int l = 0; l < 3; l++)
                {
                    float dxy = A[l].X * a[i].X + A[l].Y * a[i].Y;
                    float x = dxy / sxy * a[i].X;
                    if (x < 0)
                        x = 0 - x;
                    float y = dxy / sxy * a[i].Y;
                    if (y < 0)
                        y = 0 - y;
                    x = x + y;
                    if (x > max1)
                        max1 = x;
                    if (x < min1)
                        min1 = x;
                }
                for (int l = 0; l < B.Length; l++)
                {
                    float dxy = B[l].X * a[i].X + B[l].Y * a[i].Y;
                    float x = dxy / sxy * a[i].X;
                    if (x < 0)
                        x = 0 - x;
                    float y = dxy / sxy * a[i].Y;
                    if (y < 0)
                        y = 0 - y;
                    x = x + y;
                    if (x > max2)
                        max2 = x;
                    if (x < min2)
                        min2 = x;
                }
                if (min1 > max2 | min2 > max1)
                {
                    return false;
                }
            }
            if (again > 0)
                return true;
            a = new Vector3[B.Length];
            for (int i = 0; i < B.Length - 1; i++)
            {
                a[i].X = B[i + 1].X - B[i].X;
                a[i].Y = B[i + 1].Y - B[i].Y;
            }
            a[a.Length - 1].X = B[0].X - B[a.Length - 1].X;
            a[a.Length - 1].Y = B[0].Y - B[a.Length - 1].Y;
            again++;
            goto label1;
        }
        public static bool CircleToPolygon(Vector2 C, float r, Vector2[] P)
        {
            Vector2 A = new Vector2();
            Vector2 B = new Vector2();
            float z = 10, r2 = r * r, x = 0, y = 0;
            float[] d = new float[P.Length];
            int id = 0;
            for (int i = 0; i < P.Length; i++)
            {
                x = C.X - P[i].X;
                y = C.Y - P[i].Y;
                x = x * x + y * y;
                if (x <= r2)
                    return true;
                d[i] = x;
                if (x < z)
                {
                    z = x;
                    id = i;
                }
            }
            int p1 = id - 1;
            if (p1 < 0)
                p1 = P.Length - 1;
            float a, b, c;
            c = d[p1];
            a = d[id];
            B = P[id];
            A = P[p1];
            x = B.X - A.X;
            x *= x;
            y = B.Y - A.Y;
            y *= y;
            b = x + y;
            x = c - a;
            if (x < 0)
                x = -x;
            if (x <= b)
            {
                y = b + c - a;
                y = y * y / 4 / b;
                if (c - y <= r2)
                    return true;
            }
            else
            {
                p1 = id + 1;
                if (p1 == P.Length)
                    p1 = 0;
                c = d[p1];
                A = P[p1];
                x = B.X - A.X;
                x *= x;
                y = B.Y - A.Y;
                y *= y;
                b = x + y;
                x = c - a;
                if (x < 0)
                    x = -x;
                if (x <= b)
                {
                    y = b + c - a;
                    y = y * y / 4 / b;
                    if (c - y <= r2)
                        return true;
                }
            }
            return DotToPolygon(P, new Vector2(C.X, C.Y));//circle inside polygon
        }
        public static bool CircleToPolygon(Vector2 C, float r, ref Vector2[] P)
        {
            Vector2 A = new Vector2();
            Vector2 B = new Vector2();
            float z = 10, r2 = r * r, x = 0, y = 0;
            float[] d = new float[P.Length];
            int id = 0;
            for (int i = 0; i < P.Length; i++)
            {
                x = C.X - P[i].X;
                y = C.Y - P[i].Y;
                x = x * x + y * y;
                if (x <= r2)
                    return true;
                d[i] = x;
                if (x < z)
                {
                    z = x;
                    id = i;
                }
            }
            int p1 = id - 1;
            if (p1 < 0)
                p1 = P.Length - 1;
            float a, b, c;
            c = d[p1];
            a = d[id];
            B = P[id];
            A = P[p1];
            x = B.X - A.X;
            x *= x;
            y = B.Y - A.Y;
            y *= y;
            b = x + y;
            x = c - a;
            if (x < 0)
                x = -x;
            if (x <= b)
            {
                y = b + c - a;
                y = y * y / 4 / b;
                if (c - y <= r2)
                    return true;
            }
            else
            {
                p1 = id + 1;
                if (p1 == P.Length)
                    p1 = 0;
                c = d[p1];
                A = P[p1];
                x = B.X - A.X;
                x *= x;
                y = B.Y - A.Y;
                y *= y;
                b = x + y;
                x = c - a;
                if (x < 0)
                    x = -x;
                if (x <= b)
                {
                    y = b + c - a;
                    y = y * y / 4 / b;
                    if (c - y <= r2)
                        return true;
                }
            }
            return DotToPolygon(P, C);//circle inside polygon
        }
        public static bool CircleToLine(Vector2 C, float r, Vector2 A, Vector2 B)
        {
            float vx1 = C.X - A.X;
            float vy1 = C.Y - A.Y;
            float vx2 = B.X - A.X;
            float vy2 = B.Y - A.Y;
            float len = SquareRootFloat(vx2 * vx2 + vy2 * vy2);
            vx2 /= len;
            vy2 /= len;
            float u = vx1 * vx2 + vy1 * vy2;
            float x0 = 0f;
            float y0 = 0f;
            if (u <= 0)
            {
                x0 = A.X;
                y0 = A.Y;
            }
            else if (u >= len)
            {
                x0 = B.X;
                y0 = B.Y;
            }
            else
            {
                x0 = A.X + vx2 * u;
                y0 = A.Y + vy2 * u;
            }
            return (C.X - x0) * (C.X - x0) + (C.Y - y0) * (C.Y - y0) <= r * r;
        }
        public static bool CircleToLineA(Vector2 C, float r, Vector2 A, Vector2 B)
        {
            r *= r;
            float x = C.X - B.X;
            x *= x;
            float y = C.Y - B.Y;
            y *= y;
            float a = x + y;
            if (a <= r)
                return true;
            x = A.X - C.X;
            x *= x;
            y = A.Y - C.Y;
            y *= y;
            float c = x + y;
            if (c <= r)
                return true;
            x = B.X - A.X;
            x *= x;
            y = B.Y - A.Y;
            y *= y;
            float b = x + y;
            x = c - a;
            if (x < 0)
                x = -x;
            if (x > b)
                return false;
            y = b + c - a;
            y *= y / 4 / b;
            if (c - y <= r)
                return true;
            return false;
        }
        #endregion

        public static float Aim(ref Vector2 self, ref Vector3 v)
        {
            float x = v.X - self.X;
            float y = v.Y - self.Y;
            return atan(x, y);
        }
        public static float Aim(ref Vector3 self, ref Vector3 v)
        {
            float x = v.X - self.X;
            float y = v.Y - self.Y;
            return atan(x, y);
        }
        public static void RotationVectorArray(ref Vector4[] Raw,ref Vector3[] Vertical,Vector3 location,Vector3 rotation,float scale, int count)
        {
            //x=√(d²/((sin a)²+(sin b)²+(sin c)²))
            float x = location.X;
            float y = location.Y;
            float z = location.Z;
            float a = rotation.X;
            float b = rotation.Y;
            float c = rotation.Z;
            for(int i=0;i<count;i++)
            {
                int t = (int)Raw[i].X+(int)a;
                if (t > 360)
                    t -= 360;
                float a1 = Sin(t);
                float s= a1 * a1;
                t = (int)Raw[i].Y + (int)b;
                t += 90;
                if (t > 360)
                    t -= 360;
                float a2 = Sin(t);
                s += a2 *a2;
                t = (int)Raw[i].Z + (int)c;
                if (t > 360)
                    t -= 360;
                float a3 = Sin(t);
                s += a3 * a3;
                float d = scale * Raw[i].W;
                d *= d;
                d /= s;
                d = SquareRootFloat(d);
                Vertical[i].X = x + a3 * d;
                Vertical[i].Y = y + a1 * d;
                Vertical[i].Z = z + a2 * d;
            }
        }
        unsafe public static void RotationVectorArray(ref Vector4[] Raw, byte* bp, int offset, int structsize, Vector3 location,
            Vector3 rotation, float scale, int count)
        {
            //x=√(d²/((sin a)²+(sin b)²+(sin c)²))
            float x = location.X;
            float y = location.Y;
            float z = location.Z;
            float a = rotation.X;
            float b = rotation.Y;
            float c = rotation.Z;
            bp += offset;
            for (int i = 0; i < count; i++)
            {
                Vector3* vp = (Vector3*)bp;
                int t = (int)Raw[i].X + (int)a;
                if (t > 360)
                    t -= 360;
                float a1 = Sin(t);
                float s = a1 * a1;
                t = (int)Raw[i].Y + (int)b;
                t += 90;
                if (t > 360)
                    t -= 360;
                float a2 = Sin(t);
                s += a2 * a2;
                t = (int)Raw[i].Z + (int)c;
                if (t > 360)
                    t -= 360;
                float a3 = Sin(t);
                s += a3 * a3;
                float d = scale * Raw[i].W;
                d *= d;
                d /= s;
                d = SquareRootFloat(d);
                vp->X = x + a3 * d;
                vp->Y = y + a1 * d;
                vp->Z = z + a2 * d;
                bp += structsize;
            }
        }
    }
}
