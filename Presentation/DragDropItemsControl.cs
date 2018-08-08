using System.Collections;
using System.Collections.Specialized;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Presentation
{
    public class MyContainer : ItemsControl
    {
        public FrameworkElement GenerateFrameworkElement(object item)
        {
            FrameworkElement Element = GenerateFrameworkElementToRenderTheItem(item);
            return Element;
        }
    }

    public class DragDropItemsControl : PanelDragDropTarget
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IList), typeof(DragDropItemsControl), new PropertyMetadata(null, OnItemsSourcePropertyChanged));
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(DragDropItemsControl), new PropertyMetadata(null, OnItemTemplatePropertyChanged));

        public DragDropItemsControl()
        {
            Container = new MyContainer();
        }

        MyContainer Container;

        public IList ItemsSource { get; set; }
        public DataTemplate ItemTemplate { get; set; }

        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DragDropItemsControl Ctrl = d as DragDropItemsControl;
            if (Ctrl != null)
            {
                Ctrl.OnItemsSourcePropertyChanged(e.OldValue, e.NewValue);
            }
        }

        private static void OnItemTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DragDropItemsControl Ctrl = d as DragDropItemsControl;
            if (Ctrl != null)
            {
                Ctrl.OnItemTemplatePropertyChanged(e.OldValue, e.NewValue);
            }
        }

        private void OnItemsSourcePropertyChanged(object OldValue, object NewValue)
        {
            if (OldValue != NewValue)
            {
                INotifyCollectionChanged AsOldCollection = OldValue as INotifyCollectionChanged;
                if (AsOldCollection != null)
                    AsOldCollection.CollectionChanged -= OnSourceCollectionChanged;

                INotifyCollectionChanged AsNewCollection = NewValue as INotifyCollectionChanged;
                if (AsNewCollection != null)
                {
                    IList AsList = NewValue as IList;
                    if (AsList != null)
                    {
                        NotifyCollectionChangedEventArgs Args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, AsList, 0);
                        OnSourceCollectionChanged(this, Args);
                    }

                    AsNewCollection.CollectionChanged += OnSourceCollectionChanged;
                }
            }
        }

        private void OnItemTemplatePropertyChanged(object OldValue, object NewValue)
        {
            Container.ItemTemplate = NewValue as DataTemplate;
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Panel ContentPanel = Content as Panel;
            UIElementCollection Children = ContentPanel.Children;
            int StartIndex;
            Container.ItemTemplate = ItemTemplate;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    StartIndex = e.NewStartingIndex;
                    foreach (object Item in e.NewItems)
                    {
                        FrameworkElement Element = Container.GenerateFrameworkElement(Item);
                        AddItem(ContentPanel, Element);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
            }
        }
    }
}
