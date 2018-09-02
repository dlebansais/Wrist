using System;
using System.IO;
using System.Text;
using System.Xaml;

namespace Parser
{
    public class ParsingSourceStream : IParsingSourceStream
    {
        static ParsingSourceStream()
        {
            CurrentSource = null;
        }

        public static IParsingSourceStream CreateFromFileName(string fileName)
        {
            ParsingSourceStream Result = new ParsingSourceStream();
            Result.FileName = fileName;

            return Result;
        }

        public static IParsingSource GetCurrentSource()
        {
            return CurrentSource?.FreezedPosition();
        }

        private static IParsingSourceStream CurrentSource;

        private ParsingSourceStream()
        {
        }

        public string FileName { get; private set; }
        public string Line { get; private set; }
        public bool EndOfStream { get { return sr.EndOfStream; } }
        public int LineIndex { get; private set; }

        public IParsingSource FreezedPosition()
        {
            return new ParsingSource(FileName, Line, LineIndex);
        }

        public IParsingSourceStream Open()
        {
            fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            sr = new StreamReader(fs, Encoding.UTF8);
            LineIndex = 0;
            Line = null;
            return this;
        }

        public IParsingSourceStream OpenXamlFromFile(XamlSchemaContext context)
        {
            fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            xr = new XamlXmlReader(fs, context);
            LineIndex = 0;
            Line = null;
            return this;
        }

        public IParsingSourceStream OpenXamlFromBytes(byte[] contentBytes, XamlSchemaContext context)
        {
            ms = new MemoryStream(contentBytes);
            xr = new XamlXmlReader(ms, context);
            LineIndex = 0;
            Line = null;
            return this;
        }

        public void Close()
        {
            using (StreamReader _sr = sr)
            {
                sr = null;
            }
            using (XamlXmlReader _xr = xr)
            {
                xr = null;
            }

            using (MemoryStream _ms = ms)
            {
                ms = null;
            }

            using (FileStream _fs = fs)
            {
                fs = null;
            }
        }

        public void ReadLine()
        {
            LineIndex++;
            Line = sr.ReadLine();
        }

        public string ReadToEnd()
        {
            return sr.ReadToEnd();
        }

        public object LoadXaml()
        {
            CurrentSource = this;
            object Result;

            try
            {
                Result = System.Windows.Markup.XamlReader.Load(xr);
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                if (e.InnerException is ParsingException AsParsingException)
                    throw AsParsingException;
                else
                    throw new ParsingException(23, this, e);
            }

            CurrentSource = null;

            return Result;
        }

        private FileStream fs;
        private StreamReader sr;
        private XamlXmlReader xr;
        private MemoryStream ms;

        #region Implementation of IDisposable
        /// <summary>
        ///     Called when an object should release its resources.
        /// </summary>
        /// <param name="isDisposing">Indicates if resources must be disposed now.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
                DisposeNow();
        }

        private void DisposeNow()
        {
            Close();
        }

        /// <summary>
        ///     Called when an object should release its resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Object destructor
        /// </summary>
        ~ParsingSourceStream()
        {
            Dispose(false);
        }
        #endregion
    }
}
