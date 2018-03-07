using System.Collections.Generic;

namespace CryptScript
{
    public class Identifier
    {
        public string Name { get; set; }
        public Token Reference { get; set; }
        
        public Identifier(string name)
        {
            Name = name;
        }
    }

    public class IdentifierGroup
    {
        private Dictionary<string, Identifier> IDs = new Dictionary<string, Identifier>();

        public void AddID(string name) =>
            IDs.Add(name, new Identifier(name));

        public void RemoveID(string name) =>
            IDs.Remove(name);

        public void SetReference(string name, Token reference) =>
            IDs[name].Reference = reference;

        public Token GetReference(string name, Token reference) =>
            IDs[name].Reference;
    }
}
