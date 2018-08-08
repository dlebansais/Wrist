using System;
using System.IO;
using Windows.UI.Xaml.Media.Imaging;

namespace Presentation
{
    public class FileBitmapImage : IDisposable
    {
        public FileBitmapImage(IDisposable source, Stream fs)
        {
            Source = source;
            Fs = fs;

            BitmapImage = new BitmapImage();
            BitmapImage.SetSource(fs);
        }

        public BitmapImage BitmapImage { get; private set; }

        private IDisposable Source;
        private Stream Fs;

        public void Dispose()
        {
            Fs.Dispose();
            Source.Dispose();
        }
    }
}
