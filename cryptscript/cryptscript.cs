using System;
using System.Collections.Generic;

namespace CryptScript
{
    public class Interpreter
    {
        public static Token GetTokenResult(List<Token> expression)
        {
            Token newToken = new Token(TokenType.Error, "SyntaxError");
            switch(expression.Count)
            {
                case 1:
                    newToken = expression[0];
                    break;

                case 2:
                    if(Token.OperationOf.ContainsKey(expression[0].Type))
                    {
                        if(expression[0].Type == TokenType.Subraction)
                        {
                            // parse negative numbers
                            newToken = Token.Operation(OperationType.Subraction, new Token(TokenType.Integer, "0"), expression[1]);
                        }
                        else if(expression[0].Type == TokenType.NOT)
                        {
                            // not operation
                            newToken = Token.Operation(OperationType.NOT, expression[1]);
                        }
                    }

                    break;

                case 3:
                    if(Token.OperationOf.ContainsKey(expression[1].Type))
                    {
                        // this is where operations are actually performed

                        if(expression[0].Type == TokenType.String && expression[1].Type != TokenType.Addition)
                        {
                            // return error if string is followed by a math operation
                            newToken = new Token(TokenType.Error, "TypeMismatch");
                        }
                        else if(expression[0].Type == TokenType.Subraction)
                        {
                            // find negative numbers
                            newToken = Token.Operation(OperationType.Subraction, new Token(TokenType.Integer, "0"), expression[1]);
                        }
                        else if(expression[0].Type == TokenType.NOT)
                        {
                            // not operation
                            newToken = Token.Operation(OperationType.NOT, expression[1]);
                        }
                        else
                        {
                            // perform operations that use two tokens
                            OperationType opType;

                            if(expression[1].Type == TokenType.Addition)
                            {
                                // determine concatenation or addition for plus token
                                opType = expression[0].Type == TokenType.String
                                    ? OperationType.Concatenation
                                    : OperationType.Addition;
                            }
                            else
                            {
                                opType = Token.OperationOf[expression[1].Type];
                            }

                            newToken = Token.Operation(opType, expression[0], expression[2]);
                        }
                    }

                    break;

                default:
                    // token list is split into smaller chunks based on order of operations and simplified down to one token

                    int pointer = 0;

                    // search through left to right for parentheses
                    int parenStart = -1;
                    int parensBetween = 0;
                    while(pointer < expression.Count)
                    {
                        if(expression[pointer].Type == TokenType.LeftParenthesis)
                        {
                            if(parenStart == -1)
                            {
                                parenStart = pointer;
                            }
                            else
                            {
                                parensBetween++;
                            }
                        }
                        else if(expression[pointer].Type == TokenType.RightParenthesis)
                        {
                            // evaluate tokens in parentheses recursively and replace them in the list with their result
                            if(parensBetween == 0)
                            {
                                Token evalToken = GetTokenResult(expression.GetRange(parenStart + 1, pointer - parenStart - 1));
                                expression.RemoveRange(parenStart, pointer - parenStart + 1);
                                expression.Insert(parenStart, evalToken);
                                pointer = -1;
                                parenStart = -1;
                            }
                            else
                            {
                                parensBetween--;
                            }
                        }

                        pointer++;
                    }

                    pointer = 1;

                    // for exponents
                    while(pointer < expression.Count - 1)
                    {
                        if(expression[pointer].Type == TokenType.Exponent)
                        {
                            Token evalToken = GetTokenResult(expression.GetRange(pointer - 1, 3));
                            expression.RemoveRange(pointer - 1, 3);
                            expression.Insert(pointer - 1, evalToken);
                            pointer = 0;
                        }

                        pointer++;
                    }

                    pointer = 1;

                    // for multiplication/division
                    while(pointer < expression.Count - 1)
                    {
                        if(new List<TokenType> {TokenType.Multiplication, TokenType.Division, TokenType.Modulo}.Contains(expression[pointer].Type))
                        {
                            Token evalToken = GetTokenResult(expression.GetRange(pointer - 1, 3));
                            expression.RemoveRange(pointer - 1, 3);
                            expression.Insert(pointer - 1, evalToken);
                            pointer = 0;
                        }

                        pointer++;
                    }

                    pointer = 1;

                    // for addition/subtraction
                    while(pointer < expression.Count - 1)
                    {
                        if(expression[pointer].Type == TokenType.Addition || expression[pointer].Type == TokenType.Subraction)
                        {
                            Token evalToken = GetTokenResult(expression.GetRange(pointer - 1, 3));
                            expression.RemoveRange(pointer - 1, 3);
                            expression.Insert(pointer - 1, evalToken);
                            pointer = 0;
                        }

                        pointer++;
                    }

                    pointer = 1;

                    // for boolean operations
                    while(pointer < expression.Count - 1)
                    {
                        if(new List<TokenType> {TokenType.AND, TokenType.OR, TokenType.XOR, TokenType.Equal, TokenType.Inequal,
                                                TokenType.Greater, TokenType.Less, TokenType.GreaterEqual, TokenType.LessEqual}.Contains(expression[pointer].Type))
                        {
                            Token evalToken = GetTokenResult(expression.GetRange(pointer - 1, 3));
                            expression.RemoveRange(pointer - 1, 3);
                            expression.Insert(pointer - 1, evalToken);
                            pointer = 0;
                        }

                        pointer++;
                    }

                    newToken = expression.Count > 1
                        ? new Token(TokenType.Error, "SyntaxError")
                        : expression[0];
                    
                    break;
            }

            return newToken;
        }

        public static void Main(String[] args)
        {
            Console.WriteLine("Type 'exit' to quit.");

            while (true)
            {
                Console.Write(">> ");

                string input = Console.ReadLine().Trim();
                if (input == "exit")
                {
                    break;
                }

                Lexer lexer = new Lexer(input);
                Token result = GetTokenResult(lexer.Tokenize());
                Console.WriteLine(result.ToString());
            }
        }
    }
}