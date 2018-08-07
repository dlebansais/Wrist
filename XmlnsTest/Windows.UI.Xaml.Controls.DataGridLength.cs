using System;

namespace Windows.UI.Xaml.Controls
{
    public struct DataGridLength
    {
        public Boolean IsAbsolute { get; set; }
        public Boolean IsAuto { get; set; }
        public Boolean IsStar { get; set; }
        public DataGridLengthUnitType UnitType { get; set; }
        public Double Value { get; set; }
    }
}
