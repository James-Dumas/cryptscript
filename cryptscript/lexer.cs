using System.Collections.Generic;

using Cryptscript;

namespace CryptscriptLexer
{
    public class Lexer
    {
        public static Dictionary<string, string> TOKENS = new Dictionary<string, string>()
        {
            {" *#![a-zA-Z0-9]+$", "WALLET"},
            {" *\"[^\"]*\"", "STRING"},
            {" *'[^']*'", "STRING"},
            {@" *\+", "ADD"},
            {" *-", "SUB"},
            {@" *\*", "MULT"},
            {" */", "DIV"},
            {" *%", "MOD"},
            {@" *\^", "EXP"},
            {" *&&", "AND"},
            {" +(?i)(and) +", "AND"},
            {@" *\|\|", "OR"},
            {" +(?i)(or) +", "OR"},
            {@" *\|\|>", "XOR"},
            {" +(?i)(xor) +", "XOR"},
            {" *!", "NOT"},
            {" +(?i)(not) +", "NOT"},
            {" *==", "EQUAL"},
            {" *!=", "INEQUAL"},
            {" *>", "GREATER"},
            {" *<", "LESS"},
            {" *>=", "GREATEREQ"},
            {" *<=", "LESSEQ"},
            {" *=", "SET"},
            {@" *\(", "LPAREN"},
            {@" *\)", "RPAREN"},
            {@" *\[", "LBRACK"},
            {@" *\]", "RBRACK"},
            {@" *\{", "LCURLY"},
            {@" *\}", "RCURLY"},
            {" *,", "COMMA"},
            {" *~[^#]*", "COMMENT"},
            {" *#[a-zA-Z0-9]+$", "BUFFER"},
            {" *(?i)(if)", "IF"},
            {" *(?i)(then)", "THEN"},
            {" *(?i)(else)", "ELSE"},
            {" *(?i)(while)", "WHILE"},
            {" *(?i)(for)", "FOR"},
            {" *(?i)(do)", "DO"},
            {" *(?i)(end)", "END"},
            {" *(?i)(true)", "TRUE"},
            {" *(?i)(false)", "FALSE"},
            {@" *\d*\.\d+", "DECIMAL"},
            {@" *\d+", "INTEGER"},
            {" *[A-Za-z][A-Za-z0-9_]*", "ID"},
        };

        private string text;

        public Lexer(string text)
        {
            this.text = text.Trim();
        }

        /// convert string of text into tokens
        public List<Token> tokenize()
        {
            List<Token> tokens = new List<Token>();
            Token errorToken = new Token("ERROR", "SyntaxError");
            int start = 0;
            while(start < this.text.Length)
            {
                Token newToken = new Token("", "");
                bool match = false;
                bool error = false;
                int end = this.text.Length;
                while(!match && !error)
                {
                    // try to match every token's regex to a substring of the text that is decreasing in length
                    string subStr = this.text.Substring(start, end - start);
                    error = end <= start;
                    foreach(KeyValuePair<string, string> item in TOKENS)
                    {
                        string tokenStr = Regex.Match(subStr, item.Key).Value;
                        match = tokenStr == subStr;
                        if(match)
                        {
                            // add a new token with the value found at the matched regex
                            newToken = new Token(item.Value, subStr);
                            start += subStr.Length;
                            break;
                        }
                    }

                    end--;
                }

                // return syntax error when text can't be tokenized
                if(error)
                {
                    tokens.Add(errorToken);
                    break;
                }
                else
                {
                    tokens.Add(newToken);
                }
            }

			return tokens;
        }
    }
}
