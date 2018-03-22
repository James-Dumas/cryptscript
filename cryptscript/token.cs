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
            System.String.Format("({0}, '{1}')", Type.ToString(), Value);

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
        Unknown
    }
}