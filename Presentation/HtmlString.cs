namespace Presentation
{
    public static class HtmlString
    {
        public static string PercentEncoded(string s)
        {
            string Result = "";

            foreach (char c in s)
                switch (c)
                {
                    case ' ':
                        Result += "%20";
                        break;

                    case '!':
                        Result += "%21";
                        break;

                    case '#':
                        Result += "%23";
                        break;

                    case '$':
                        Result += "%24";
                        break;

                    case '&':
                        Result += "%26";
                        break;

                    case '\'':
                        Result += "%27";
                        break;

                    case '(':
                        Result += "%28";
                        break;

                    case ')':
                        Result += "%29";
                        break;

                    case '*':
                        Result += "%2A";
                        break;

                    case '+':
                        Result += "%2B";
                        break;

                    case ',':
                        Result += "%2C";
                        break;

                    case '/':
                        Result += "%2F";
                        break;

                    case ':':
                        Result += "%3A";
                        break;

                    case ';':
                        Result += "%3B";
                        break;

                    case '=':
                        Result += "%3D";
                        break;

                    case '?':
                        Result += "%3F";
                        break;

                    case '@':
                        Result += "%40";
                        break;

                    case '[':
                        Result += "%5B";
                        break;

                    case ']':
                        Result += "%5D";
                        break;

                    default:
                        Result += c;
                        break;
                }

            return Result;
        }
    }
}
