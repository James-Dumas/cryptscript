using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Cryptscript
{
    public class Token
    {
        public string type;
        public string value;

        public Token(string type, string value)
        {
            this.type = type;
            this.value = value.Trim();
        }

        public string Repr()
        {
            return String.Format("({0}, '{1}')", this.type, this.value);
        }
    }

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
            {" +and +", "AND"},
            {@" *\|\|", "OR"},
            {" +or +", "OR"},
            {@" *\|\|>", "XOR"},
            {" +xor +", "XOR"},
            {" *!", "NOT"},
            {" +not +", "NOT"},
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
            {" *if", "IF"},
            {" *then", "THEN"},
            {" *else", "ELSE"},
            {" *while", "WHILE"},
            {" *for", "FOR"},
            {" *do", "DO"},
            {" *end", "END"},
            {@" *\d*\.\d+", "DECIMAL"},
            {@" *\d+", "INTEGER"},
            {" *[A-Za-z][A-Za-z0-9_]*", "ID"},
        };

        private string text;

        public Lexer(string text)
        {
            this.text = text.Trim();
        }

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
                            newToken = new Token(item.Value, subStr);
                            start += subStr.Length;
                            break;
                        }
                    }

                    end--;
                }

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

    public class Interpreter
    {
        public static void Main(String[] args)
        {
            Console.WriteLine("Type 'exit' to quit.");
            while(true)
            {
                Console.Write(">> ");
                string input = Console.ReadLine().Trim();
                if(input == "exit")
                {
                    break;
                }

                Lexer lexer = new Lexer(input);
                string output = "";
                foreach(Token token in lexer.tokenize())
                {
                    output += token.Repr() + " ";
                }
                Console.WriteLine(output);
            }
        }
    }
}