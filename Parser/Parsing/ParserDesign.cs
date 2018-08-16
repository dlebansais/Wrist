using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xaml;

namespace Parser
{
    public class ParserDesign : FormParser<IDesign>
    {
        public ParserDesign(string folderName, string extension)
            : base(folderName, extension)
        {
        }

        public override IDesign Parse(string fileName)
        {
            IParsingSourceStream SourceStream = ParsingSourceStream.CreateFromFileName(fileName);

            ResourceDictionary Content;
            List<string> FileNames;
            LoadResourceFile(SourceStream, out Content, out FileNames);

            if (!IsTypeStyleFound(Content, typeof(Windows.UI.Xaml.Controls.Button)))
                throw new ParsingException(82, SourceStream, "Missing 'Button' style.");
            if (!IsTypeStyleFound(Content, typeof(Windows.UI.Xaml.Controls.TextBox)))
                throw new ParsingException(83, SourceStream, "Missing 'TextBox' style.");
            if (!IsTypeStyleFound(Content, typeof(Windows.UI.Xaml.Controls.Image)))
                throw new ParsingException(84, SourceStream, "Missing 'Image' style.");
            if (!IsTypeStyleFound(Content, typeof(Windows.UI.Xaml.Controls.ListBox)))
                throw new ParsingException(85, SourceStream, "Missing 'ListBox' style.");
            if (!IsTypeStyleFound(Content, typeof(Windows.UI.Xaml.Controls.TextBlock)))
                throw new ParsingException(86, SourceStream, "Missing 'TextBlock' style.");
            if (!IsTypeStyleFound(Content, typeof(Windows.UI.Xaml.Controls.RadioButton)))
                throw new ParsingException(87, SourceStream, "Missing 'RadioButton' style.");
            if (!IsTypeStyleFound(Content, typeof(Windows.UI.Xaml.Controls.CheckBox)))
                throw new ParsingException(88, SourceStream, "Missing 'CheckBox' style.");
            if (!IsTypeStyleFound(Content, typeof(Windows.UI.Xaml.Controls.PasswordBox)))
                throw new ParsingException(89, SourceStream, "Missing 'PasswordBox' style.");
            if (!IsTypeStyleFound(Content, typeof(Windows.UI.Xaml.Controls.Primitives.ToggleButton)))
                throw new ParsingException(206, SourceStream, "Missing 'ToggleButton' style.");

            string MainFileName = FileNames[0];
            string Name = Path.GetFileNameWithoutExtension(MainFileName);
            return new Design(FileNames, ParserDomain.ToXamlName(SourceStream, Name, "Design"), Content);
        }

        private void LoadResourceFile(IParsingSourceStream sourceStream, out ResourceDictionary content, out List<string> fileNames)
        {
            try
            {
                string FolderName = Path.GetDirectoryName(sourceStream.FileName);
                FolderName = FolderName.Replace("\\", "/");

                using (sourceStream.Open())
                {
                    string ContentString = sourceStream.ReadToEnd();
                    ContentString = ContentString.Replace("Source=\"/", $"Source=\"{FolderName}/");
                    byte[] ContentBytes = Encoding.UTF8.GetBytes(ContentString);

                    XamlSchemaContext Context = GetContext(sourceStream);

                    using (sourceStream.OpenXamlFromBytes(ContentBytes, Context))
                    {
                        fileNames = new List<string>();
                        fileNames.Add(sourceStream.FileName);

                        content = LoadResourceFile(sourceStream, fileNames);
                    }
                }
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ParsingException(90, sourceStream, e);
            }
        }

        private XamlSchemaContext GetContext(IParsingSourceStream sourceStream)
        {
            List<Assembly> ReferencedAssemblies = new List<Assembly>();

            try
            {
                AssemblyName[] ReferencedAssemblyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
                for (int i = 0; i < ReferencedAssemblyNames.Length; i++)
                {
                    AssemblyName ReferencedAssemblyName = ReferencedAssemblyNames[i];
                    if (ReferencedAssemblyName.Name == "mscorlib" || ReferencedAssemblyName.Name == "XmlnsTest")
                        ReferencedAssemblies.Add(Assembly.Load(ReferencedAssemblyName));
                }
            }
            catch (Exception e)
            {
                throw new ParsingException(91, sourceStream, e);
            }

            XamlSchemaContext Result = new XamlSchemaContext(ReferencedAssemblies);
            return Result;
        }

        private ResourceDictionary LoadResourceFile(IParsingSourceStream sourceStream, List<string> FileNames)
        {
            Windows.UI.Xaml.ResourceDictionary Root;

            try
            {
                Root = (Windows.UI.Xaml.ResourceDictionary)sourceStream.LoadXaml();
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ParsingException(90, sourceStream, e);
            }

            ResourceDictionary WrappedDictionary = new ResourceDictionary();
            AddDictionaryContent(sourceStream, WrappedDictionary, Root, FileNames);

            return WrappedDictionary;
        }

        private void AddDictionaryContent(IParsingSourceStream sourceStream, ResourceDictionary wrapped, Windows.UI.Xaml.ResourceDictionary dictionary, List<string> FileNames)
        {
            foreach (object Key in dictionary.Keys)
                if (!wrapped.Contains(Key))
                    wrapped.Add(Key, dictionary[Key]);
                else
                    throw new ParsingException(92, sourceStream, $"Key '{Key}' found multiple times.");

            foreach (Windows.UI.Xaml.ResourceDictionary Item in dictionary.MergedDictionaries)
                if (Item.Source != null)
                {
                    IParsingSourceStream NestedSourceStream = ParsingSourceStream.CreateFromFileName(Item.Source.AbsolutePath);

                    ResourceDictionary NestedContent;
                    List<string> NestedFileNames;
                    LoadResourceFile(NestedSourceStream, out NestedContent, out NestedFileNames);

                    AddDictionaryContent(NestedSourceStream, wrapped, NestedContent);
                    FileNames.AddRange(NestedFileNames);
                }
        }

        private void AddDictionaryContent(IParsingSourceStream sourceStream, ResourceDictionary wrapped, ResourceDictionary dictionary)
        {
            foreach (object Key in dictionary.Keys)
                if (!wrapped.Contains(Key))
                    wrapped.Add(Key, dictionary[Key]);
                else
                    throw new ParsingException(92, sourceStream, $"Key '{Key}' found multiple times.");

            foreach (ResourceDictionary Item in dictionary.MergedDictionaries)
                AddDictionaryContent(sourceStream, wrapped, Item);
        }

        private bool IsTypeStyleFound(ResourceDictionary dictionary, Type type)
        {
            bool IsIncluded = dictionary.Contains(type);
            if (!IsIncluded)
                return false;

            bool IsStyle = (dictionary[type] is Windows.UI.Xaml.Style);
            if (!IsStyle)
                return false;

            return true;
        }
    }
}
