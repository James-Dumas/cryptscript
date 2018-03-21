using System;
using System.Collections.Generic;

namespace CryptScript
{
    public class Parser
    {
        private IdentifierGroup IDs { get; set; }
        private bool inLoop { get; set; } = false;

        public Parser(IdentifierGroup ids)
        {
            IDs = ids;
            BuiltIn.AddBuiltIns(IDs);
        }

        public IObject Parse(List<Token> tokens, bool inFunc)
        {
            IObject result = null;

            for(int i = 0; i < tokens.Count; i++)
            {
                // check for unknown tokens
                if(tokens[i].Type == TokenType.Unknown)
                {
                    Interpreter.ThrowError(new Error(ErrorType.UnknownTokenError));
                    return null;
                }

                // remove comments
                if(tokens[i].Type == TokenType.Comment)
                {
                    tokens.RemoveAt(i);
                }
            }

            if(Interpreter.WalletAddr.Reference is Zilch)
            {
                result = new Error(ErrorType.NoWalletAddressError);
            }
            else
            {
                // mine until you get a block
                // NOTE: insert reference to miner here

                if(tokens.Count > 0)
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
                    else if(tokens.Count > 3 && tokens[0].Type == TokenType.Func && tokens[1].Type == TokenType.ID && tokens[2].Type == TokenType.LeftParenthesis && tokens[tokens.Count - 1].Type == TokenType.RightParenthesis)
                    {
                        // check for function declaration
                        List<string> args = new List<string>() {};
                        if(tokens[3].Type != TokenType.RightParenthesis)
                        {
                            foreach(List<Token> arg in ParseCommaSyntax(tokens.GetRange(3, tokens.Count - 4)))
                            {
                                if(arg.Count > 1 || arg[0].Type != TokenType.ID)
                                {
                                    return new Error(ErrorType.SyntaxError);
                                }

                                args.Add(arg[0].Value);
                            }
                        }

                        // NOTE: WIP code
                        // TODO: allow function declaration by waiting for more tokenized lines of code
                        //       and storing them until end token is found

                        List<List<Token>> code = new List<List<Token>>();


                        IDs.SetReference(tokens[1].Value, new Routine(code, args));
                    }
                    /*
                    else if(tokens[0].Type == TokenType.If && tokens[tokens.Count - 1].Type == TokenType.Do)
                    {
                        // check for if statement

                        if(Expression.ToBool(EvaluateExpression(tokens.GetRange(1, tokens.Count - 2)).Result()))
                        {

                        }
                        else
                        {

                        }
                    }
                    */
                    else if(tokens.Count > 1 && tokens[0].Type == TokenType.Return && inFunc)
                    {
                        // check for return keyword
                        result = EvaluateExpression(tokens.GetRange(1, tokens.Count - 1)).Result();
                    }
                    else
                    {
                        result = EvaluateExpression(tokens).Result();
                        // if in a function, still evaluate but don't return the result
                        if(inFunc)
                        {
                            result = null;
                        }
                    }
                }
            }

            if(tokens[0].Type == TokenType.Wallet)
            {
                // check for wallet address
                Interpreter.WalletAddr.Reference = new String(tokens[0].Value);
                result = null;
            }

            if(result is Error)
            {
                Interpreter.ThrowError((Error) result);
                return null;
            }

            return result;
        }

        public IExpression EvaluateExpression(List<Token> expression)
        {
            // DEBUG
            /*
            string output = "Parsing: ";
            foreach(Token t in expression)
            {
                output += t.ToString() + " ";
            }
            Console.WriteLine(output);
            */
            // DEBUG

            if(Interpreter.StopExecution)
            {
                return new BaseExpression(null);
            }

            while(expression.Count > 0 && expression[0].Type == TokenType.LeftParenthesis && expression[expression.Count - 1].Type == TokenType.RightParenthesis)
            {
                // remove parentheses around expression
                bool hasSurroundingParens = true;
                int numParens = 0;
                foreach(Token t in expression.GetRange(1, expression.Count - 2))
                {
                    if(t.Type == TokenType.LeftParenthesis)
                    {
                        numParens++;
                    }
                    if(t.Type == TokenType.RightParenthesis)
                    {
                        if(numParens > 0)
                        {
                            numParens--;
                        }
                        else
                        {
                            hasSurroundingParens = false;
                        }
                    }
                }

                if(hasSurroundingParens)
                {
                    expression = expression.GetRange(1, expression.Count - 2);
                }
                else
                {
                    break;
                }
            }

            for(int i = 0; i < expression.Count - 2; i++)
            {
                if(expression[i].Type == TokenType.ID && expression[i+1].Type == TokenType.LeftParenthesis)
                {
                    // find ending parenthesis
                    int parenEnd = i + 2;
                    int numParens = 0;
                    while(true)
                    {
                        if(expression[parenEnd].Type == TokenType.LeftParenthesis)
                        {
                            numParens++;
                        }
                        else if(expression[parenEnd].Type == TokenType.RightParenthesis)
                        {
                            if(numParens > 0)
                            {
                                numParens--;
                            }
                            else
                            {
                                break;
                            }
                        }

                        parenEnd++;
                        if(parenEnd >= expression.Count)
                        {
                            return new BaseExpression(new Error(ErrorType.SyntaxError));
                        }
                    }

                    // try to call function
                    IObject calledObj = IDs.GetReference(expression[i].Value);
                    if(calledObj is ICallable)
                    {
                        List<IObject> args = new List<IObject>() {};

                        if(parenEnd > i + 2)
                        {
                            foreach(List<Token> argExpression in ParseCommaSyntax(expression.GetRange(i+2, parenEnd - i - 2)))
                            {
                                IObject arg = EvaluateExpression(argExpression).Result();
                                if(arg is Error)
                                {
                                    Interpreter.ThrowError((Error) arg);
                                    return new BaseExpression(null);
                                }
                                args.Add(arg);
                            }
                        }

                        int num = 0;
                        while(IDs.HasID("r-" + num.ToString()))
                        {
                            num++;
                        }

                        IDs.SetReference("r-" + num.ToString(), ((ICallable) calledObj).Call(args));
                        expression.RemoveRange(i, parenEnd - i + 1);
                        expression.Insert(i, new Token(TokenType.ID, "r-" + num.ToString()));
                    }
                    else
                    {
                        return new BaseExpression(new Error(ErrorType.IdNotCallableError));
                    }
                }
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

                    IExpression result;
                    if(index >= 0)
                    {
                        if(expression[index].Type == TokenType.NOT)
                        {
                            result = new Expression(EvaluateExpression(expression.GetRange(index + 1, expression.Count - index - 1)),
                                                new BaseExpression(new Zilch()),
                                                OperationType.NOT);
                        }
                        else
                        {
                            result = new Expression(EvaluateExpression(expression.GetRange(0, index)),
                                                EvaluateExpression(expression.GetRange(index + 1, expression.Count - index - 1)),
                                                Token.OperationOf[expression[index].Type]);
                        }
                    }
                    else
                    {
                        result = new BaseExpression(null);
                    }

                    int num = 0;
                    while(IDs.HasID("r-" + num.ToString()))
                    {
                        IDs.RemoveID("r-" + num.ToString());
                        num++;
                    }

                    return result;

            }
        }

        public static List<List<Token>> ParseCommaSyntax(List<Token> expression)
        {
            List<List<Token>> result = new List<List<Token>>();
            int start = 0;
            for(int i = 0; i <= expression.Count; i++)
            {
                if(i == expression.Count || expression[i].Type == TokenType.Comma)
                {
                    result.Add(expression.GetRange(start, i - start));
                    start = i + 1;
                }
            }

            return result;
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