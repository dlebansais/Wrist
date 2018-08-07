using System;

namespace Windows.UI.Xaml.Media
{
    public struct Matrix
    {
        public Boolean IsIdentity { get; set; }
        public Double M11 { get; set; }
        public Double M12 { get; set; }
        public Double M21 { get; set; }
        public Double M22 { get; set; }
        public Double OffsetX { get; set; }
        public Double OffsetY { get; set; }
    }
}
