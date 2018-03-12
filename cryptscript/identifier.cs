using System.Collections.Generic;

namespace CryptScript
{
    public class Identifier
    {
        public Token Reference { get; set; }
        
        public Identifier()
        {
            Reference = new Token(TokenType.Zilch, "");
        }
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

        public void SetReference(string name, Token reference) =>
            IDs[name].Reference = reference;

        public Token GetReference(string name) =>
            IDs[name].Reference;
    }
}