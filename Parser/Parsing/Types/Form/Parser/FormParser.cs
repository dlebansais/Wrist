namespace Parser
{
    public abstract class FormParser<T> : IFormParser<T>
        where T : class, IForm
    {
        public FormParser(string folderName, string extension)
        {
            FolderName = folderName;
            Extension = extension;
        }

        public void InitResult()
        {
            ParsedResult = new FormCollection<T>();
        }

        public string FolderName { get; private set; }
        public string Extension { get; private set; }
        public IFormCollection<T> ParsedResult { get; private set; }
        IFormCollection IFormParser.ParsedResult { get { return ParsedResult; } }
        public abstract T Parse(string fileName);
        IForm IFormParser.Parse(string fileName) { return Parse(fileName); }
    }
}
