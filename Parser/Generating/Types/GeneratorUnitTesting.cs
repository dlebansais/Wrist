using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parser
{
    public class GeneratorUnitTesting : IGeneratorUnitTesting
    {
        public GeneratorUnitTesting(IUnitTesting unitTesting)
        {
            Operations = new List<IGeneratorTestingOperation>();

            foreach (ITestingOperation Operation in unitTesting.Operations)
                if (Operation is IClickOperation AsClick)
                    Operations.Add(new GeneratorClickOperation(AsClick));
                else if (Operation is IFillOperation AsFill)
                    Operations.Add(new GeneratorFillOperation(AsFill));
                else if (Operation is ISelectOperation AsSelect)
                    Operations.Add(new GeneratorSelectOperation(AsSelect));
                else if (Operation is IToggleOperation AsToggle)
                    Operations.Add(new GeneratorToggleOperation(AsToggle));
                else
                    throw new InvalidOperationException();
        }

        public List<IGeneratorTestingOperation> Operations { get; private set; }

        public void Generate(string outputFolderName, string appNamespace)
        {
            string UnitTestingFileName = Path.Combine(outputFolderName, "UnitTesting.cs");

            using (FileStream UnitTestingFile = new FileStream(UnitTestingFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter CSharpWriter = new StreamWriter(UnitTestingFile, Encoding.UTF8))
                {
                    Generate(appNamespace, CSharpWriter);
                }
            }
        }

        public bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            foreach (IGeneratorTestingOperation Operation in Operations)
                IsConnected |= Operation.Connect(domain);

            return IsConnected;
        }

        private void Generate(string appNamespace, StreamWriter cSharpWriter)
        {
            cSharpWriter.WriteLine("using System;");
            cSharpWriter.WriteLine("using System.Collections.Generic;");
            cSharpWriter.WriteLine("using Windows.UI.Xaml;");
            cSharpWriter.WriteLine("using Windows.UI.Xaml.Controls;");
            cSharpWriter.WriteLine("using Windows.UI.Xaml.Controls.Primitives;");
            cSharpWriter.WriteLine("using Windows.UI.Xaml.Media;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine($"namespace {appNamespace}");
            cSharpWriter.WriteLine("{");
            cSharpWriter.WriteLine("    public abstract class TestingOperation");
            cSharpWriter.WriteLine("    {");
            cSharpWriter.WriteLine("        public string Info;");
            cSharpWriter.WriteLine("        public string ControlName;");
            cSharpWriter.WriteLine("        public Action<UnitTesting, TestingOperation, Page, FrameworkElement> Handler;");
            cSharpWriter.WriteLine("    }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("    public class ClickOperation : TestingOperation");
            cSharpWriter.WriteLine("    {");
            cSharpWriter.WriteLine("    }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("    public class FillOperation : TestingOperation");
            cSharpWriter.WriteLine("    {");
            cSharpWriter.WriteLine("    }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("    public class SelectOperation : TestingOperation");
            cSharpWriter.WriteLine("    {");
            cSharpWriter.WriteLine("    }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("    public class ToggleOperation : TestingOperation");
            cSharpWriter.WriteLine("    {");
            cSharpWriter.WriteLine("    }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("    public class UnitTesting");
            cSharpWriter.WriteLine("    {");
            cSharpWriter.WriteLine("        public UnitTesting()");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            TestTimer = new DispatcherTimer();");
            cSharpWriter.WriteLine("            TestTimer.Interval = TimeSpan.FromSeconds(1);");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        private DispatcherTimer TestTimer;");
            cSharpWriter.WriteLine("        private int OperationTicks;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public List<TestingOperation> Operations = new List<TestingOperation>()");
            cSharpWriter.WriteLine("        {");

            foreach (IGeneratorTestingOperation Operation in Operations)
            {
                string PageName = Operation.Page.XamlName;
                string ControlName = Operation.Component.ControlName;

                if (Operation is IGeneratorClickOperation AsClick)
                {
                    IGeneratorComponentButton AsButton = (IGeneratorComponentButton)Operation.Component;
                    string EventName = AsButton.ClickEventName(Operation.Page);
                    string EventArgs = "RoutedEventArgs";

                    cSharpWriter.WriteLine("            new ClickOperation()");
                    cSharpWriter.WriteLine("            {");
                    cSharpWriter.WriteLine($"                Info = \"Line {Operation.LineIndex} in {Operation.TestingFileName.Replace('\\', '/')}\",");
                    cSharpWriter.WriteLine($"                ControlName = \"{ControlName}\",");
                    cSharpWriter.WriteLine("                Handler = (UnitTesting unitTesting, TestingOperation operation, Page currentPage, FrameworkElement ctrl) =>");
                    cSharpWriter.WriteLine("                {");
                    cSharpWriter.WriteLine($"                    {PageName} page;");
                    cSharpWriter.WriteLine("                    Button ctrlButton;");
                    cSharpWriter.WriteLine($"                    if ((page = AssertPage<{PageName}>(unitTesting, operation, currentPage)) != null)");
                    cSharpWriter.WriteLine("                        if ((ctrlButton = AssertControl<Button>(unitTesting, operation, ctrl)) != null)");
                    cSharpWriter.WriteLine($"                            page.{EventName}(ctrlButton , new {EventArgs}());");
                    cSharpWriter.WriteLine("                }");
                    cSharpWriter.WriteLine("            },");
                }

                else if (Operation is IGeneratorToggleOperation AsToggle)
                {
                    IGeneratorComponentPopup AsPopup = (IGeneratorComponentPopup)Operation.Component;

                    cSharpWriter.WriteLine("            new ToggleOperation()");
                    cSharpWriter.WriteLine("            {");
                    cSharpWriter.WriteLine($"                Info = \"Line {Operation.LineIndex} in {Operation.TestingFileName.Replace('\\', '/')}\",");
                    cSharpWriter.WriteLine($"                ControlName = \"{ControlName}\",");
                    cSharpWriter.WriteLine("                Handler = (UnitTesting unitTesting, TestingOperation operation, Page currentPage, FrameworkElement ctrl) =>");
                    cSharpWriter.WriteLine("                {");
                    cSharpWriter.WriteLine($"                    {PageName} page;");
                    cSharpWriter.WriteLine("                    ToggleButton ctrlButton;");
                    cSharpWriter.WriteLine($"                    if ((page = AssertPage<{PageName}>(unitTesting, operation, currentPage)) != null)");
                    cSharpWriter.WriteLine("                        if ((ctrlButton = AssertControl<ToggleButton>(unitTesting, operation, ctrl)) != null)");
                    cSharpWriter.WriteLine("                            ctrlButton.IsChecked = !ctrlButton.IsChecked;");
                    cSharpWriter.WriteLine("                }");
                    cSharpWriter.WriteLine("            },");
                }

                else if (Operation is IGeneratorFillOperation AsFill)
                {
                    string ControlType;
                    string BindingName;
                    string EventName;
                    string EventArgs;
                    if (Operation.Component is IGeneratorComponentEdit AsEdit)
                    {
                        ControlType = "TextBox";
                        BindingName = AsEdit.GetObjectBinding(null, AsEdit.BoundObject, AsEdit.BoundObjectProperty);
                        EventName = AsEdit.TextChangedEventName;
                        EventArgs = AsEdit.HandlerArgumentTypeName;
                    }
                    else if (Operation.Component is IGeneratorComponentPasswordEdit AsPasswordEdit)
                    {
                        ControlType = "PasswordBox";
                        BindingName = AsPasswordEdit.GetObjectBinding(null, AsPasswordEdit.BoundObject, AsPasswordEdit.BoundObjectProperty);
                        EventName = AsPasswordEdit.PasswordChangedEventName;
                        EventArgs = AsPasswordEdit.HandlerArgumentTypeName;
                    }
                    else
                        throw new InvalidOperationException();

                    cSharpWriter.WriteLine("            new FillOperation()");
                    cSharpWriter.WriteLine("            {");
                    cSharpWriter.WriteLine($"                Info = \"Line {Operation.LineIndex} in {Operation.TestingFileName.Replace('\\', '/')}\",");
                    cSharpWriter.WriteLine($"                ControlName = \"{ControlName}\",");
                    cSharpWriter.WriteLine("                Handler = (UnitTesting unitTesting, TestingOperation operation, Page currentPage, FrameworkElement ctrl) =>");
                    cSharpWriter.WriteLine("                {");
                    cSharpWriter.WriteLine($"                    {PageName} page;");
                    cSharpWriter.WriteLine($"                    {ControlType} ctrlEdit;");
                    cSharpWriter.WriteLine($"                    if ((page = AssertPage<{PageName}>(unitTesting, operation, currentPage)) != null)");
                    cSharpWriter.WriteLine($"                        if ((ctrlEdit = AssertControl<{ControlType}>(unitTesting, operation, ctrl)) != null)");
                    cSharpWriter.WriteLine("                        {");
                    cSharpWriter.WriteLine($"                            page.{BindingName} = \"{AsFill.Content}\";");
                    cSharpWriter.WriteLine($"                            page.{EventName}(ctrlEdit, new {EventArgs}());");
                    cSharpWriter.WriteLine("                        }");
                    cSharpWriter.WriteLine("                }");
                    cSharpWriter.WriteLine("            },");
                }

                else if (Operation is IGeneratorSelectOperation AsSelect)
                {
                }
                else
                    throw new InvalidOperationException();
            }

            cSharpWriter.WriteLine("        };");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        public void Start(Page startPage)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            OperationTicks = 0;");
            cSharpWriter.WriteLine("            TestTimer.Tick += OnTick;");
            cSharpWriter.WriteLine("            TestTimer.Start();");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        private void OnTick(object sender, object e)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            DispatcherTimer TestTimer = (DispatcherTimer)sender;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("            if (Operations.Count == 0)");
            cSharpWriter.WriteLine("            {");
            cSharpWriter.WriteLine("                TestTimer.Stop();");
            cSharpWriter.WriteLine("                TestTimer.Tick -= OnTick;");
            cSharpWriter.WriteLine("                return;");
            cSharpWriter.WriteLine("            }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("            TestingOperation Operation = Operations[0];");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("            Page CurrentPage = (Page)Window.Current.Content;");
            cSharpWriter.WriteLine("            FrameworkElement Ctrl = FindControl(CurrentPage, Operation.ControlName);");
            cSharpWriter.WriteLine("            if (Ctrl != null)");
            cSharpWriter.WriteLine("            {");
            cSharpWriter.WriteLine("                Operations.RemoveAt(0);");
            cSharpWriter.WriteLine("                OperationTicks = 0;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("                Operation.Handler(this, Operation, CurrentPage, Ctrl);");
            cSharpWriter.WriteLine("            }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("            else if (OperationTicks < 5)");
            cSharpWriter.WriteLine("                OperationTicks++;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("            else");
            cSharpWriter.WriteLine("                Abort(Operation, \"Control not found.\");");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        private void Abort(TestingOperation operation, string reason)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            TestTimer.Stop();");
            cSharpWriter.WriteLine("            TestTimer.Tick -= OnTick;");
            cSharpWriter.WriteLine("            MessageBox.Show($\"{operation.Info}\\r\\nReason: {reason}\", \"Unit Test Failure\");");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        private FrameworkElement FindControl(DependencyObject root, string name)");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            FrameworkElement Result = null;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("            if (root is Presentation.Popup AsPopup)");
            cSharpWriter.WriteLine("                return FindControl(AsPopup.Child, name);");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("            else if (root is FrameworkElement AsFrameworkElement)");
            cSharpWriter.WriteLine("            {");
            cSharpWriter.WriteLine("                if (AsFrameworkElement.Name == name)");
            cSharpWriter.WriteLine("                    Result = AsFrameworkElement;");
            cSharpWriter.WriteLine("                else if (AsFrameworkElement.Opacity > 0)");
            cSharpWriter.WriteLine("                {");
            cSharpWriter.WriteLine("                    int Count = VisualTreeHelper.GetChildrenCount(root);");
            cSharpWriter.WriteLine("                    for (int i = 0; i < Count; i++)");
            cSharpWriter.WriteLine("                    {");
            cSharpWriter.WriteLine("                        DependencyObject Child = VisualTreeHelper.GetChild(root, i);");
            cSharpWriter.WriteLine("                        if ((Result = FindControl(Child, name)) != null)");
            cSharpWriter.WriteLine("                            break;");
            cSharpWriter.WriteLine("                    }");
            cSharpWriter.WriteLine("                }");
            cSharpWriter.WriteLine("                else");
            cSharpWriter.WriteLine("                    return null;");
            cSharpWriter.WriteLine("            }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("            return Result;");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        private static T AssertPage<T>(UnitTesting unitTesting, TestingOperation operation, Page page) where T : Page");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            if (!(page is T))");
            cSharpWriter.WriteLine("            {");
            cSharpWriter.WriteLine("                unitTesting.Abort(operation, \"Invalid page.\");");
            cSharpWriter.WriteLine("                return null;");
            cSharpWriter.WriteLine("            }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("            return page as T;");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("        private static T AssertControl<T>(UnitTesting unitTesting, TestingOperation operation, FrameworkElement ctrl) where T : FrameworkElement");
            cSharpWriter.WriteLine("        {");
            cSharpWriter.WriteLine("            FrameworkElement element = ctrl;");
            cSharpWriter.WriteLine("            while (element != null)");
            cSharpWriter.WriteLine("            {");
            cSharpWriter.WriteLine("                if (element.GetType().Name.EndsWith(\"PopupRoot\"))");
            cSharpWriter.WriteLine("                    break;");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("                if (!element.IsLoaded || !element.IsEnabled || element.Visibility != Visibility.Visible)");
            cSharpWriter.WriteLine("                {");
            cSharpWriter.WriteLine("                    unitTesting.Abort(operation, \"Invalid control.\");");
            cSharpWriter.WriteLine("                    return null;");
            cSharpWriter.WriteLine("                }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("                element = element.Parent as FrameworkElement;");
            cSharpWriter.WriteLine("            }");
            cSharpWriter.WriteLine();
            cSharpWriter.WriteLine("            return ctrl as T;");
            cSharpWriter.WriteLine("        }");
            cSharpWriter.WriteLine("    }");
            cSharpWriter.WriteLine("}");
        }
    }
}
