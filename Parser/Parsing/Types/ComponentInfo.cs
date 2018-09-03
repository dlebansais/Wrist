namespace Parser
{
    public class ComponentInfo
    {
        public IDeclarationSource NameSource;
        public IDeclarationSource FixedValueSource;
        public IDeclarationSource ObjectSource;
        public IDeclarationSource MemberSource;
        public IDeclarationSource KeySource;

        public static ComponentInfo Parse(IParsingSourceStream sourceStream, string infoText)
        {
            IDeclarationSource NameSource;
            string MemberValue;
            ParserDomain.ParseStringPair(sourceStream, infoText, '=', out NameSource, out MemberValue);

            if (!MemberValue.Contains("."))
                return new ComponentInfo() { NameSource = NameSource, FixedValueSource = new DeclarationSource(MemberValue, sourceStream), ObjectSource = null, MemberSource = null, KeySource = null };

            else
            {
                IDeclarationSource ObjectSource;
                string MemberName;
                ParserDomain.ParseStringPair(sourceStream, MemberValue, '.', out ObjectSource, out MemberName);

                string Key;
                int StartIndex = MemberName.IndexOf("[");
                int EndIndex = MemberName.IndexOf("]");
                IDeclarationSource KeySource;
                if (StartIndex > 0 && EndIndex > StartIndex)
                {
                    Key = MemberName.Substring(StartIndex + 1, EndIndex - StartIndex - 1);
                    MemberName = MemberName.Substring(0, StartIndex);
                    KeySource = new DeclarationSource(Key, sourceStream);
                }
                else
                    KeySource = null;

                return new ComponentInfo() { NameSource = NameSource, FixedValueSource = null, ObjectSource = ObjectSource, MemberSource = new DeclarationSource(MemberName, sourceStream), KeySource = KeySource };
            }
        }
    }
}
