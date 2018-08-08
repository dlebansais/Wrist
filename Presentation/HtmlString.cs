namespace Presentation
{
    public static class HtmlString
    {
        public static string Entities(string s)
        {
            s = s.Replace(" ", "%20");

            return s;
        }
    }
}
