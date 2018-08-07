using System;

namespace Windows.UI.Xaml
{
    public struct GridLength
    {
        public GridUnitType GridUnitType { get; set; }
        public Boolean IsAbsolute { get; set; }
        public Boolean IsAuto { get; set; }
        public Boolean IsStar { get; set; }
        public Double Value { get; set; }
    }
}
