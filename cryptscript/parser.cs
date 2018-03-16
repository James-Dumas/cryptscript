using System;
using System.Collections.Generic;

namespace CryptScript
{
    public class Parser
    {
        private IdentifierGroup IDs { get; set; }
        private Identifier walletAddr { get; set; }

        public Parser()
        {
            walletAddr = new Identifier();
            IDs = new IdentifierGroup();
        }

        public IObject Parse(List<Token> tokens)
        {
            IObject result = null;

            for(int i = 0; i < tokens.Count; i++)
            {
                // check for unknown tokens
                if(tokens[i].Type == TokenType.Unknown)
                {
                    return new Error(ErrorType.UnknownTokenError);
                }

                // remove comments
                if(tokens[i].Type == TokenType.Comment)
                {
                    tokens.RemoveAt(i);
                }
            }

            if(walletAddr.Reference is Zilch)
            {
                result = new Error(ErrorType.NoWalletAddressError);
            }
            else
            {
                if(tokens.Count > 1 && tokens[0].Type == TokenType.ID && tokens[1].Type == TokenType.Set)
                {
                    // check for 'variable = expression' syntax

                    IObject expressionResult = EvaluateExpression(tokens.GetRange(2, tokens.Count - 2)).Result(); 
                    string IdName = tokens[0].Value;
                    if(expressionResult is Error)
                    {
                        result = expressionResult;
                    }
                    else
                    {
                        IDs.SetReference(IdName, expressionResult);
                    }
                }
                else
                {
                    result = EvaluateExpression(tokens).Result();
                }
            }

            if(tokens[0].Type == TokenType.Wallet)
            {
                // check for wallet address
                walletAddr.Reference = new String(tokens[0].Value);
                result = null;
            }

            return result;
        }

        public IExpression EvaluateExpression(List<Token> expression)
        {
            // DEBUG
            string output = "Parsing: ";
            foreach(Token t in expression)
            {
                output += t.ToString() + " ";
            }
            Console.WriteLine(output);
            // DEBUG

            while(expression.Count > 0 && expression[0].Type == TokenType.LeftParenthesis && expression[expression.Count - 1].Type == TokenType.RightParenthesis)
            {
                // remove parentheses around expression
                expression = expression.GetRange(1, expression.Count - 2);
            }

            switch(expression.Count)
            {
                case 0:
                    return new BaseExpression(new Error(ErrorType.SyntaxError));

                case 1:
                    return new BaseExpression(CreateFromToken(expression[0]));
                
                case 2:
                    switch(expression[0].Type)
                    {
                        case TokenType.Subraction:
                            return new Expression(new BaseExpression(CreateObject(0)),
                                                  new BaseExpression(CreateFromToken(expression[1])),
                                                  OperationType.Subraction);

                        case TokenType.NOT:
                            return new Expression(new BaseExpression(CreateFromToken(expression[1])),
                                                  new BaseExpression(CreateFromToken(expression[1])),
                                                  OperationType.NOT);

                        default:
                            return new BaseExpression(new Error(ErrorType.SyntaxError));

                    }
                
                case 3:
                    if(!Token.OperationOf.ContainsKey(expression[1].Type))
                    {
                        return new BaseExpression(new Error(ErrorType.SyntaxError));
                    }
                    else
                    {
                        // perform operations that use two tokens
                        return new Expression(new BaseExpression(CreateFromToken(expression[0])),
                                              new BaseExpression(CreateFromToken(expression[2])),
                                              Token.OperationOf[expression[1].Type]);
                    }
                
                default:
                    
                    // create Expression instance from tokens by parsing expression tokens in reverse order of operations
                    // which is: Boolean, Addition/Subtraction, Multiplication/Division, Exponent, Parentheses
                    // within boolean: AND/OR/XOR, Equal/Inequal, Greater/Lesser, NOT

                    int numParens = 0;
                    int index = -1;
                    int step = 0;
                    while(index == -1 && step <= 6)
                    {
                        for(int i = expression.Count - 1; i >= 0; i--)
                        {
                            if(expression[i].Type == TokenType.RightParenthesis)
                            {
                                numParens++;
                            }
                            if(numParens > 0)
                            {
                                if(expression[i].Type == TokenType.LeftParenthesis)
                                {
                                    numParens--;
                                }
                            }
                            else
                            {
                                if((step == 0 && (new List<TokenType> {TokenType.AND, TokenType.OR, TokenType.XOR}.Contains(expression[i].Type)))
                                || (step == 1 && (expression[i].Type == TokenType.Equal || expression[i].Type == TokenType.Inequal))
                                || (step == 2 && (new List<TokenType> {TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual}.Contains(expression[i].Type)))
                                || (step == 3 && (expression[i].Type == TokenType.NOT))
                                || (step == 4 && (expression[i].Type == TokenType.Addition || expression[i].Type == TokenType.Subraction))
                                || (step == 5 && (new List<TokenType> {TokenType.Multiplication, TokenType.Division, TokenType.Modulo}.Contains(expression[i].Type)))
                                || (step == 6 && (expression[i].Type == TokenType.Exponent)))
                                {
                                    index = i;
                                    break;
                                }
                            }
                        }

                        step++;
                    }

                    if(expression[index].Type == TokenType.NOT)
                    {
                        return new Expression(EvaluateExpression(expression.GetRange(index + 1, expression.Count - index - 1)),
                                              new BaseExpression(new Zilch()),
                                              OperationType.NOT);
                    }
                    else
                    {
                        return new Expression(EvaluateExpression(expression.GetRange(0, index)),
                                              EvaluateExpression(expression.GetRange(index + 1, expression.Count - index - 1)),
                                              Token.OperationOf[expression[index].Type]);
                    }

            }
        }

        public IObject CreateFromToken(Token t)
        {
            IObject result = new Error(ErrorType.SyntaxError);

            switch(t.Type)
            {
                case TokenType.ID:
                    result = IDs.GetReference(t.Value);
                    break;

                case TokenType.Integer:
                    result = new Integer(t.Value);
                    break;

                case TokenType.Decimal:
                    result = new Decimal(t.Value);
                    break;
                
                case TokenType.String:
                    result = new String(t.Value);
                    break;

                case TokenType.Bool:
                    result = new Boolean(t.Value.ToLower());
                    break;

                case TokenType.Zilch:
                    result = new Zilch();
                    break;
            }

            return result;
        }

        public static IObject CreateObject(object value)
        {
            IObject result = new Zilch();
            Type t = value.GetType();
            if(value == null)
            {
                result = new Zilch();
            }
            else if(t.Equals(typeof(int)))
            {
                result = new Integer(value);
            }
            else if(t.Equals(typeof(double)))
            {
                result = new Decimal(value);
            }
            else if(t.Equals(typeof(string)))
            {
                result = new String(value);
            }
            else if(t.Equals(typeof(bool)))
            {
                result = new Boolean(value);
            }

            return result;
        }
    }
}