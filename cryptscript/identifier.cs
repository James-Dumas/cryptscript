using System.Collections.Generic;

using Cryptscript;

namespace CryptscriptIdentifier
{
    public class Identifier
    {
        public string Name;
        private Token reference;
        
        public Identifier(string name)
        {
            Name = name;
        }

        public void SetReference(Token reference)
        {
            this.reference = reference;
        }

        public Token GetReference()
        {
            return this.reference;
        }
    }

    public class IdentifierGroup
    {
        private Dictionary<string, Identifier> IDs = new Dictionary<string, Identifier>();

        public void AddID(string name)
        {
            IDs.Add(name, new Identifier(name));
        }

        public void RemoveID(string name)
        {
            IDs.Remove(name);
        }
        
        public void SetReference(string name, Token reference)
        {
            IDs[name].SetReference(reference);
        }

        public Token GetReference(string name, Token reference)
        {
            return IDs[name].GetReference();
        }
    }
}
