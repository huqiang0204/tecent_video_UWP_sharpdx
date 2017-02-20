using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct2D1;
using SharpDX.WIC;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.IO;

namespace DX
{
   public  class Resource
    {
        public struct WICBitmapSource
        {
            public BitmapDecoder Decoder;
            public BitmapFrameDecode FrameDecoder;
            public FormatConverter Converter;
        }
        public static async Task<IRandomAccessStream> LoadApplictionResource(string name)
        {
            Uri u = new Uri("ms-appx:///"+name);
            RandomAccessStreamReference rass = RandomAccessStreamReference.CreateFromUri(u);
            return await rass.OpenReadAsync();
        }
        public static WICBitmapSource CreateWicBitmap(Stream str)
        {
            WICBitmapSource bs = new WICBitmapSource();
            ImagingFactory fac = ThreadResource.GetWicFactory();
            bs.Decoder = new BitmapDecoder(fac, str, DecodeOptions.CacheOnLoad);
            bs.FrameDecoder = bs.Decoder.GetFrame(0);
            bs.Converter = new FormatConverter(fac);
            bs.Converter.Initialize(bs.FrameDecoder, SharpDX.WIC.PixelFormat.Format32bppPRGBA);
            return bs;
        }
        public static SharpDX.Direct2D1.Bitmap CreateD2dBitmap(SharpDX.Direct2D1.DeviceContext context, FormatConverter fconv)
        {
            return SharpDX.Direct2D1.Bitmap.FromWicBitmap(context, fconv);
        }
        public static Texture2D CreateTex2DFromBitmap(SharpDX.WIC.BitmapSource bsource, SharpDX.Direct3D11.Device device)
        {
            Texture2DDescription desc;
            desc.Width = bsource.Size.Width;
            desc.Height = bsource.Size.Height;
            desc.ArraySize = 1;
            desc.BindFlags = BindFlags.ShaderResource;
            desc.Usage = ResourceUsage.Default;
            desc.CpuAccessFlags = CpuAccessFlags.None;
            desc.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
            desc.MipLevels = 1;
            desc.OptionFlags = ResourceOptionFlags.None;
            desc.SampleDescription.Count = 1;
            desc.SampleDescription.Quality = 0;

            using (DataStream s = new DataStream(bsource.Size.Height * bsource.Size.Width * 4, true, true))
            {
                bsource.CopyPixels(bsource.Size.Width * 4, s);
                DataRectangle rect = new DataRectangle(s.DataPointer, bsource.Size.Width * 4);
                return new Texture2D(device, desc, rect);
            }
        }
        public static void CreateSharder()
        {

        }
    }
}
