using System.Collections.Generic;

namespace cryptscript
{
    public class Identifier
    {
        public IObject Reference { get; set; } = new Zilch();
    }

    public class IdentifierGroup
    {
        private Dictionary<string, Identifier> IDs { get; set; } = new Dictionary<string, Identifier>();

        public bool HasID(string name) =>
            IDs.ContainsKey(name);

        public void AddID(string name) =>
            IDs.Add(name, new Identifier());

        public void RemoveID(string name) =>
            IDs.Remove(name);

        public void SetReference(string name, IObject reference)
        {
            if(!HasID(name))
            {
                AddID(name);
            }
            IDs[name].Reference = reference;
        }

        public IObject GetReference(string name)
        {
            if(!HasID(name))
            {
                AddID(name);
            }

            return IDs[name].Reference;
        }

        public List<string> GetAllIDNames()
        {
            List<string> IDNames = new List<string>();
            foreach(KeyValuePair<string, Identifier> kv in IDs)
            {
                IDNames.Add(kv.Key);
            }

            return IDNames;
        }

        public void AddIDGroup(IdentifierGroup group)
        {
            foreach(string IDName in group.GetAllIDNames())
            {
                SetReference(IDName, group.GetReference(IDName));
            }
        }
    }
}