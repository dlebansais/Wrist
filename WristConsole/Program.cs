using Parser;
using System;
using System.IO;
using System.Threading;
using WristManager;

namespace WristConsole
{
    class Program
    {
        static void Main()
        {
            MainManager MainManager = new MainManager();

            string[] Args = Environment.GetCommandLineArgs();
            string LaunchFolder = System.IO.Path.GetDirectoryName(Args[0]);
            if (LaunchFolder.ToLower().EndsWith("\\bin\\debug"))
                LaunchFolder = LaunchFolder.Substring(0, LaunchFolder.Length - 10);
            else if (LaunchFolder.ToLower().EndsWith("\\bin\\x64\\debug"))
                LaunchFolder = LaunchFolder.Substring(0, LaunchFolder.Length - 14);
            else if (LaunchFolder.ToLower().EndsWith("\\bin\\release"))
                LaunchFolder = LaunchFolder.Substring(0, LaunchFolder.Length - 12);
            else if (LaunchFolder.ToLower().EndsWith("\\bin\\x64\\release"))
                LaunchFolder = LaunchFolder.Substring(0, LaunchFolder.Length - 16);

            string ExpectedProjectName = "WristConsole";
            if (LaunchFolder.ToLower().EndsWith($"\\{ExpectedProjectName.ToLower()}"))
                LaunchFolder = LaunchFolder.Substring(0, LaunchFolder.Length - ExpectedProjectName.Length - 1);

            string InputFolder;
            if (Args.Length > 1)
                InputFolder = Path.GetFullPath(Args[1]);
            else
                InputFolder = Path.Combine(LaunchFolder, "Samples", "comet");

            string OutputFolder;
            if (Args.Length > 2)
                OutputFolder = Path.GetFullPath(Args[2]);
            else
                OutputFolder = Path.Combine(LaunchFolder, "AppCSHtml5");

            string HomePageName;
            if (Args.Length > 3)
                HomePageName = Args[3];
            else
                HomePageName = "home";

            string ColorThemeName;
            if (Args.Length > 4)
                ColorThemeName = Args[4];
            else
                ColorThemeName = "default";

            string UnitTestName;
            if (Args.Length > 5)
                UnitTestName = Args[5];
            else
                UnitTestName = null;

            try
            {
                Console.WriteLine("Building...");
                IDomain Domain;
                MainManager.Build(InputFolder, OutputFolder, HomePageName, ColorThemeName, UnitTestName, out Domain);
                Domain.CheckUnused((string message) => Console.WriteLine(message));
                Console.WriteLine("Done.");
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
            catch (ParsingException e)
            {
                using (Stream ErrorStream = Console.OpenStandardOutput())
                {
                    using (StreamWriter ErrorWriter = new StreamWriter(ErrorStream))
                    {
                        e.WriteDiagnostic(ErrorWriter);
                    }
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
            catch (Exception e)
            {
                using (Stream ErrorStream = Console.OpenStandardOutput())
                {
                    using (StreamWriter ErrorWriter = new StreamWriter(ErrorStream))
                    {
                        ErrorWriter.WriteLine(e.Message);
                        ErrorWriter.WriteLine(e.StackTrace);
                    }
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
        }
    }
}
