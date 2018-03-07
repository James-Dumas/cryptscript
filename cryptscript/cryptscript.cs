using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using CryptscriptLexer;
using CryptscriptIdentifier;
using CryptscriptOperation;

namespace Cryptscript
{
    public class Token
    {
        public string Type;
        public string Value;

        public Token(string type, string value)
        {
            Type = type;
            Value = value.Trim();
            // remove quotes around strings
            if(type == "STRING")
            {
                Value = Value.Substring(1, Value.Length - 2);
            }
        }

        /// get string representation of token
        public string Repr()
        {
            return String.Format("({0}, '{1}')", Type, Value);
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
