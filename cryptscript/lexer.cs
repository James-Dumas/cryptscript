using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace cryptscript
{
    public class Lexer
    {
        #region Public Fields

        public static Dictionary<string, TokenType> Tokens { get; } = new Dictionary<string, TokenType>()
        {
            { " *#![a-zA-Z0-9]+",                 TokenType.Wallet },
            { @" *""((\\"")|[^""])*""",           TokenType.String },
            { @" *'((\\')|[^'])*'",               TokenType.String },
            { @" *\+",                            TokenType.Addition },
            { " *-",                              TokenType.Subraction },
            { @" *\*",                            TokenType.Multiplication },
            { " */",                              TokenType.Division },
            { " *%",                              TokenType.Modulo },
            { @" *\^",                            TokenType.Exponent },
            { " *&&",                             TokenType.AND },
            { " *(?i)(and)",                      TokenType.AND },
            { @" *\|\|",                          TokenType.OR },
            { " *(?i)(or)",                       TokenType.OR },
            { @" *\|\|>",                         TokenType.XOR },
            { " *(?i)(xor)",                      TokenType.XOR },
            { " *!",                              TokenType.NOT },
            { " *(?i)(not)",                      TokenType.NOT },
            { " *==",                             TokenType.Equal },
            { " *!=",                             TokenType.Inequal },
            { " *>",                              TokenType.Greater },
            { " *<",                              TokenType.Less },
            { " *>=",                             TokenType.GreaterEqual },
            { " *<=",                             TokenType.LessEqual },
            { " *=",                              TokenType.Set },
            { @" *\(",                            TokenType.LeftParenthesis },
            { @" *\)",                            TokenType.RightParenthesis },
            { @" *\[",                            TokenType.LeftBracket },
            { @" *\]",                            TokenType.RightBracket },
            { @" *\{",                            TokenType.LeftCurly },
            { @" *\}",                            TokenType.RightCurly },
            { " *,",                              TokenType.Comma },
            { " *:",                              TokenType.Colon },
            { " *~[^#]*",                         TokenType.Comment },
            { " *(?i)(given that)",               TokenType.If },
            { " *(?i)(do)",                       TokenType.Do },
            { " *(?i)(otherwise)",                TokenType.Else },
            { " *(?i)(if however)",               TokenType.Elif },
            { " *(?i)(as long as)",               TokenType.While },
            { " *(?i)(through)",                  TokenType.For },
            { " *(?i)(routine)",                  TokenType.Func },
            { " *(?i)(concur)",                   TokenType.End },
            { " *(?i)(cease)",                    TokenType.Break },
            { " *(?i)(submit)",                   TokenType.Return },
            { " *(?i)(attempt)",                  TokenType.Try },
            { " *(?i)(upon)",                     TokenType.Catch },
            { " *(?i)(utilize)",                  TokenType.Import },
            { " *(?i)(zilch)",                    TokenType.Zilch },
            { " *(?i)(true)",                     TokenType.Bool },
            { " *(?i)(false)",                    TokenType.Bool },
            { @" *\d*\.\d+",                      TokenType.Decimal },
            { @" *\d+",                           TokenType.Integer },
            { " *[A-Za-z][A-Za-z0-9_]*",          TokenType.ID },
            { " *.",                              TokenType.Unknown },
        };

        #endregion Public Fields

        /// <summary>
        /// Convert string of text into tokens
        /// </summary>
        /// <returns>List of <see cref="Token"/>s</returns>
        public List<Token> Tokenize(string text)
        {
            List<Token> tokens = new List<Token>();

            for (int i = 0; i < text.Length;)
            {
                Token newToken = new Token(TokenType.Unknown, "");
                bool match = false;
                bool error = false;
                int end = text.Length;

                while (!match && !error)
                {
                    // try to match every token's regex to a substring of the text that is decreasing in length
                    string subStr = text.Substring(i, end - i);
                    error = end <= i;
                    if (error) { break; }

                    foreach (KeyValuePair<string, TokenType> item in Tokens)
                    {
                        string tokenStr = Regex.Match(subStr, item.Key).Value;
                        match = tokenStr == subStr;

                        if (match)
                        {
                            // add a new token with the value found at the matched regex
                            newToken = new Token(item.Value, subStr);
                            i += subStr.Length;
                            break;
                        }
                    }

                    end--;
                }

                tokens.Add(newToken);
                if(error) { break; }
            }

            return tokens;
        }
    }

    public class Line
    {
        // basic container class for keeping track of line numbers
        public List<Token> Tokens { get; set; }
        public int LineNum { get; set; }

        public Line(List<Token> tokens, int lineNum)
        {
            Tokens = tokens;
            LineNum = lineNum;
        }
    }
}