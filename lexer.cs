using System;

namespace Cryptscript
{
    public class Token
    {
        public string type;
        public string value;

        public Token(string type, string value)
        {
            this.type = type;
            this.value = value;
        }
    }

    public class Lexer
    {
        public static OrderedDictionary<string, string> TOKENS = new OrderedDictionary<string, string>()
        {
            {" *\"[^\"]*\" *", "STRING"},
            {" *'[^']*' *", "STRING"},
            {@" *\+ *", "ADD"},
            {" *- *", "SUB"},
            {@" *\* *", "MULT"},
            {" */ *", "DIV"},
            {" *% *", "MOD"},
            {@" *\^ *", "EXP"},
            {" *&& *", "AND"},
            {" +and +", "AND"},
            {@" *\|\| *", "OR"},
            {" +or +", "OR"},
            {@" *\|\|> *", "XOR"},
            {" +xor +", "XOR"},
            {" *! *", "NOT"},
            {" +not +", "NOT"},
            {" *== *", "EQUAL"},
            {" *!= *", "INEQUAL"},
            {" *= *", "SET"},
            {@" *\( *", "LPAREN"},
            {@" *\) *", "RPAREN"},
            {@" *\[ *", "LBRACK"},
            {@" *\] *", "RBRACK"},
            {@" *\{ *", "LCURLY"},
            {@" *\} *", "RCURLY"},
            {" *, *", "COMMA"},
            {" *~[^#]*", "BUFFER"},
            {" *#.+$", "COMMENT"},
            {" *if *", "IF"},
            {" *then *", "THEN"},
            {" *else *", "ELSE"},
            {" *while *", "WHILE"},
            {" *for *", "FOR"},
            {" *end *", "END"},
            {@" *\d*\.\d+ *", "DECIMAL"},
            {@" *\d+ *", "INTEGER"},
            {" *[A-Za-z][A-Za-z0-9_]* *", "ID"},
        };

        private string text;

        public Lexer(string text)
        {
            this.text = string;
        }

        public Token[] tokenize()
        {
            List<Token> tokens = new List<Token>();
            int start = 0;
            while(start < this.text.Length)
            {
                bool match = false;
                int end = this.text.Length;
                while(!match)
                {
                    subStr = this.text.substring(start, end - start);
                    foreach(DictionaryEntry item in TOKENS)
                    {
                        string tokenStr = Regex.Match(subStr, item.Key).Value;
                        match = tokenStr == subStr;
                        if(match)
                        {
                            Token newToken = new Token(item.Value, subStr);
                            start += subStr.Length
                            break;
                        }
                    }

                    end--;
                }

                tokens.Add(newToken);
            }

			return tokens;
        }
    }
}

