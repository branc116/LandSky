using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandSky.UIComponents
{
    public class UIComponentsCollection : IList<UIComponentBase>
    {
        
        public int Count
        {
            get
            {
                return mCollection.Count();
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        public UIComponentBase this[int index]
        {
            get
            {
                return mCollection[index];
            }

            set
            {
                mCollection[index] = value;
            }
        }
        public UIComponentBase this[string name]
        {
            get
            {
                return mCollection.Where(i => i.Name == name).First();
            }

            set
            {
                var a = mCollection.Where(i => i.Name == name).First();
                mCollection[mCollection.IndexOf(a)] = value;
            }
        }
        public UIComponentBase CurrentActive { get; private set; } = null;

        private int mCurrentTab = -1;
        private List<UIComponentBase> mCollection = new List<UIComponentBase>();

        internal void AddRange(UIComponentBase[] Components)
        {
            bool shuldTab = mCollection.Count == 0 ? true : false;
            mCollection.AddRange(Components);
            if (shuldTab)
                TabNext();
        }

        public void Clear()
        {
            mCollection.Clear();
        }
        public void RemoveAt(int index)
        {
            mCollection.RemoveAt(index);
        }
        public int CountSpecType(Type t)
        {
            return mCollection.Count(i => i.GetType() == t);
        }
        public UIComponentBase TabNext()
        {
            mCurrentTab++;
            mCurrentTab %= Count == 0 ? 1 : Count;
            if (CurrentActive != null)
            {
                CurrentActive.Focus = false;
                CurrentActive.OnTab -= CurrentActiveOnTab;
            }
            CurrentActive = mCollection?[mCurrentTab];
            if (CurrentActive != null) {

                CurrentActive.OnTab += CurrentActiveOnTab;
                CurrentActive.Focus = true;
                
            }
            return mCollection?[mCurrentTab];
        }

        
        public bool ParseCommand(ConsoleKeyInfo KeyInfo)
        {
            return (bool)CurrentActive?.NewInput(KeyInfo);
        }
        public void Add(UIComponentBase item)
        {
            mCollection.Add(item);
            if (Count == 1)
                TabNext();
        }
        public bool Remove(UIComponentBase item)
        {
            return mCollection.Remove(item);
        }
        public int IndexOf(UIComponentBase item)
        {
            return mCollection.IndexOf(item);
        }
        public bool Contains(UIComponentBase item)
        {
            return mCollection.Contains(item);
        }
        public void Insert(int index, UIComponentBase item)
        {
            mCollection.Insert(index, item);
        }
        public IEnumerator<UIComponentBase> GetEnumerator()
        {
            foreach (var item in mCollection)
            {
                yield return item;
            }
        }
        public void CopyTo(UIComponentBase[] array, int arrayIndex)
        {
            mCollection.CopyTo(array, arrayIndex);
        }

        private void CurrentActiveOnTab(object sender, DateTime e)
        {
            TabNext();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return mCollection.GetEnumerator();
        }
    }
}
