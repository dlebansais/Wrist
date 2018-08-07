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

        public override ILayout Parse(string fileName)
        {
            string Name = Path.GetFileNameWithoutExtension(fileName);
            IParsingSource Source = ParsingSource.CreateFromFileName(fileName);

            try
            {
                XamlSchemaContext Context = GetContext();
                using (Source.OpenXamlFromFile(Context))
                {
                    return Parse(Name, Source);
                }
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ParsingException(Source, e);
            }
        }

        private XamlSchemaContext GetContext()
        {
            List<Assembly> ReferencedAssemblies = new List<Assembly>();
            ReferencedAssemblies.Add(Assembly.GetExecutingAssembly());

            return new XamlSchemaContext(ReferencedAssemblies);
        }

        private ILayout Parse(string name, IParsingSource source)
        {
            Layout Root;

            try
            {
                Root = (Layout)source.LoadXaml();
                Root.SetName(name, ParserDomain.ToXamlName(source, name, "Layout"));
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ParsingException(source, e);
            }

            return Root;
        }
    }
}
