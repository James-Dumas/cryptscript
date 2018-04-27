using System;
using System.Collections.Generic;

namespace cryptscript
{
    public class Iterable : Object, IObject
    {
        public List<IObject> Items { get; protected set; }
        public int Length => ((List<IObject>) Items).Count;

        public Iterable(List<IObject> items)
        {
            Items = items;
        }
    }

    public class IterList : Iterable, IObject
    {
        public IterList(List<IObject> items) : base(items) {}

        // pre-req: index is valid for the list
        public IObject Get(int index)
            => Items[RealIndex(index)];

        // pre-req: index is valid for the list
        public void Set(int index, IObject obj)
            => Items[RealIndex(index)] = obj;

        // pre-req: index is valid for the list
        public IObject Pop(int index)
        {
            IObject obj = Items[RealIndex(index)];
            Items.RemoveAt(RealIndex(index));
            return obj;
        }

        // pre-req: index is valid for the list
        public void Insert(int index, IObject obj)
            => Items.Insert(RealIndex(index), obj);

        public int RealIndex(int index)
            => index >= 0
                ? index
                : Length + index;

        public override string Repr()
        {
            string repr = "[";
            for(int i = 0; i < Length; i++)
            {
                repr += Items[i].Repr(true);
                if(i + 1 < Length)
                {
                    repr += ", ";
                }
            }

            repr += "]";
            return repr;
        }

        public override string Repr(bool showQuotes) => Repr();
    }

    public class IterDict : Iterable, IObject
    {
        private List<IObject> Keys { get; set; }

        public IterDict(List<IObject> keys, List<IObject> items) : base(items)
        {
            Keys = keys;
        }

        public bool HasKey(IObject key)
        {
            bool result = false;
            foreach(IObject k in Keys)
            {
                result = result || k.Value.Equals(key.Value);
            }

            return result;
        }

        private int GetIndex(IObject key)
        {
            int result = -1;
            for(int i = 0; i < Length; i++)
            {
                if(Keys[i].Value.Equals(key.Value))
                {
                    result = i;
                }
            }

            return result;
        }

        // pre-req: key is in the dictionary
        public IObject Get(IObject key)
            => Items[GetIndex(key)];

        // pre-req: key is immutable
        public void Set(IObject key, IObject obj)
        {
            if(HasKey(key))
            {
                Items[GetIndex(key)] = obj;
            }
            else
            {
                Keys.Add(key);
                Items.Add(obj);
            }
        }

        // pre-req: key is in the dictionary
        public IObject Pop(IObject key)
        {
            int index = Keys.IndexOf(key);
            IObject obj = Items[index];
            Keys.RemoveAt(index);
            Items.RemoveAt(index);
            return obj;
        }

        public override string Repr()
        {
            string repr = "{";
            for(int i = 0; i < Length; i++)
            {
                repr += Keys[i].Repr(true) + ": " + Items[i].Repr(true);
                if(i + 1 < Length)
                {
                    repr += ", ";
                }
            }

            repr += "}";
            return repr;
        }

        public override string Repr(bool showQuotes) => Repr();
    }
}