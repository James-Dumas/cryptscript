using System;

namespace CryptScript
{
    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value.Trim();

            // Remove quotes around strings
            if (type == TokenType.String)
            {
                Value = Value.Substring(1, Value.Length - 2);
            }
        }

        /// <summary>
        /// Returns a formatted representation of the token
        /// </summary>
        public override string ToString()
        {
            return String.Format("({0}, '{1}')", Type.ToString(), Value);
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

        /// <summary>
        /// Performs an operation on two tokens
        /// </summary>
        /// <param name="operation">The operation to perform</param>
        /// <param name="x">The first token</param>
        /// <param name="y">The second token</param>
        /// <returns></returns>
        private static Token Operation(OperationType operation, Token x, Token y)
        {
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
                        ResultType(x, y) == TokenType.Decimal
                            ? Convert.ToDouble(x.Value) + Convert.ToDouble(y.Value)
                            : Convert.ToInt32(x.Value) + Convert.ToInt32(y.Value));
                    break;

                case OperationType.Subraction:
                    ReturnString = Convert.ToString(
                        ResultType(x, y) == TokenType.Decimal
                            ? Convert.ToDouble(x.Value) - Convert.ToDouble(y.Value)
                            : Convert.ToInt32(x.Value) - Convert.ToInt32(y.Value));
                    break;

                case OperationType.Multiplication:
                    ReturnString = Convert.ToString(
                        ResultType(x, y) == TokenType.Decimal
                            ? Convert.ToDouble(x.Value) * Convert.ToDouble(y.Value)
                            : Convert.ToInt32(x.Value) * Convert.ToInt32(y.Value));
                    break;

                case OperationType.Division:
                    ReturnString = Convert.ToString(
                        ResultType(x, y) == TokenType.Decimal
                            ? Convert.ToDouble(x.Value) / Convert.ToDouble(y.Value)
                            : Convert.ToInt32(x.Value) / Convert.ToInt32(y.Value));
                    break;

                case OperationType.Concatenation:
                    string a = x.Type == TokenType.String
                        ? x.Value.Substring(1, x.Value.Length - 2)
                        : Convert.ToString(x.Value);
                    string b = y.Type == TokenType.String
                        ? y.Value.Substring(1, y.Value.Length - 2)
                        : Convert.ToString(y.Value);
                    ReturnString = a + b;
                    break;
            }

            // Return the build token
            return new Token(ReturnType, ReturnString);
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
        While,
        For,
        Do,
        End,
        True,
        False,
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
        Concatenation
    }
}