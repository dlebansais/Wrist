#if HTTP
using CSHTML5;
#endif
using System.Collections.Generic;

namespace DatabaseManager
{
    public class NetTools
    {
        public static object GetDocumentUrl()
        {
#if HTTP
            return Interop.ExecuteJavaScript("document.URL");
#else
            return null;
#endif
        }

        public static Dictionary<string, string> GetQueryString()
        {
            Dictionary<string, string> Result = new Dictionary<string, string>();

            object Url = GetDocumentUrl();
            if (Url != null)
            {
                string UrlAsString = Url.ToString();

                int StartIndex;
                if ((StartIndex = UrlAsString.IndexOf('?')) >= 0)
                {
                    string QueryString = UrlAsString.Substring(StartIndex + 1);
                    foreach (string KeyValuePair in QueryString.Split('&'))
                    {
                        string Key, Value;

                        string[] Splitted = KeyValuePair.Split('=');
                        if (Splitted.Length >= 2)
                        {
                            Key = Splitted[0];
                            Value = Splitted[1];
                            for (int i = 2; i < Splitted.Length; i++)
                                Value += "=" + Splitted[i];
                        }
                        else
                        {
                            Key = KeyValuePair;
                            Value = null;
                        }

                        Result.Add(Key, Value);
                    }
                }
            }

            return Result;
        }
    }
}
