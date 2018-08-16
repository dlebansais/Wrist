namespace Parser
{
    public class TextDecoration : LayoutElement, ITextDecoration
    {
        public string Text { get; set; }

        public override string ToString()
        {
            return $"{GetType().Name} \"{Text}\"";
        }
    }
}
