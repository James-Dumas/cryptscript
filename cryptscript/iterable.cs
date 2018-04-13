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

        public IObject Get(int index)
            => Items[RealIndex(index)];

        public void Set(int index, IObject obj)
            => Items[RealIndex(index)] = obj;

        public void Append(IObject obj)
            => Items.Add(obj);

        public IObject Pop(int index)
        {
            IObject obj = Items[RealIndex(index)];
            Items.RemoveAt(RealIndex(index));
            return obj;
        }

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
                repr += Items[i].Repr();
                if(i + 1 < Length)
                {
                    repr += ", ";
                }
            }

            repr += "]";
            return repr;
        }
    }

    public class IterDict : Iterable, IObject
    {
        private List<IObject> Keys { get; set; }

        public IterDict(List<IObject> keys, List<IObject> items) : base(items)
        {
            Keys = keys;
        }

        public IObject Get(IObject key)
        {
            if(Keys.Contains(key))
            {
                return Items[Keys.IndexOf(key)];
            }
            else
            {
                return new Error(ErrorType.KeyError);
            }
        }

        public void Set(IObject key, IObject obj)
        {
            if(Keys.Contains(key))
            {
                Items[Keys.IndexOf(key)] = obj;
            }
            else
            {
                Keys.Add(key);
                Items.Add(obj);
            }
        }

        public IObject Pop(IObject key)
        {
            if(Keys.Contains(key))
            {
                int index = Keys.IndexOf(key);
                IObject obj = Items[index];
                Keys.RemoveAt(index);
                Items.RemoveAt(index);
                return obj;
            }
            else
            {
                return new Error(ErrorType.KeyError);
            }
        }

        public override string Repr()
        {
            string repr = "{";
            for(int i = 0; i < Length; i++)
            {
                repr += Keys[i].Repr() + ": " + Items[i].Repr();
                if(i + 1 < Length)
                {
                    repr += ", ";
                }
            }

            repr += "}";
            return repr;
        }
    }
}