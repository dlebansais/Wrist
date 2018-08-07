using System.Windows.Markup;

namespace Windows.UI.Xaml.Media.Animation
{
    [ContentProperty("KeyFrames")]
    public class ObjectAnimationUsingKeyFrames : Timeline
    {
        public ObjectKeyFrameCollection KeyFrames { get; set; } = new ObjectKeyFrameCollection();
    }
}
