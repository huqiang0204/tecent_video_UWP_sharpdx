using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace DX
{
    public class BindingMod
    {
        public UIPanel Parent;
    }
    public struct BindingElement
    {
        public UIElement UI;
        public Action<object,object> SetData;
        public Vector2 RawLocation;
    }
}
