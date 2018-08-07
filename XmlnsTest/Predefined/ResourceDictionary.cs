using System.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Windows.UI.Xaml
{
    public class ResourceDictionary : DependencyObject, IDictionary, ICollection, IDictionary<object, object>, ICollection<KeyValuePair<object, object>>, IEnumerable<KeyValuePair<object, object>>, IEnumerable, INameScope
    {
        public ObservableCollection<ResourceDictionary> MergedDictionaries { get; set; } = new ObservableCollection<ResourceDictionary>();
        public Uri Source { get; set; }
        public Boolean IsReadOnly { get; set; }
        public Boolean IsSynchronized { get; set; }
        public object SyncRoot { get; set; }
        public Boolean IsFixedSize { get; set; }

        public object this[object key]
        {
            get { return MyDictionary[key]; }
            set { MyDictionary[key] = value; }
        }

        public ICollection<object> Keys { get { return MyDictionary.Keys; } }
        ICollection IDictionary.Keys { get { return MyDictionary.Keys; } }
        public ICollection<object> Values { get { return MyDictionary.Values; } }
        ICollection IDictionary.Values { get { return MyDictionary.Values; } }

        public void Add(object key, object value)
        {
            MyDictionary.Add(key, value);
        }

        public bool ContainsKey(object key)
        {
            return MyDictionary.ContainsKey(key);
        }

        public bool Remove(object key)
        {
            return MyDictionary.Remove(key);
        }

        public bool TryGetValue(object key, out object value)
        {
            return MyDictionary.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<object, object> item)
        {
            ICollection<KeyValuePair<object, object>> AsCollection = MyDictionary as ICollection<KeyValuePair<object, object>>;
            AsCollection.Add(item);
        }

        public void Clear()
        {
            ICollection<KeyValuePair<object, object>> AsCollection = MyDictionary as ICollection<KeyValuePair<object, object>>;
            AsCollection.Clear();
        }

        public bool Contains(KeyValuePair<object, object> item)
        {
            ICollection<KeyValuePair<object, object>> AsCollection = MyDictionary as ICollection<KeyValuePair<object, object>>;
            return AsCollection.Contains(item);
        }

        bool IDictionary.Contains(object item)
        {
            IDictionary AsDictionary = MyDictionary as IDictionary;
            return AsDictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<object, object>[] items, int index)
        {
            ICollection<KeyValuePair<object, object>> AsCollection = MyDictionary as ICollection<KeyValuePair<object, object>>;
            AsCollection.CopyTo(items, index);
        }

        void ICollection.CopyTo(Array items, int index)
        {
            ICollection AsCollection = MyDictionary as ICollection;
            AsCollection.CopyTo(items, index);
        }

        public bool Remove(KeyValuePair<object, object> item)
        {
            ICollection<KeyValuePair<object, object>> AsCollection = MyDictionary as ICollection<KeyValuePair<object, object>>;
            return AsCollection.Remove(item);
        }

        void IDictionary.Remove(object item)
        {
            IDictionary AsDictionary = MyDictionary as IDictionary;
            AsDictionary.Remove(item);
        }

        public int Count
        {
            get
            {
                ICollection<KeyValuePair<object, object>> AsCollection = MyDictionary as ICollection<KeyValuePair<object, object>>;
                return AsCollection.Count;
            }
        }

        public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            IEnumerable<KeyValuePair<object, object>> AsEnumerable = MyDictionary as ICollection<KeyValuePair<object, object>>;
            return AsEnumerable.GetEnumerator();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            IDictionary AsDictionary = MyDictionary as IDictionary;
            return AsDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerable AsEnumerable = MyDictionary as IEnumerable;
            return AsEnumerable.GetEnumerator();
        }

        public object FindName(string name) { return null; }
        public void RegisterName(string name, object scopedElement) { }
        public void UnregisterName(string name) { }

        private Dictionary<object, object> MyDictionary = new Dictionary<object, object>();
    }
}
