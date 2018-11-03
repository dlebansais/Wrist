using NetTools;
using Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace AppCSHtml5
{
    public class Samples : ObjectBase, ISamples
    {
        private enum ErrorCodes
        {
            AnyError = -1,
            Success = 0,
            Error,
        }

        private static readonly string TitleAbstractMethod0 = "Declaration of an abstract method";
        private static readonly string TitleAbstractMethod1 = "Declaration of an abstract class";
        private static readonly string TitleAbstractMethod2 = "Method inherited as abstract";
        private static readonly string TitleAgents0 = "Initializing and calling agents";
        private static readonly string TitleAnchors0 = "Anchored Type";
        private static readonly string TitleAnchors1 = "Parent Class Declaration";
        private static readonly string TitleAnchors2 = "Child class declaration";
        private static readonly string TitleAnchors3 = "Anchored types differences";
        private static readonly string TitleAttachment0 = "Invalid assignment";
        private static readonly string TitleAttachment1 = "Attachment of a reference";
        private static readonly string TitleAttachment2 = "Multiple attachments tests";
        private static readonly string TitleAttachment3 = "Simultaneous attachments";
        private static readonly string TitleConcurrency0 = "Creating on new processor";
        private static readonly string TitleConcurrency1 = "Creating on existing processor";
        private static readonly string TitleConcurrency2 = "Raising events";
        private static readonly string TitleContracts0 = "The check instruction";
        private static readonly string TitleContracts1 = "A precondition";
        private static readonly string TitleContracts2 = "A postcondition";
        private static readonly string TitleContracts3 = "Class invariant example";
        private static readonly string TitleContracts4 = "Loop invariant";
        private static readonly string TitleDiscreteConstants0 = "Declaration of discrete constants";
        private static readonly string TitleDiscreteConstants1 = "Discrete constants with values";
        private static readonly string TitleEditor0 = "Example of an editor's feature";
        private static readonly string TitleEntities0 = "Type of a first argument";
        private static readonly string TitleEquality0 = "Incomparable class";
        private static readonly string TitleExceptions0 = "Exception handler with retry";
        private static readonly string TitleExceptions1 = "Summary of exceptions thrown";
        private static readonly string TitleGenerics0 = "Array of numbers";
        private static readonly string TitleGenerics1 = "The Array class";
        private static readonly string TitleGenerics2 = "Constrained generic parameter";
        private static readonly string TitleGenerics3 = "Generic parameter with default";
        private static readonly string TitleGenerics4 = "Assigned type argument";
        private static readonly string TitleIndexers0 = "Declaring an indexer";
        private static readonly string TitleIndexers1 = "Declaring a read-write indexer";
        private static readonly string TitleIndexers2 = "Declaring a read-only indexer";
        private static readonly string TitleIndexers3 = "Declaring a write-only indexer";
        private static readonly string TitleIndexers4 = "Calling indexers";
        private static readonly string TitleIndexers5 = "Changing a cell in a 3D matrice";
        private static readonly string TitleIndexers6 = "Reading a hashtable";
        private static readonly string TitleInheritance0 = "The inherit clause";
        private static readonly string TitleInheritance1 = "Renaming inherited methods";
        private static readonly string TitleInheritance2 = "Combined initialize method";
        private static readonly string TitleInheritance3 = "Using conformance (Part 1)";
        private static readonly string TitleInheritance4 = "Using conformance (Part 2)";
        private static readonly string TitleInheritance5 = "Discontinued method";
        private static readonly string TitleLibraries0 = "Declaring a library";
        private static readonly string TitleLibraries1 = "Using a library";
        private static readonly string TitleLibraries2 = "Using a library with rename";
        private static readonly string TitleLibraries3 = "Library with a source";
        private static readonly string TitleLibraries4 = "Import with a source";
        private static readonly string TitleOnceFunctions0 = "Once per processor delaration";
        private static readonly string TitleOverlays0 = "Matrice addition, with 2 overlays";
        private static readonly string TitleOverlays1 = "Redefined function with overlays";
        private static readonly string TitlePolymorphism0 = "Example of polymorphism";
        private static readonly string TitlePreprocessor0 = "Replicated class";
        private static readonly string TitlePreprocessor1 = "Library with class replication";
        private static readonly string TitleProperties0 = "A read-write property";
        private static readonly string TitleTypeAliases0 = "A complicated type declaration";
        private static readonly string TitleTypeAliases1 = "Intermediate type aliases";
        private static readonly string TitleValueTypes0 = "Simple comparison";
        private static readonly string TitleValueTypes1 = "Creation of a reference";
        private static readonly string TitleValueTypes2 = "Using type Any";
        private static readonly string TitleValueTypes3 = "Specializing to a value type";
        private static readonly string TitleVariants0 = "Loop variant";
        private static readonly string TitleVersioning0 = "Importing a library as stable";
        private static readonly string TitleVersioning1 = "Importing a library as strict";

        public Samples()
        {
            InitSampleCodeTable();
            InitSimulation();

            Database.DebugLog = true;
            Database.DebugLogFullResponse = true;
        }

        private void InitSampleCodeTable()
        {
            _AllSampleCodes = new Dictionary<string, ISampleCode>();
            _AllSampleCodes.Add(TitleAbstractMethod0, new SampleCode(PageNames.feature_abstract_methodsPage, true));
            _AllSampleCodes.Add(TitleAbstractMethod1, new SampleCode(PageNames.feature_abstract_methodsPage, true));
            _AllSampleCodes.Add(TitleAbstractMethod2, new SampleCode(PageNames.feature_abstract_methodsPage, true));
            _AllSampleCodes.Add(TitleAgents0, new SampleCode(PageNames.feature_agentsPage, false));
            _AllSampleCodes.Add(TitleAnchors0, new SampleCode(PageNames.feature_anchorsPage, true));
            _AllSampleCodes.Add(TitleAnchors1, new SampleCode(PageNames.feature_anchorsPage, false));
            _AllSampleCodes.Add(TitleAnchors2, new SampleCode(PageNames.feature_anchorsPage, false));
            _AllSampleCodes.Add(TitleAnchors3, new SampleCode(PageNames.feature_anchorsPage, true));
            _AllSampleCodes.Add(TitleAttachment0, new SampleCode(PageNames.feature_attachmentPage, true));
            _AllSampleCodes.Add(TitleAttachment1, new SampleCode(PageNames.feature_attachmentPage, true));
            _AllSampleCodes.Add(TitleAttachment2, new SampleCode(PageNames.feature_attachmentPage, true));
            _AllSampleCodes.Add(TitleAttachment3, new SampleCode(PageNames.feature_attachmentPage, true));
            _AllSampleCodes.Add(TitleConcurrency0, new SampleCode(PageNames.feature_concurrencyPage, true));
            _AllSampleCodes.Add(TitleConcurrency1, new SampleCode(PageNames.feature_concurrencyPage, true));
            _AllSampleCodes.Add(TitleConcurrency2, new SampleCode(PageNames.feature_concurrencyPage, true));
            _AllSampleCodes.Add(TitleContracts0, new SampleCode(PageNames.feature_contractsPage, true));
            _AllSampleCodes.Add(TitleContracts1, new SampleCode(PageNames.feature_contractsPage, true));
            _AllSampleCodes.Add(TitleContracts2, new SampleCode(PageNames.feature_contractsPage, true));
            _AllSampleCodes.Add(TitleContracts3, new SampleCode(PageNames.feature_contractsPage, true));
            _AllSampleCodes.Add(TitleContracts4, new SampleCode(PageNames.feature_contractsPage, false));
            _AllSampleCodes.Add(TitleDiscreteConstants0, new SampleCode(PageNames.feature_discrete_constantsPage, true));
            _AllSampleCodes.Add(TitleDiscreteConstants1, new SampleCode(PageNames.feature_discrete_constantsPage, true));
            _AllSampleCodes.Add(TitleEditor0, new SampleCode(PageNames.feature_editorPage, true));
            _AllSampleCodes.Add(TitleEntities0, new SampleCode(PageNames.feature_entitiesPage, true));
            _AllSampleCodes.Add(TitleEquality0, new SampleCode(PageNames.feature_equalityPage, true));
            _AllSampleCodes.Add(TitleExceptions0, new SampleCode(PageNames.feature_exceptionsPage, true));
            _AllSampleCodes.Add(TitleExceptions1, new SampleCode(PageNames.feature_exceptionsPage, true));
            _AllSampleCodes.Add(TitleGenerics0, new SampleCode(PageNames.feature_genericsPage, true));
            _AllSampleCodes.Add(TitleGenerics1, new SampleCode(PageNames.feature_genericsPage, true));
            _AllSampleCodes.Add(TitleGenerics2, new SampleCode(PageNames.feature_genericsPage, true));
            _AllSampleCodes.Add(TitleGenerics3, new SampleCode(PageNames.feature_genericsPage, true));
            _AllSampleCodes.Add(TitleGenerics4, new SampleCode(PageNames.feature_genericsPage, true));
            _AllSampleCodes.Add(TitleIndexers0, new SampleCode(PageNames.feature_indexersPage, true));
            _AllSampleCodes.Add(TitleIndexers1, new SampleCode(PageNames.feature_indexersPage, true));
            _AllSampleCodes.Add(TitleIndexers2, new SampleCode(PageNames.feature_indexersPage, true));
            _AllSampleCodes.Add(TitleIndexers3, new SampleCode(PageNames.feature_indexersPage, true));
            _AllSampleCodes.Add(TitleIndexers4, new SampleCode(PageNames.feature_indexersPage, true));
            _AllSampleCodes.Add(TitleIndexers5, new SampleCode(PageNames.feature_indexersPage, true));
            _AllSampleCodes.Add(TitleIndexers6, new SampleCode(PageNames.feature_indexersPage, true));
            _AllSampleCodes.Add(TitleInheritance0, new SampleCode(PageNames.feature_inheritancePage, true));
            _AllSampleCodes.Add(TitleInheritance1, new SampleCode(PageNames.feature_inheritancePage, true));
            _AllSampleCodes.Add(TitleInheritance2, new SampleCode(PageNames.feature_inheritancePage, false));
            _AllSampleCodes.Add(TitleInheritance3, new SampleCode(PageNames.feature_inheritancePage, false));
            _AllSampleCodes.Add(TitleInheritance4, new SampleCode(PageNames.feature_inheritancePage, true));
            _AllSampleCodes.Add(TitleInheritance5, new SampleCode(PageNames.feature_inheritancePage, true));
            _AllSampleCodes.Add(TitleLibraries0, new SampleCode(PageNames.feature_librariesPage, true));
            _AllSampleCodes.Add(TitleLibraries1, new SampleCode(PageNames.feature_librariesPage, true));
            _AllSampleCodes.Add(TitleLibraries2, new SampleCode(PageNames.feature_librariesPage, true));
            _AllSampleCodes.Add(TitleLibraries3, new SampleCode(PageNames.feature_librariesPage, true));
            _AllSampleCodes.Add(TitleLibraries4, new SampleCode(PageNames.feature_librariesPage, true));
            _AllSampleCodes.Add(TitleOnceFunctions0, new SampleCode(PageNames.feature_once_functionsPage, true));
            _AllSampleCodes.Add(TitleOverlays0, new SampleCode(PageNames.feature_overlaysPage, true));
            _AllSampleCodes.Add(TitleOverlays1, new SampleCode(PageNames.feature_overlaysPage, true));
            _AllSampleCodes.Add(TitlePolymorphism0, new SampleCode(PageNames.feature_polymorphismPage, true));
            _AllSampleCodes.Add(TitlePreprocessor0, new SampleCode(PageNames.feature_preprocessorPage, false));
            _AllSampleCodes.Add(TitlePreprocessor1, new SampleCode(PageNames.feature_preprocessorPage, false));
            _AllSampleCodes.Add(TitleProperties0, new SampleCode(PageNames.feature_propertiesPage, true));
            _AllSampleCodes.Add(TitleTypeAliases0, new SampleCode(PageNames.feature_type_aliasesPage, false));
            _AllSampleCodes.Add(TitleTypeAliases1, new SampleCode(PageNames.feature_type_aliasesPage, false));
            _AllSampleCodes.Add(TitleValueTypes0, new SampleCode(PageNames.feature_value_typesPage, false));
            _AllSampleCodes.Add(TitleValueTypes1, new SampleCode(PageNames.feature_value_typesPage, true));
            _AllSampleCodes.Add(TitleValueTypes2, new SampleCode(PageNames.feature_value_typesPage, false));
            _AllSampleCodes.Add(TitleValueTypes3, new SampleCode(PageNames.feature_value_typesPage, false));
            _AllSampleCodes.Add(TitleVariants0, new SampleCode(PageNames.feature_variantsPage, false));
            _AllSampleCodes.Add(TitleVersioning0, new SampleCode(PageNames.feature_versioningPage, true));
            _AllSampleCodes.Add(TitleVersioning1, new SampleCode(PageNames.feature_versioningPage, true));
        }

        private Dictionary<string, ISampleCode> _AllSampleCodes;
        private Random Rng = new Random();
        public ISampleCode AbstractMethod0 { get { return GetSampleCode(TitleAbstractMethod0); } }
        public ISampleCode AbstractMethod1 { get { return GetSampleCode(TitleAbstractMethod1); } }
        public ISampleCode AbstractMethod2 { get { return GetSampleCode(TitleAbstractMethod2); } }
        public ISampleCode Agents0  { get { return GetSampleCode(TitleAgents0); } }
        public ISampleCode Anchors0  { get { return GetSampleCode(TitleAnchors0); } }
        public ISampleCode Anchors1  { get { return GetSampleCode(TitleAnchors1); } }
        public ISampleCode Anchors2  { get { return GetSampleCode(TitleAnchors2); } }
        public ISampleCode Anchors3  { get { return GetSampleCode(TitleAnchors3); } }
        public ISampleCode Attachment0  { get { return GetSampleCode(TitleAttachment0); } }
        public ISampleCode Attachment1  { get { return GetSampleCode(TitleAttachment1); } }
        public ISampleCode Attachment2  { get { return GetSampleCode(TitleAttachment2); } }
        public ISampleCode Attachment3  { get { return GetSampleCode(TitleAttachment3); } }
        public ISampleCode Concurrency0  { get { return GetSampleCode(TitleConcurrency0); } }
        public ISampleCode Concurrency1  { get { return GetSampleCode(TitleConcurrency1); } }
        public ISampleCode Concurrency2  { get { return GetSampleCode(TitleConcurrency2); } }
        public ISampleCode Contracts0  { get { return GetSampleCode(TitleContracts0); } }
        public ISampleCode Contracts1  { get { return GetSampleCode(TitleContracts1); } }
        public ISampleCode Contracts2  { get { return GetSampleCode(TitleContracts2); } }
        public ISampleCode Contracts3  { get { return GetSampleCode(TitleContracts3); } }
        public ISampleCode Contracts4  { get { return GetSampleCode(TitleContracts4); } }
        public ISampleCode DiscreteConstants0  { get { return GetSampleCode(TitleDiscreteConstants0); } }
        public ISampleCode DiscreteConstants1  { get { return GetSampleCode(TitleDiscreteConstants1); } }
        public ISampleCode Editor0  { get { return GetSampleCode(TitleEditor0); } }
        public ISampleCode Entities0  { get { return GetSampleCode(TitleEntities0); } }
        public ISampleCode Equality0  { get { return GetSampleCode(TitleEquality0); } }
        public ISampleCode Exceptions0  { get { return GetSampleCode(TitleExceptions0); } }
        public ISampleCode Exceptions1  { get { return GetSampleCode(TitleExceptions1); } }
        public ISampleCode Generics0  { get { return GetSampleCode(TitleGenerics0); } }
        public ISampleCode Generics1  { get { return GetSampleCode(TitleGenerics1); } }
        public ISampleCode Generics2  { get { return GetSampleCode(TitleGenerics2); } }
        public ISampleCode Generics3  { get { return GetSampleCode(TitleGenerics3); } }
        public ISampleCode Generics4  { get { return GetSampleCode(TitleGenerics4); } }
        public ISampleCode Indexers0  { get { return GetSampleCode(TitleIndexers0); } }
        public ISampleCode Indexers1  { get { return GetSampleCode(TitleIndexers1); } }
        public ISampleCode Indexers2  { get { return GetSampleCode(TitleIndexers2); } }
        public ISampleCode Indexers3  { get { return GetSampleCode(TitleIndexers3); } }
        public ISampleCode Indexers4  { get { return GetSampleCode(TitleIndexers4); } }
        public ISampleCode Indexers5  { get { return GetSampleCode(TitleIndexers5); } }
        public ISampleCode Indexers6  { get { return GetSampleCode(TitleIndexers6); } }
        public ISampleCode Inheritance0  { get { return GetSampleCode(TitleInheritance0); } }
        public ISampleCode Inheritance1  { get { return GetSampleCode(TitleInheritance1); } }
        public ISampleCode Inheritance2  { get { return GetSampleCode(TitleInheritance2); } }
        public ISampleCode Inheritance3  { get { return GetSampleCode(TitleInheritance3); } }
        public ISampleCode Inheritance4  { get { return GetSampleCode(TitleInheritance4); } }
        public ISampleCode Inheritance5  { get { return GetSampleCode(TitleInheritance5); } }
        public ISampleCode Libraries0  { get { return GetSampleCode(TitleLibraries0); } }
        public ISampleCode Libraries1  { get { return GetSampleCode(TitleLibraries1); } }
        public ISampleCode Libraries2  { get { return GetSampleCode(TitleLibraries2); } }
        public ISampleCode Libraries3  { get { return GetSampleCode(TitleLibraries3); } }
        public ISampleCode Libraries4  { get { return GetSampleCode(TitleLibraries4); } }
        public ISampleCode OnceFunctions0  { get { return GetSampleCode(TitleOnceFunctions0); } }
        public ISampleCode Overlays0  { get { return GetSampleCode(TitleOverlays0); } }
        public ISampleCode Overlays1  { get { return GetSampleCode(TitleOverlays1); } }
        public ISampleCode Polymorphism0  { get { return GetSampleCode(TitlePolymorphism0); } }
        public ISampleCode Preprocessor0  { get { return GetSampleCode(TitlePreprocessor0); } }
        public ISampleCode Preprocessor1  { get { return GetSampleCode(TitlePreprocessor1); } }
        public ISampleCode Properties0  { get { return GetSampleCode(TitleProperties0); } }
        public ISampleCode TypeAliases0  { get { return GetSampleCode(TitleTypeAliases0); } }
        public ISampleCode TypeAliases1  { get { return GetSampleCode(TitleTypeAliases1); } }
        public ISampleCode ValueTypes0  { get { return GetSampleCode(TitleValueTypes0); } }
        public ISampleCode ValueTypes1  { get { return GetSampleCode(TitleValueTypes1); } }
        public ISampleCode ValueTypes2  { get { return GetSampleCode(TitleValueTypes2); } }
        public ISampleCode ValueTypes3  { get { return GetSampleCode(TitleValueTypes3); } }
        public ISampleCode Variants0  { get { return GetSampleCode(TitleVariants0); } }
        public ISampleCode Versioning0  { get { return GetSampleCode(TitleVersioning0); } }
        public ISampleCode Versioning1  { get { return GetSampleCode(TitleVersioning1); } }

        public ISampleCode RandomSampleCode
        {
            get
            {
                int Index = Rng.Next(_AllSampleCodes.Count);

                for(;;)
                {
                    foreach (KeyValuePair<string, ISampleCode> Entry in _AllSampleCodes)
                        if (Index > 0)
                            Index--;
                        else if (((SampleCode)Entry.Value).IsFrontPage)
                            return GetSampleCode(Entry.Key);
                }
            }
        }

        private ISampleCode GetSampleCode(string title)
        {
            if (!((SampleCode)_AllSampleCodes[title]).IsLoaded)
                GetSampleCode(title, (int error, object result) => OnSampleCodeReceived(error, result, title));

            return _AllSampleCodes[title];
        }

        private void OnSampleCodeReceived(int error, object result, string title)
        {
            if (error == (int)ErrorCodes.Success && result != null)
            {
                IDictionary<string, string> Item = (IDictionary<string, string>)result;

                SampleCode SampleCode = (SampleCode)_AllSampleCodes[title];
                SampleCode.UpdateContent(Item["feature"], Encoding.UTF8.GetString(Convert.FromBase64String(Item["text"])), Item["title_enu"], Item["title_fra"]);
            }
        }

        public void On_SelectSample(PageNames pageName, IObjectBase senderContext, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            SampleCode Sample = (SampleCode)senderContext;
            destinationPageName = Sample.PageName;
        }

        #region Transactions
        private void GetSampleCode(string title, Action<int, object> callback)
        {
            Database.Query(new DatabaseQueryOperation("get sample code", "features/query_sample_code.php", new Dictionary<string, string>() { { "title", HtmlString.PercentEncoded(title) } }, (object sender, CompletionEventArgs e) => OnGetAllSampleCodesCompleted(sender, e, callback)));
        }

        private void OnGetAllSampleCodesCompleted(object sender, CompletionEventArgs e, Action<int, object> callback)
        {
            IDictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "front_page", "feature", "text", "title_enu", "title_fra", "result" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => callback(ParseResult(Result["result"]), Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => callback((int)ErrorCodes.AnyError, null));
        }

        public static int ParseResult(string result)
        {
            int IntError;
            if (int.TryParse(result, out IntError))
                return IntError;
            else
                return -1;
        }
        #endregion

        #region Simulation
        private void InitSimulation()
        {
            if (NetTools.UrlTools.IsUsingRestrictedFeatures)
                return;

            OperationHandler.Add(new OperationHandler($"/{Database.QueryScriptPath}features/query_sample_code.php", OnQuerySampleCode));
        }

        private List<IDictionary<string, string>> OnQuerySampleCode(IDictionary<string, string> parameters)
        {
            List<IDictionary<string, string>> Result = new List<IDictionary<string, string>>();

            Result.Add(new Dictionary<string, string>()
            {
                { "front_page", "0" },
                { "feature", "abstract_methods" },
                { "text", Convert.ToBase64String(Encoding.UTF8.GetBytes("<p><span id=\"sc_neutral\">Insert</span>&nbsp;<span id=\"sc_keyword\">procedure</span></p>")) },
                { "title_enu", "Declaration of an abstract method" },
                { "title_fra", "Déclaration d'une méthode abstraite" },
                { "result", ((int)ErrorCodes.Success).ToString() },
            });

            return Result;
        }
        #endregion

        private Database Database = Database.Current;

        #region Implementation of INotifyPropertyChanged
        /// <summary>
        ///     Implements the PropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameter is mandatory with [CallerMemberName]")]
        internal void NotifyThisPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
