using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Presentation
{
    public class ControlTools
    {
        public static void ChangeEnabledStyleOrColor(Control AsControl, bool IsEnabled, Dictionary<Control, Brush> BrushTable, Dictionary<Control, Style> StyleTable, ResourceDictionary Resources)
        {
            string StyleKey = null;
            foreach (object Key in Resources.Keys)
                if ((Key is string KeyString) && (Resources[Key] is Style AsStyle) && (AsStyle == AsControl.Style))
                {
                    StyleKey = KeyString;
                    break;
                }

            if (IsEnabled)
            {
                if (BrushTable.ContainsKey(AsControl))
                    AsControl.Foreground = BrushTable[AsControl];
                else if (StyleTable.ContainsKey(AsControl))
                    AsControl.Style = StyleTable[AsControl] as Style;
                else if (StyleKey == null)
                    BrushTable.Add(AsControl, AsControl.Foreground);
                else
                    StyleTable.Add(AsControl, AsControl.Style);
            }
            else
            {
                Style DisabledStyle = null;
                if (StyleKey != null)
                {
                    string DisabledStyleKey = StyleKey + "Disabled";
                    if (Resources.ContainsKey(DisabledStyleKey))
                        DisabledStyle = Resources[DisabledStyleKey] as Style;
                }

                if (DisabledStyle != null)
                {
                    if (!StyleTable.ContainsKey(AsControl))
                        StyleTable.Add(AsControl, AsControl.Style);

                    AsControl.Style = DisabledStyle;
                }
                else
                {
                    if (!BrushTable.ContainsKey(AsControl))
                        BrushTable.Add(AsControl, AsControl.Foreground);

                    AsControl.Foreground = new SolidColorBrush(Colors.Gray);
                }
            }
        }
    }
}
