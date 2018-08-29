using System;
using Windows.UI.Xaml;

namespace DatabaseManager
{
#if HTTP
#else
    public delegate void DownloadStringCompletedEventHandler(object sender, DownloadStringCompletedEventArgs e);

    public class DownloadStringCompletedEventArgs
    {
        public DownloadStringCompletedEventArgs(string result)
        {
            Result = result;
        }

        public string Result { get; private set; }
    }

    public class WebClient
    {
        public WebClient()
        {
            DatabaseTimer = new DispatcherTimer();
            DatabaseTimer.Interval = TimeSpan.FromSeconds(1);
            DatabaseTimer.Tick += OnTick;
        }

        private DispatcherTimer DatabaseTimer;

        public event DownloadStringCompletedEventHandler DownloadStringCompleted;

        public void DownloadStringAsync(Uri address)
        {
            Address = address;
            DatabaseTimer.Start();
        }

        private Uri Address;

        private void OnTick(object sender, object e)
        {
            DispatcherTimer DatabaseTimer = (DispatcherTimer)sender;
            DatabaseTimer.Stop();

            string Result = OperationHandler.Execute(Address.OriginalString);
            Address = null;

            DownloadStringCompleted?.Invoke(this, new DownloadStringCompletedEventArgs(Result));
        }
    }
#endif
}
