using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xaml;

namespace Parser
{
    public class ParserLayout : FormParser<ILayout>
    {
        public ParserLayout(string folderName, string extension)
            : base(folderName, extension)
        {
        }

        public override ILayout Parse(string fileName, IDictionary<ConditionalDefine, bool> conditionalDefineTable)
        {
            IParsingSourceStream SourceStream = ParsingSourceStream.CreateFromFileName(fileName, conditionalDefineTable);

            try
            {
                XamlSchemaContext Context = GetContext();
                using (SourceStream.OpenXamlFromFile(Context))
                {
                    return Parse(fileName, SourceStream);
                }
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ParsingException(93, SourceStream, e);
            }
        }

        private XamlSchemaContext GetContext()
        {
            List<Assembly> ReferencedAssemblies = new List<Assembly>();
            ReferencedAssemblies.Add(Assembly.GetExecutingAssembly());

            return new XamlSchemaContext(ReferencedAssemblies);
        }

        private ILayout Parse(string fileName, IParsingSourceStream sourceStream)
        {
            Layout Root;

            try
            {
                Root = (Layout)sourceStream.LoadXaml();

                string Name = Path.GetFileNameWithoutExtension(fileName);
                string XamlName = ParserDomain.ToXamlName(sourceStream, Name, "Layout");

                Root.SetName(Name, XamlName, fileName);
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ParsingException(93, sourceStream, e);
            }

            return Root;
        }
    }
}
