using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace Parser
{
    public class GeneratorDesign : IGeneratorDesign
    {
        public static Dictionary<IDesign, IGeneratorDesign> GeneratorDesignMap { get; } = new Dictionary<IDesign, IGeneratorDesign>();

        public GeneratorDesign(IDesign design)
        {
            FileNames = design.FileNames;
            XamlName = design.XamlName;
            Root = design.Root;

            GeneratorDesignMap.Add(design, this);
        }

        public List<string> FileNames { get; private set; }
        public string Name { get { return Path.GetFileNameWithoutExtension(FileNames[0]); } }
        public string XamlName { get; private set; }
        public ResourceDictionary Root { get; private set; }

        public bool Connect(IGeneratorDomain domain)
        {
            return false;
        }

        public void Generate(IGeneratorDomain domain, string outputFolderName, IGeneratorColorScheme colorScheme)
        {
            string XamlFileName = GeneratorDomain.GetFilePath(outputFolderName, Name);
            string XamlFolderName = Path.GetDirectoryName(XamlFileName);

            if (!Directory.Exists(XamlFolderName))
                Directory.CreateDirectory(XamlFolderName);

            using (FileStream XamlFile = new FileStream(XamlFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter XamlWriter = new StreamWriter(XamlFile, Encoding.UTF8))
                {
                    Generate(domain, XamlWriter, true, 0, colorScheme);
                }
            }
        }

        public void Generate(IGeneratorDomain domain, StreamWriter xamlWriter, bool declareXmlns, int indentation, IGeneratorColorScheme colorScheme)
        {
            string s = GeneratorLayout.IndentationString(indentation);

            if (declareXmlns)
            {
                colorScheme.WriteXamlLine(xamlWriter, s + "<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"");
                colorScheme.WriteXamlLine(xamlWriter, s + "                    xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">");
            }
            else
                colorScheme.WriteXamlLine(xamlWriter, s + "<ResourceDictionary>");

            indentation++;

            foreach (object Key in Root.Keys)
            {
                string StyleKey = KeyToString(Key);
                object Item = Root[Key];
                string Content = System.Windows.Markup.XamlWriter.Save(Item);
                Content = CleanupXaml(StyleKey, Content, indentation);
                colorScheme.WriteXamlLine(xamlWriter, Content);
            }

            colorScheme.WriteXamlLine(xamlWriter, s + "</ResourceDictionary>");
        }

        private string CleanupXaml(string key, string xamlText, int indentation)
        {
            string Result = "";
            string s;

            xamlText = xamlText.Replace(" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"", "");
            xamlText = xamlText.Replace("<Style.Resources><ResourceDictionary /></Style.Resources>", "");
            
            List<string> TagList = new List<string>();

            int EndTagIndex = 0;
            int OldTagIndex = 0;

            while ((EndTagIndex = xamlText.IndexOf("><", EndTagIndex)) >= 0)
            {
                string Tag = xamlText.Substring(OldTagIndex, EndTagIndex - OldTagIndex + 1);
                TagList.Add(Tag);

                OldTagIndex = ++EndTagIndex;
            }

            TagList.Add(xamlText.Substring(OldTagIndex));

            bool IsFirstTag = true;

            foreach (string t in TagList)
            {
                string Tag;

                if (IsFirstTag)
                {
                    IsFirstTag = false;

                    int BeginOffset;
                    if ((BeginOffset = t.IndexOf(' ')) > 0)
                        Tag = t.Substring(0, BeginOffset) + " x:Key=\"" + key + "\"" + t.Substring(BeginOffset);
                    else if ((BeginOffset = t.IndexOf('>')) > 0)
                        Tag = t.Substring(0, BeginOffset) + " x:Key=\"" + key + "\"" + t.Substring(BeginOffset);
                    else
                        Tag = t;
                }
                else
                    Tag = t;

                if (Tag.StartsWith("</"))
                {
                    indentation--;
                    s = GeneratorLayout.IndentationString(indentation);
                    Result += s + Tag + "\r\n";
                }

                else if (Tag.EndsWith("/>"))
                {
                    s = GeneratorLayout.IndentationString(indentation);
                    Result += s + Tag + "\r\n";
                }

                else
                {
                    s = GeneratorLayout.IndentationString(indentation);
                    Result += s + Tag + "\r\n";

                    if (!Tag.Contains("</"))
                        indentation++;
                }
            }

            return Result;
        }

        private string KeyToString(object key)
        {
            if (key is Type AsType)
                return XamlName + AsType.Name + "Style";

            else if (key is string AsString)
                return AsString;

            else
                throw new InvalidOperationException();
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
