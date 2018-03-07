using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CryptScript
{
    public class Lexer
    {
        #region Public Fields

        public string Text
        {
            get { return Text; }
            set { Text = value.Trim(); }
        }

        public static Dictionary<string, TokenType> Tokens = new Dictionary<string, TokenType>()
        {
            { " *#![a-zA-Z0-9]+$",          TokenType.Wallet },
            { " *\"[^\"]*\"",               TokenType.String },
            { " *'[^']*'",                  TokenType.String },
            { @" *\+",                      TokenType.Addition },
            { " *-",                        TokenType.Subraction },
            { @" *\*",                      TokenType.Multiplication },
            { " */",                        TokenType.Division },
            { " *%",                        TokenType.Modulo },
            { @" *\^",                      TokenType.Exponent },
            { " *&&",                       TokenType.AND },
            { " +(?i)(and) +",              TokenType.AND },
            { @" *\|\|",                    TokenType.OR },
            { " +(?i)(or) +",               TokenType.OR },
            { @" *\|\|>",                   TokenType.XOR },
            { " +(?i)(xor) +",              TokenType.XOR },
            { " *!",                        TokenType.NOT },
            { " +(?i)(not) +",              TokenType.NOT },
            { " *==",                       TokenType.Equal },
            { " *!=",                       TokenType.Inequal },
            { " *>",                        TokenType.Greater },
            { " *<",                        TokenType.Less },
            { " *>=",                       TokenType.GreaterEqual },
            { " *<=",                       TokenType.LessEqual },
            { " *=",                        TokenType.Set },
            { @" *\(",                      TokenType.LeftParenthesis },
            { @" *\)",                      TokenType.RightParenthesis },
            { @" *\[",                      TokenType.LeftBracket },
            { @" *\]",                      TokenType.RightBracket },
            { @" *\{",                      TokenType.LeftCurly },
            { @" *\}",                      TokenType.RightCurly },
            { " *,",                        TokenType.Comma },
            { " *~[^#]*",                   TokenType.Comment },
            { " *#[a-zA-Z0-9]+$",           TokenType.Buffer },
            { " *(?i)(if)",                 TokenType.If },
            { " *(?i)(then)",               TokenType.Then },
            { " *(?i)(else)",               TokenType.Else },
            { " *(?i)(while)",              TokenType.While },
            { " *(?i)(for)",                TokenType.For },
            { " *(?i)(do)",                 TokenType.Do },
            { " *(?i)(end)",                TokenType.End },
            { " *(?i)(true)",               TokenType.True },
            { " *(?i)(false)",              TokenType.False },
            { @" *\d*\.\d+",                TokenType.Decimal },
            { @" *\d+",                     TokenType.Integer },
            { " *[A-Za-z][A-Za-z0-9_]*",    TokenType.ID },
        };

        #endregion Public Fields

        public Lexer(string text)
        {
            Text = text.Trim();
        }

        /// <summary>
        /// Convert string of text into tokens
        /// </summary>
        /// <returns>List of <see cref="Token"/>s</returns>
        public List<Token> Tokenize()
        {
            List<Token> tokens = new List<Token>();
            Token errorToken = new Token(TokenType.Error, "SyntaxError");

            for (int i = 0; i < Text.Length;)
            {
                Token newToken = new Token(TokenType.Blank, "");
                bool match = false;
                bool error = false;
                int end = Text.Length;

                while (!match && !error)
                {
                    // try to match every token's regex to a substring of the text that is decreasing in length
                    string subStr = Text.Substring(i, end - i);
                    error = end <= i;

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

                // return syntax error when text can't be tokenized
                if (error)
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