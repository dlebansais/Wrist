using Windows.UI.Xaml;
using System;

namespace Windows.UI.Xaml.Controls
{
    public class MediaElement : FrameworkElement
    {
        public Boolean AutoPlay { get; set; }
        public Boolean IsAudioOnly { get; set; }
        public Boolean IsLooping { get; set; }
        public Boolean IsMuted { get; set; }
        public Uri Source { get; set; }
        public Double Volume { get; set; }
        public Boolean ShowControls { get; set; }
    }
}
