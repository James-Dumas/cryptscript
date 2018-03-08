using System;
using System.Collections.Generic;

namespace CryptScript
{
    public class Token
    {
        public TokenType Type;
        public string Value;

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value.Trim();

            // Remove quotes around strings
            if(type == TokenType.String)
            {
                Value = Value.Substring(1, Value.Length - 2);
            }

            // for int/double, convert to int/double then back to string, to remove unnecessary leading/trailing zeros
            if(type == TokenType.Integer)
            {
                Value = Convert.ToString(Convert.ToInt32(Value));
            }

            if(type == TokenType.Decimal)
            {
                Value = Convert.ToString(Convert.ToDouble(Value));
            }
        }

        /// <summary>
        /// Returns a formatted representation of the token
        /// </summary>
        public override string ToString()
        {
            return String.Format("({0}, '{1}')", Type.ToString(), Value);
        }

        /// <summary>
        /// Performs an operation on 1-2 tokens
        /// </summary>
        /// <param name="operation">The operation to perform</param>
        /// <param name="x">The first token</param>
        /// <param name="y">The second token</param>
        /// <returns></returns>
        public static Token Operation(OperationType operation, params Token[] inputs)
        {
            Token x = inputs[0];
            Token y = inputs.Length > 1
                ? inputs[1]
                : inputs[0];

            // Get the token type
            TokenType ReturnType = operation == OperationType.Concatenation
                ? TokenType.String
                : ResultType(x, y);
            
            // Perform the operation
            string ReturnString = "";
            switch (operation)
            {
                case OperationType.Addition:
                    ReturnString = Convert.ToString(
                        ReturnType == TokenType.Decimal
                            ? ToDouble(x) + ToDouble(y)
                            : ToInt(x) + ToInt(y)
                        );
                    break;

                case OperationType.Subraction:
                    ReturnString = Convert.ToString(
                        ReturnType == TokenType.Decimal
                            ? ToDouble(x) - ToDouble(y)
                            : ToInt(x) - ToInt(y)
                        );
                    break;

                case OperationType.Multiplication:
                    ReturnString = Convert.ToString(
                        ReturnType == TokenType.Decimal
                            ? ToDouble(x) * ToDouble(y)
                            : ToInt(x) * ToInt(y)
                        );
                    break;

                case OperationType.Division:
                    // division always returns decimal
                    ReturnType = TokenType.Decimal;
                    ReturnString = Convert.ToString(ToDouble(x) / ToDouble(y));
                    break;
                
                case OperationType.Modulo:
                    ReturnString = Convert.ToString(
                        ReturnType == TokenType.Decimal
                            ? ToDouble(x) % ToDouble(y)
                            : ToInt(x) % ToInt(y)
                        );
                    break;

                case OperationType.Concatenation:
                    string a = x.Type == TokenType.String
                        ? x.Value
                        : Convert.ToString(x.Value);
                    string b = y.Type == TokenType.String
                        ? y.Value
                        : Convert.ToString(y.Value);
                    ReturnString = a + b;
                    break;
                
                case OperationType.Exponent:
                    ReturnString = Convert.ToString(
                        ReturnType == TokenType.Decimal
                            ? Math.Pow(ToDouble(x), ToDouble(y))
                            : Math.Pow(ToInt(x), ToInt(y))
                    );
                    break;

                case OperationType.AND:
                    ReturnType = TokenType.Bool;
                    ReturnString = Convert.ToString(ToBool(x) && ToBool(y));
                    break;
                    
                case OperationType.OR:
                    ReturnType = TokenType.Bool;
                    ReturnString = Convert.ToString(ToBool(x) || ToBool(y));
                    break;

                case OperationType.XOR:
                    ReturnType = TokenType.Bool;
                    ReturnString = Convert.ToString(ToBool(x) && !ToBool(y) || (!ToBool(x) && ToBool(y)));
                    break;
                    
                case OperationType.NOT:
                    ReturnType = TokenType.Bool;
                    ReturnString = Convert.ToString(!ToBool(x));
                    break;

                case OperationType.Equal:
                    ReturnType = TokenType.Bool;
                    ReturnString = Convert.ToString(Equal(x, y));
                    break;
                
                case OperationType.Inequal:
                    ReturnType = TokenType.Bool;
                    ReturnString = Convert.ToString(!Equal(x, y));
                    break;
                    
                case OperationType.Greater:
                    ReturnType = TokenType.Bool;
                    ReturnString = Convert.ToString(Greater(x, y));
                    break;
                    
                case OperationType.Less:
                    ReturnType = TokenType.Bool;
                    ReturnString = Convert.ToString(Less(x, y));
                    break;
                    
                case OperationType.GreaterEqual:
                    ReturnType = TokenType.Bool;
                    ReturnString = Convert.ToString(!Less(x, y));
                    break;
                    
                case OperationType.LessEqual:
                    ReturnType = TokenType.Bool;
                    ReturnString = Convert.ToString(!Greater(x, y));
                    break;
            }

            // Return the build token
            return new Token(ReturnType, ReturnString);
        }

        #region Internal Utility Methods

        /// <summary>
        /// Returns the resulting type of two tokens
        /// </summary>
        private static TokenType ResultType(Token x, Token y) =>
            x.Type == TokenType.Decimal || y.Type == TokenType.Decimal
                ? TokenType.Decimal
                : x.Type == TokenType.String || y.Type == TokenType.String
                    ? TokenType.String
                    : TokenType.Integer;

        private static double ToDouble(Token x)
        {
            return Convert.ToDouble(x.Value);
        }

        private static int ToInt(Token x)
        {
            return Convert.ToInt32(x.Value);
        }

        private static bool ToBool(Token x)
        {
            return x.Type == TokenType.Bool
                ? x.Value.Substring(0, 1).ToLower() == "t"
                : x.Type != TokenType.Zilch;
        }

        private static bool Equal(Token x, Token y)
        {
            if(x.Type == TokenType.Bool || y.Type == TokenType.Bool)
            {
                return ToBool(x) == ToBool(y);
            }
            else if(x.Type == y.Type || x.Type == TokenType.String || y.Type == TokenType.String)
            {
                return x.Value == y.Value;
            }
            else if((x.Type == TokenType.Decimal && y.Type == TokenType.Integer) || (x.Type == TokenType.Integer && y.Type == TokenType.Decimal))
            {
                return ToDouble(x) == ToDouble(y);
            }

            return true;
        }

        private static bool Greater(Token x, Token y)
        {
            if(x.Type == TokenType.Decimal || y.Type == TokenType.Decimal)
            {
                return ToDouble(x) > ToDouble(y);
            }
            else
            {
                return ToInt(x) > ToInt(y);
            }
        }

        private static bool Less(Token x, Token y)
        {
            if(x.Type == TokenType.Decimal || y.Type == TokenType.Decimal)
            {
                return ToDouble(x) < ToDouble(y);
            }
            else
            {
                return ToInt(x) < ToInt(y);
            }
        }

        #endregion Internal Utility Methods

        #region Operator Overloads

        public static Token operator +(Token x, Token y) =>
            Operation(ResultType(x, y) == TokenType.String
                          ? OperationType.Concatenation
                          : OperationType.Addition,
                x, y);

        public static Token operator -(Token x, Token y) =>
            Operation(OperationType.Subraction, x, y);

        public static Token operator *(Token x, Token y) =>
            Operation(OperationType.Multiplication, x, y);

        public static Token operator /(Token x, Token y) =>
            Operation(OperationType.Division, x, y);

        public static Token operator %(Token x, Token y) =>
            Operation(OperationType.Modulo, x, y);

        #endregion Operator Overloads
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
        Do,
        End,
        Zilch,
        Bool,
        Integer,
        Decimal,
        ID,
        Error,
        Blank
    }

    /// <summary>
    /// Possible types of operations
    /// </summary>
    public enum OperationType
    {
        Addition,
        Subraction,
        Multiplication,
        Division,
        Modulo,
        Concatenation,
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
        LessEqual
    }
}