using System;
using System.Collections.Generic;

namespace cryptscript
{
    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value.Trim();

            switch (type)
            {
                case TokenType.String:
                    Value = Value.Substring(1, Value.Length - 2);
                    break;

                case TokenType.Integer:
                    Value = Convert.ToString(Convert.ToInt32(Value));
                    break;

                case TokenType.Decimal:
                    Value = Convert.ToString(Convert.ToDouble(Value));
                    break;
                
                case TokenType.Wallet:
                    Value = Value.Substring(2, Value.Length - 2);
                    break;
            }
        }

        /// <summary>
        /// Returns a formatted representation of the token
        /// </summary>
        public override string ToString() =>
            System.String.Format("(Type: {0}, Value: '{1}')", Type.ToString(), Value);

        /// <summary>
        /// Dictionary that relates tokens to their respective operations
        /// </summary>
        public static Dictionary<TokenType, OperationType> OperationOf = new Dictionary<TokenType, OperationType>()
        {
            { TokenType.Addition, OperationType.Addition },
            { TokenType.Subraction, OperationType.Subraction },
            { TokenType.Multiplication, OperationType.Multiplication },
            { TokenType.Division, OperationType.Division },
            { TokenType.Modulo, OperationType.Modulo },
            { TokenType.Exponent, OperationType.Exponent },
            { TokenType.AND, OperationType.AND },
            { TokenType.OR, OperationType.OR },
            { TokenType.XOR, OperationType.XOR },
            { TokenType.NOT, OperationType.NOT },
            { TokenType.Equal, OperationType.Equal },
            { TokenType.Inequal, OperationType.Inequal },
            { TokenType.Greater, OperationType.Greater },
            { TokenType.Less, OperationType.Less },
            { TokenType.GreaterEqual, OperationType.GreaterEqual },
            { TokenType.LessEqual, OperationType.LessEqual }
        };

        public static List<TokenType> LoopTokens = new List<TokenType>()
        {
            TokenType.If,
            TokenType.Elif,
            TokenType.Else,
            TokenType.While,
            TokenType.For,
            TokenType.Func
        };
    }

    public class TokenGroup : Token
    {
        public List<Token> Tokens { get; set; }

        public TokenGroup(TokenType type, List<Token> tokens) : base(type, "")
        {
            if(type == TokenType.List || type == TokenType.Dict)
            {
                tokens.RemoveAt(0);
                tokens.RemoveAt(tokens.Count - 1);
            }

            Tokens = tokens;
        }

        public override string ToString()
        {
            string str = "(Type: " + Type.ToString() + ", Tokens: {";
            for(int i = 0; i < Tokens.Count; i++)
            {
                str += Tokens[i].ToString();
                if(i < Tokens.Count - 1)
                {
                    str += ", ";
                }
            }

            str += "})";

            return str;
        }
    }

    /// <summary>
    /// Possible types of tokens
    /// </summary>
    public enum TokenType
    {
        Wallet,
        String,
        Addition,
        Subraction,
        Multiplication,
        Division,
        Modulo,
        Exponent,
        AND,
        OR,
        XOR,
        NOT,
        Equal,
        Inequal,
        Greater,
        Less,
        GreaterEqual,
        LessEqual,
        Set,
        LeftParenthesis,
        RightParenthesis,
        LeftBracket,
        RightBracket,
        LeftCurly,
        RightCurly,
        Comma,
        Comment,
        Buffer,
        If,
        Then,
        Else,
        Elif,
        While,
        For,
        Func,
        Do,
        End,
        Break,
        Return,
        Zilch,
        Bool,
        Integer,
        Decimal,
        ID,
        Unknown,
        List,
        Dict,
        IndexedObj,
        CalledFunc
    }
}