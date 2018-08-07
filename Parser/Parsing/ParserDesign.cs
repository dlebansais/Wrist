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
            IParsingSource Source = ParsingSource.CreateFromFileName(fileName);

            ResourceDictionary Content;
            List<string> FileNames;
            LoadResourceFile(Source, out Content, out FileNames);

            if (!IsTypeStyleFound(Content, typeof(Windows.UI.Xaml.Controls.Button)))
                throw new ParsingException(Source, "Missing 'Button' style");
            if (!IsTypeStyleFound(Content, typeof(Windows.UI.Xaml.Controls.TextBox)))
                throw new ParsingException(Source, "Missing 'TextBox' style");
            if (!IsTypeStyleFound(Content, typeof(Windows.UI.Xaml.Controls.Image)))
                throw new ParsingException(Source, "Missing 'Image' style");
            if (!IsTypeStyleFound(Content, typeof(Windows.UI.Xaml.Controls.ListBox)))
                throw new ParsingException(Source, "Missing 'ListBox' style");
            if (!IsTypeStyleFound(Content, typeof(Windows.UI.Xaml.Controls.TextBlock)))
                throw new ParsingException(Source, "Missing 'TextBlock' style");

            string MainFileName = FileNames[0];
            string Name = Path.GetFileNameWithoutExtension(MainFileName);
            return new Design(FileNames, ParserDomain.ToXamlName(Source, Name, "Design"), Content);
        }

        private void LoadResourceFile(IParsingSource source, out ResourceDictionary content, out List<string> fileNames)
        {
            try
            {
                string FolderName = Path.GetDirectoryName(source.FileName);
                FolderName = FolderName.Replace("\\", "/");

                using (source.Open())
                {
                    string ContentString = source.ReadToEnd();
                    ContentString = ContentString.Replace("Source=\"/", $"Source=\"{FolderName}/");
                    byte[] ContentBytes = Encoding.UTF8.GetBytes(ContentString);

                    XamlSchemaContext Context = GetContext(source);

                    using (source.OpenXamlFromBytes(ContentBytes, Context))
                    {
                        fileNames = new List<string>();
                        fileNames.Add(source.FileName);

                        content = LoadResourceFile(source, fileNames);
                    }
                }
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ParsingException(source, e);
            }
        }

        private XamlSchemaContext GetContext(IParsingSource source)
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
                throw new ParsingException(source, e);
            }

            XamlSchemaContext Result = new XamlSchemaContext(ReferencedAssemblies);
            return Result;
        }

        private ResourceDictionary LoadResourceFile(IParsingSource source, List<string> FileNames)
        {
            Windows.UI.Xaml.ResourceDictionary Root;

            try
            {
                Root = (Windows.UI.Xaml.ResourceDictionary)source.LoadXaml();
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ParsingException(source, e);
            }

            ResourceDictionary WrappedDictionary = new ResourceDictionary();
            AddDictionaryContent(source, WrappedDictionary, Root, FileNames);

            return WrappedDictionary;
        }

        private void AddDictionaryContent(IParsingSource source, ResourceDictionary wrapped, Windows.UI.Xaml.ResourceDictionary dictionary, List<string> FileNames)
        {
            foreach (object Key in dictionary.Keys)
                if (!wrapped.Contains(Key))
                    wrapped.Add(Key, dictionary[Key]);
                else
                    throw new ParsingException(source, $"Key {Key} found multiple times");

            foreach (Windows.UI.Xaml.ResourceDictionary Item in dictionary.MergedDictionaries)
                if (Item.Source != null)
                {
                    IParsingSource NestedSource = ParsingSource.CreateFromFileName(Item.Source.AbsolutePath);

                    ResourceDictionary NestedContent;
                    List<string> NestedFileNames;
                    LoadResourceFile(NestedSource, out NestedContent, out NestedFileNames);

                    AddDictionaryContent(NestedSource, wrapped, NestedContent);
                    FileNames.AddRange(NestedFileNames);
                }
        }

        private void AddDictionaryContent(IParsingSource source, ResourceDictionary wrapped, ResourceDictionary dictionary)
        {
            foreach (object Key in dictionary.Keys)
                if (!wrapped.Contains(Key))
                    wrapped.Add(Key, dictionary[Key]);
                else
                    throw new ParsingException(source, $"Key {Key} found multiple times");

            foreach (ResourceDictionary Item in dictionary.MergedDictionaries)
                AddDictionaryContent(source, wrapped, Item);
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
