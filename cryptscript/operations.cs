using System;

using Cryptscript;

namespace CryptscriptOperation
{
    public class Operation
    {
        private static string ResultType(Token A, Token B)
        {
            return A.Type == "DECIMAL" || B.Type == "DECIMAL" ? "DECIMAL" : "INTEGER";
        }

        public Token Add(Token A, Token B)
        {
            if(ResultType(A, B) == "DECIMAL")
            {
                return new Token("DECIMAL", Convert.ToString(Convert.ToDouble(A.Value) + Convert.ToDouble(B.Value)));
            }
            else
            {
                return new Token("INTEGER", Convert.ToString(Convert.ToInt32(A.Value) + Convert.ToInt32(B.Value)));
            }
        }

        public Token Subtract(Token A, Token B)
        {
            if(ResultType(A, B) == "DECIMAL")
            {
                return new Token("DECIMAL", Convert.ToString(Convert.ToDouble(A.Value) - Convert.ToDouble(B.Value)));
            }
            else
            {
                return new Token("INTEGER", Convert.ToString(Convert.ToInt32(A.Value) - Convert.ToInt32(B.Value)));
            }
        }

        public Token Multiply(Token A, Token B)
        {
            if(ResultType(A, B) == "DECIMAL")
            {
                return new Token("DECIMAL", Convert.ToString(Convert.ToDouble(A.Value) * Convert.ToDouble(B.Value)));
            }
            else
            {
                return new Token("INTEGER", Convert.ToString(Convert.ToInt32(A.Value) * Convert.ToInt32(B.Value)));
            }
        }

        public Token Divide(Token A, Token B)
        {
            return new Token("DECIMAL", Convert.ToString(Convert.ToDouble(A.Value) / Convert.ToDouble(B.Value)));
        }

        public Token Concatenate(Token A, Token B)
        {
            string a = A.Type == "STRING" ? A.Value.Substring(1, A.Value.Length - 2) : Convert.ToString(A.Value);
            string b = B.Type == "STRING" ? B.Value.Substring(1, B.Value.Length - 2) : Convert.ToString(B.Value);
            return new Token("STRING", a + b);
        }
    }
}
