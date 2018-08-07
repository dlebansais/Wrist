using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Markup;

namespace ExtractXmlTypes
{
    class Program
    {
        private static string RootFolder;
        private static List<string> DefaultTypeList = new List<string>();

        static void Main()
        {
            string[] Args = Environment.GetCommandLineArgs();
            string LaunchFolder = System.IO.Path.GetDirectoryName(Args[0]);
            if (LaunchFolder.ToLower().EndsWith("\\bin\\debug"))
                LaunchFolder = LaunchFolder.Substring(0, LaunchFolder.Length - 10);
            else if (LaunchFolder.ToLower().EndsWith("\\bin\\x64\\debug"))
                LaunchFolder = LaunchFolder.Substring(0, LaunchFolder.Length - 14);
            else if (LaunchFolder.ToLower().EndsWith("\\bin\\release"))
                LaunchFolder = LaunchFolder.Substring(0, LaunchFolder.Length - 12);
            else if (LaunchFolder.ToLower().EndsWith("\\bin\\x64\\release"))
                LaunchFolder = LaunchFolder.Substring(0, LaunchFolder.Length - 16);

            if (LaunchFolder.ToLower().EndsWith("\\extractxmltypes"))
                LaunchFolder = LaunchFolder.Substring(0, LaunchFolder.Length - 16);

            string AssemblyPath = Path.Combine(LaunchFolder, "CSharpXamlForHtml5", "CSharpXamlForHtml5.dll");
            RootFolder = Path.Combine(LaunchFolder, "XmlnsTest");

            List<Type> AlreadyReferencedTypeList = new List<Type>();

            // Find types in referenced assemblies System and System.Xaml
            Assembly CurrentAssembly = Assembly.GetExecutingAssembly();
            AssemblyName[] AssemblyNames = CurrentAssembly.GetReferencedAssemblies();
            foreach (AssemblyName AssemblyName in AssemblyNames)
                if (AssemblyName.Name == "System" || AssemblyName.Name == "System.Xaml")
                {
                    Assembly LoadedAssembly = Assembly.Load(AssemblyName);
                    foreach (Type t in LoadedAssembly.GetTypes())
                        if (!AlreadyReferencedTypeList.Contains(t))
                            AlreadyReferencedTypeList.Add(t);
                }

            // Add types referenced in mscorlib
            Assembly SystemAssembly = Assembly.Load("mscorlib.dll");
            foreach (Type t in SystemAssembly.GetTypes())
                if (!AlreadyReferencedTypeList.Contains(t))
                    AlreadyReferencedTypeList.Add(t);

            // Obtain a list of types that don't need to be created
            foreach (Type t in AlreadyReferencedTypeList)
                DefaultTypeList.Add(GetTypeNameWithNamespace(t));

            List<Type> EnlargedTypeList;
            List<Assembly> UsedAssemblyList;
            EnumerateAllTypes(AssemblyPath, out EnlargedTypeList, out UsedAssemblyList);

            List<string> UsedNamespaceList = new List<string>();
            foreach (Type Item in EnlargedTypeList)
                ExtractXmlType(Item, EnlargedTypeList, UsedNamespaceList);

            // Display in the output window the code to use in AssemblyInfo.cs
            foreach (string Namespace in UsedNamespaceList)
                Debug.WriteLine($"[assembly: XmlnsDefinitionAttribute(\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\", \"{Namespace}\")]");
        }

        private static void EnumerateAllTypes(string AssemblyPath, out List<Type> enlargedTypeList, out List<Assembly> usedAssemblyList)
        {
            // Start from the main assembly, and recursively enumerate assemblies
            Assembly InputAssembly = Assembly.LoadFrom(AssemblyPath);
            usedAssemblyList = new List<Assembly>();
            usedAssemblyList.Add(InputAssembly);

            enlargedTypeList = new List<Type>();

            int OldCount, NewCount;
            do
            {
                OldCount = usedAssemblyList.Count + enlargedTypeList.Count;
                EnumerateAssemblyTypes(usedAssemblyList, enlargedTypeList);
                NewCount = usedAssemblyList.Count + enlargedTypeList.Count;
            }
            while (OldCount < NewCount);
        }

        private static void EnumerateAssemblyTypes(List<Assembly> usedAssemblyList, List<Type> enlargedTypeList)
        {
            List<string> NamespaceList = new List<string>();

            // Find all namespaces that have http://schemas.microsoft.com/winfx/2006/xaml/presentation as alias
            foreach (Assembly Assembly in usedAssemblyList)
                foreach (object Item in Assembly.GetCustomAttributes())
                    if (Item.GetType().Name == typeof(XmlnsDefinitionAttribute).Name)
                    {
                        string XmlNamespace = Item.GetType().GetProperty("XmlNamespace").GetValue(Item) as string;
                        string ClrNamespace = Item.GetType().GetProperty("ClrNamespace").GetValue(Item) as string;
                        if (XmlNamespace == "http://schemas.microsoft.com/winfx/2006/xaml/presentation")
                            if (!NamespaceList.Contains(ClrNamespace))
                                NamespaceList.Add(ClrNamespace);
                    }

            // Find all types that can be used in Xaml
            List<Type> TargetTypeList = new List<Type>();
            foreach (Assembly Assembly in usedAssemblyList)
                foreach (Type Item in Assembly.GetTypes())
                    foreach (string ClrNamespace in NamespaceList)
                        if (Item.Namespace == ClrNamespace)
                        {
                            if (IsTypeTarget(Item))
                                TargetTypeList.Add(Item);
                            break;
                        }

            // Find all types referenced by types we have found so far
            foreach (Type Item in TargetTypeList)
                EnlargeTypeList(Item, enlargedTypeList);
        }

        private static bool IsTypeTarget(Type type)
        {
            // A target type is a valid one with DependencyObject as ancestor
            while (IsTypeValid(type) && type.Name != "DependencyObject")
                type = GetBaseType(type);

            return IsTypeValid(type);
        }

        private static bool IsTypeValid(Type type)
        {
            if (type == null)
                return false;

            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                if (!DefaultTypeList.Contains(GetTypeNameWithNamespace(type.GetGenericTypeDefinition())))
                    return false;

                foreach (Type Parameter in type.GetGenericArguments())
                    if (!IsTypeValid(Parameter))
                        return false;
            }
            else
            {
                // If the type is not a known type
                if (!DefaultTypeList.Contains(GetTypeNameWithNamespace(type)))
                {
                    char[] InvalidPathChars = Path.GetInvalidPathChars();
                    if (type.Name.IndexOfAny(InvalidPathChars) >= 0)
                        return false;

                    Type BaseType = GetBaseType(type);
                    if (BaseType != null && !IsBaseTypeValid(BaseType))
                        return false;
                }
            }

            return true;
        }

        private static bool IsBaseTypeValid(Type type)
        {
            if (type == null)
                return false;

            // If the base type is a known type
            if (DefaultTypeList.Contains(GetTypeNameWithNamespace(type)))
            {
                // We don't want base types with abstract methods, since we only implement properties
                foreach (MethodInfo Method in type.GetMethods())
                    if (Method.IsAbstract)
                        return false;
            }
            else
            {
                if (type.IsGenericType || type.IsGenericTypeDefinition)
                    return false;
            }

            return true;
        }

        private static void EnlargeTypeList(Type type, List<Type> enlargedTypeList)
        {
            // This method find all types referenced by a type
            if (enlargedTypeList.Contains(type))
                return;

            if (type == typeof(ValueType) || type == typeof(Object))
                return;

            if (DefaultTypeList.Contains(GetTypeNameWithNamespace(type)))
            {
                if (type.IsEnum)
                    return;

                if (type.IsGenericType && !type.IsGenericTypeDefinition)
                {
                    foreach (Type Parameter in type.GetGenericArguments())
                        EnlargeTypeList(Parameter, enlargedTypeList);
                }
            }
            else
            {
                enlargedTypeList.Add(type);

                if (type.IsEnum)
                    return;

                if (type.IsGenericType && !type.IsGenericTypeDefinition)
                {
                    foreach (Type Parameter in type.GetGenericArguments())
                        EnlargeTypeList(Parameter, enlargedTypeList);
                }

                Type BaseType = GetBaseType(type);
                if (BaseType != null)
                    EnlargeTypeList(BaseType, enlargedTypeList);

                foreach (PropertyInfo Property in type.GetProperties())
                    if (IsPropertyValid(Property, type))
                        EnlargeTypeList(Property.PropertyType, enlargedTypeList);

                foreach (FieldInfo Field in type.GetFields())
                {
                    string attachedPropertyName;
                    Type attachedPropertyType;
                    Type attachedPropertyTarget;
                    if (IsValidAttachedProperty(Field, type, out attachedPropertyName, out attachedPropertyType, out attachedPropertyTarget))
                    {
                        EnlargeTypeList(attachedPropertyType, enlargedTypeList);
                        EnlargeTypeList(attachedPropertyTarget, enlargedTypeList);
                    }
                }
            }
        }

        private static bool IsPropertyValid(PropertyInfo property, Type type)
        {
            if (!property.CanRead)
                return false;

            if (property.DeclaringType != type)
                return false;

            if (!IsTypeValid(property.PropertyType))
                return false;

            // Ignore self-referencing members in structs
            if (type.IsValueType)
                if (property.PropertyType == type)
                    return false;

            return true;
        }

        private static void ExtractXmlType(Type type, List<Type> enlargedTypeList, List<string> usedNamespaceList)
        {
            // This type is predefined
            if (GetTypeNameWithNamespace(type) == "Windows.UI.Xaml.ResourceDictionary")
                return;

            if (IsTypeTarget(type))
                if (!usedNamespaceList.Contains(type.Namespace))
                    usedNamespaceList.Add(type.Namespace);

            string CSharpFileName = Path.Combine(RootFolder, GetTypeNameWithNamespace(type) + ".cs");
            using (FileStream CSharpFile = new FileStream(CSharpFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter CSharpWriter = new StreamWriter(CSharpFile, Encoding.UTF8))
                {
                    ExtractXmlType(type, enlargedTypeList, CSharpWriter);
                }
            }
        }

        private static void ExtractXmlType(Type type, List<Type> enlargedTypeList, StreamWriter cSharpWriter)
        {
            string Namespace = type.Namespace;
            string Name = GetTypeRoot(type);
            Type BaseType = GetBaseType(type);
            bool WithIndexer = HasIndexer(type) && (BaseType == null || !HasIndexer(BaseType));
            List<string> UsedNamespaces = new List<string>();

            if (!type.IsEnum)
            {
                foreach (Attribute Attribute in type.GetCustomAttributes())
                {
                    string AttributeTypeName = Attribute.GetType().Name;

                    if (AttributeTypeName == "DefaultMemberAttribute" && !WithIndexer)
                        FillUsedNamespace(typeof(DefaultMemberAttribute), UsedNamespaces);

                    else if (AttributeTypeName == "ContentPropertyAttribute")
                        FillUsedNamespace(typeof(ContentPropertyAttribute), UsedNamespaces);
                }

                FillUsedNamespace(BaseType, UsedNamespaces);

                if (WithIndexer)
                    FillUsedNamespace(typeof(System.Collections.IEnumerator), UsedNamespaces);

                foreach (PropertyInfo Property in type.GetProperties())
                    if (IsPropertyValid(Property, type))
                    {
                        FillUsedNamespace(Property.PropertyType, UsedNamespaces);

                        string CollectionInitialization;
                        Type UsingInitializer;
                        if (IsCollection(Property.PropertyType, out CollectionInitialization, out UsingInitializer))
                            FillUsedNamespace(UsingInitializer, UsedNamespaces);
                    }

                foreach (FieldInfo Field in type.GetFields())
                {
                    string attachedPropertyName;
                    Type attachedPropertyType;
                    Type attachedPropertyTarget;
                    if (IsValidAttachedProperty(Field, type, out attachedPropertyName, out attachedPropertyType, out attachedPropertyTarget))
                    {
                        FillUsedNamespace(attachedPropertyType, UsedNamespaces);
                        FillUsedNamespace(attachedPropertyTarget, UsedNamespaces);

                        string CollectionInitialization;
                        Type UsingInitializer;
                        if (IsCollection(attachedPropertyType, out CollectionInitialization, out UsingInitializer))
                            FillUsedNamespace(UsingInitializer, UsedNamespaces);
                    }
                }
            }

            if (UsedNamespaces.Contains(Namespace))
                UsedNamespaces.Remove(Namespace);

            foreach (string UsedNamespace in UsedNamespaces)
                cSharpWriter.WriteLine($"using {UsedNamespace};");

            if (UsedNamespaces.Count > 0)
                cSharpWriter.WriteLine();

            string ClassTypeDeclaration = type.IsEnum ? "enum" : (type.IsValueType ? "struct" : "class");

            string BaseTypeDeclaration;
            if (BaseType != null)
                BaseTypeDeclaration = " : " + DeclarationClause(BaseType);
            else
                BaseTypeDeclaration = "";

            cSharpWriter.WriteLine($"namespace {Namespace}");
            cSharpWriter.WriteLine("{");

            foreach (Attribute Attribute in type.GetCustomAttributes())
            {
                string AttributeTypeName = Attribute.GetType().Name;

                if (AttributeTypeName == "DefaultMemberAttribute" && !WithIndexer)
                {
                    PropertyInfo MemberNameProperty = Attribute.GetType().GetProperty("MemberName");
                    string AttributeValue = MemberNameProperty.GetValue(Attribute) as string;
                    cSharpWriter.WriteLine($"    [DefaultMember(\"{AttributeValue}\")]");
                }

                else if (AttributeTypeName == "ContentPropertyAttribute")
                {
                    PropertyInfo NameProperty = Attribute.GetType().GetProperty("Name");
                    string AttributeValue = NameProperty.GetValue(Attribute) as string;
                    cSharpWriter.WriteLine($"    [ContentProperty(\"{AttributeValue}\")]");
                }

                //else
                //    cSharpWriter.WriteLine("    //XX " + Attribute.ToString());
            }

            cSharpWriter.WriteLine($"    public {ClassTypeDeclaration} {Name}{BaseTypeDeclaration}");
            cSharpWriter.WriteLine("    {");

            if (type.IsEnum)
            {
                string[] EnumNames = type.GetEnumNames();
                foreach (string EnumName in EnumNames)
                    cSharpWriter.WriteLine($"        {EnumName},");
            }
            else
            {
                if (WithIndexer)
                    WriteDownIndexer(type, cSharpWriter);

                foreach (PropertyInfo Property in type.GetProperties())
                    if (IsPropertyValid(Property, type) && (Property.Name != "Item" || !WithIndexer))
                        WriteDownProperty(Property, cSharpWriter);

                foreach (FieldInfo Field in type.GetFields())
                {
                    string attachedPropertyName;
                    Type attachedPropertyType;
                    Type attachedPropertyTarget;
                    if (IsValidAttachedProperty(Field, type, out attachedPropertyName, out attachedPropertyType, out attachedPropertyTarget))
                        WriteDownAttachedProperty(type, attachedPropertyName, attachedPropertyType, attachedPropertyTarget, cSharpWriter);
                }
            }

            cSharpWriter.WriteLine("    }");
            cSharpWriter.WriteLine("}");
        }

        private static bool IsValidAttachedProperty(FieldInfo Field, Type type, out string attachedPropertyName, out Type attachedPropertyType, out Type attachedPropertyTarget)
        {
            attachedPropertyName = null;
            attachedPropertyType = null;
            attachedPropertyTarget = null;

            if (Field.Name.Length <= 8 || !Field.Name.EndsWith("Property") || !Field.Attributes.HasFlag(FieldAttributes.Static))
                return false;

            attachedPropertyName = Field.Name.Substring(0, Field.Name.Length - 8);
            MethodInfo Getter = null;
            MethodInfo Setter = null;

            foreach (MethodInfo Method in type.GetMethods())
                if (Method.Name == $"Get{attachedPropertyName}" && Method.Attributes.HasFlag(MethodAttributes.Static))
                {
                    ParameterInfo[] Parameters = Method.GetParameters();
                    if (Parameters.Length == 1)
                    {
                        ParameterInfo Parameter = Parameters[0];
                        Getter = Method;
                        attachedPropertyType = Method.ReturnType;
                        attachedPropertyTarget = Parameter.ParameterType;
                        break;
                    }
                }

            foreach (MethodInfo Method in type.GetMethods())
                if (Method.Name == $"Set{attachedPropertyName}" && Method.Attributes.HasFlag(MethodAttributes.Static))
                {
                    ParameterInfo[] Parameters = Method.GetParameters();
                    if (Parameters.Length == 2 && Parameters[0].ParameterType == attachedPropertyTarget && Parameters[1].ParameterType == attachedPropertyType)
                    {
                        Setter = Method;
                        break;
                    }
                }

            return (Getter != null && Setter != null);
        }

        private static void WriteDownIndexer(Type type, StreamWriter cSharpWriter)
        {
            foreach (MethodInfo Method in type.GetMethods())
                if (Method.Name == "get_Item")
                {
                    ParameterInfo[] Parameters = Method.GetParameters();
                    if (Parameters.Length == 1)
                    {
                        ParameterInfo Index = Parameters[0];

                        string ReturnType = DeclarationClause(Method.ReturnType);
                        string ParameterType = DeclarationClause(Index.ParameterType);

                        cSharpWriter.WriteLine($"        public {ReturnType} this[{ParameterType} index] {{ get {{ return null; }} set {{ }} }}");
                        cSharpWriter.WriteLine($"        public void Add({ReturnType} item) {{ }}");
                        cSharpWriter.WriteLine($"        public IEnumerator<{ReturnType}> GetEnumerator() {{ return null; }}");
                    }
                    break;
                }
        }

        private static void WriteDownProperty(PropertyInfo property, StreamWriter cSharpWriter)
        {
            string ReturnType = DeclarationClause(property.PropertyType);

            // Special cases since string will be automatically converted to DependencyProperty by WPF
            if (IsConvertibleToString(property.DeclaringType.Name, property.Name))
                ReturnType = "string";

            string CollectionInitialization;
            Type UsingInitializer;
            if (IsCollection(property.PropertyType, out CollectionInitialization, out UsingInitializer))
                CollectionInitialization = " = " + CollectionInitialization + ";";
            else
                CollectionInitialization = "";

            cSharpWriter.WriteLine($"        public {ReturnType} {property.Name} {{ get; set; }}{CollectionInitialization}");
        }

        private static void WriteDownAttachedProperty(Type type, string attachedPropertyName, Type attachedPropertyType, Type attachedPropertyTarget, StreamWriter cSharpWriter)
        {
            string PropertyType = DeclarationClause(attachedPropertyType);
            string TargetType = DeclarationClause(attachedPropertyTarget);

            // Special cases since string will be automatically converted to DependencyProperty by WPF
            if (IsConvertibleToString(type.Name, attachedPropertyName))
                PropertyType = "string";

            string CollectionInitialization;
            Type UsingInitializer;
            if (!IsCollection(attachedPropertyType, out CollectionInitialization, out UsingInitializer))
                CollectionInitialization = $"default({PropertyType})";

            cSharpWriter.WriteLine($"        public static readonly DependencyProperty {attachedPropertyName}Property;");
            cSharpWriter.WriteLine($"        public static {PropertyType} Get{attachedPropertyName}({TargetType} obj) {{ return {CollectionInitialization}; }}");
            cSharpWriter.WriteLine($"        public static void Set{attachedPropertyName}({TargetType} obj, {PropertyType} value) {{ }}");
        }

        private static bool IsPublic(MethodBase method)
        {
            return method.Attributes.HasFlag(MethodAttributes.Public);
        }

        private static bool HasIndexer(Type type)
        {
            foreach (MethodInfo Method in type.GetMethods())
                if (Method.Name == "get_Item")
                {
                    ParameterInfo[] Parameters = Method.GetParameters();
                    if (Parameters.Length == 1)
                        return true;

                    break;
                }

            return false;
        }

        private static string GetTypeNameWithNamespace(Type type)
        {
            string Root = GetTypeRoot(type);
            return $"{type.Namespace}.{Root}";
        }

        private static string GetTypeRoot(Type type)
        {
            if (type.Name == "Object")
                return "object";
            else
            {
                int GenericCharIndex = type.Name.IndexOf("`");

                string Root = GenericCharIndex >= 0 ? type.Name.Substring(0, GenericCharIndex) : type.Name;
                return Root;
            }
        }

        private static string DeclarationClause(Type type)
        {
            string Root = GetTypeRoot(type);
            string ParameterString = "";

            if (type.IsGenericType)
            {
                foreach (Type Parameter in type.GetGenericArguments())
                {
                    if (ParameterString.Length > 0)
                        ParameterString += ", ";

                    ParameterString += DeclarationClause(Parameter);
                }

                ParameterString = $"<{ParameterString}>";
            }

            return $"{Root}{ParameterString}";
        }

        private static Type GetBaseType(Type type)
        {
            if (type == null)
                return null;

            Type BaseType = type.BaseType;

            if (BaseType == null || BaseType == typeof(ValueType) || BaseType == typeof(Enum))
                return null;

            return BaseType;
        }

        private static void FillUsedNamespace(Type type, List<string> usedNamespace)
        {
            if (type == null)
                return;

            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                FillUsedNamespace(type.GetGenericTypeDefinition(), usedNamespace);

                foreach (Type Parameter in type.GetGenericArguments())
                    FillUsedNamespace(Parameter, usedNamespace);
            }
            else
            {
                if (!usedNamespace.Contains(type.Namespace))
                    usedNamespace.Add(type.Namespace);
            }
        }

        private static bool IsConvertibleToString(string typeName, string propertyName)
        {
            return ((propertyName == "Property" && typeName == "Setter") ||
                    (propertyName == "From" && typeName.EndsWith("Animation")) ||
                    (propertyName == "To" && typeName.EndsWith("Animation")) ||
                    (propertyName == "Duration" && typeName == "Timeline") ||
                    (propertyName == "KeyTime" && typeName == "ObjectKeyFrame") ||
                    (propertyName == "Margin" && typeName == "FrameworkElement") ||
                    (propertyName == "Foreground" && typeName == "Control") ||
                    (propertyName == "CornerRadius" && typeName == "Border") ||
                    (propertyName == "Background" && (typeName == "Border" || typeName == "Control" || typeName == "Panel")) ||
                    (propertyName == "BorderBrush" && (typeName == "Border" || typeName == "Control")) ||
                    (propertyName == "BorderThickness" && (typeName == "Border" || typeName == "Control")) ||
                    (propertyName == "Padding" && (typeName == "Border" || typeName == "Control")) ||
                    (propertyName == "TargetProperty" && typeName == "Storyboard"));
        }

        private static bool IsCollection(Type type, out string initializerClause, out Type UsingInitializer)
        {
            initializerClause = null;
            UsingInitializer = null;

            // Special case for string
            if (type == typeof(string))
                return false;

            if (type.IsInterface)
            {
                if (type == typeof(System.Collections.IList) || type == typeof(System.Collections.ICollection) || type == typeof(System.Collections.IEnumerable))
                {
                    initializerClause = "new List<object>()";
                    UsingInitializer = typeof(System.Collections.Generic.List<>);
                    return true;
                }

                else if (type.IsGenericType)
                {
                    Type GenericTypeDefinition = type.GetGenericTypeDefinition();
                    if (GenericTypeDefinition == typeof(IList<>) || GenericTypeDefinition == typeof(ICollection<>) || GenericTypeDefinition == typeof(IEnumerable<>))
                    {
                        Type[] TypeArguments = type.GetGenericArguments();
                        if (TypeArguments.Length == 1)
                        {
                            initializerClause = $"new List<{DeclarationClause(TypeArguments[0])}>()";
                            UsingInitializer = typeof(System.Collections.Generic.List<>);
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            else
            {
                Type[] Interfaces = type.GetInterfaces();
                foreach (Type Interface in Interfaces)
                    if (Interface == typeof(System.Collections.IEnumerable))
                    {
                        initializerClause = $"new {DeclarationClause(type)}()";
                        return true;
                    }

                return false;
            }
        }
    }
}
