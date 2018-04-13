using System;
using System.Collections.Generic;

namespace cryptscript
{
    public class Parser
    {
        private IdentifierGroup IDs { get; set; }
        public bool parsingLoop { get; set; } = false;
        public bool parsingFunc { get; set; } = false;
        public bool doneParsing { get; set; } = true;
        private List<List<Token>> storedCode { get; set; }
        private List<string> storedArgs { get; set; }
        private string routineName { get; set; }
        private TokenType loopType { get; set; }
        private List<Token> loopCondition { get; set; }
        private int endsNeeded { get; set; } = 0;

        private static bool breakOut { get; set; } = false;

        public Parser(IdentifierGroup ids)
        {
            IDs = ids;
            BuiltIn.AddBuiltIns(IDs);
        }

        public IObject Parse(List<Token> tokens, bool inSubParser)
        {
            if(Interpreter.ErrorMsg != null)
            {
                return null;
            }

            IObject result = null;

            for(int i = 0; i < tokens.Count; i++)
            {
                // check for unknown tokens
                if(tokens[i].Type == TokenType.Unknown)
                {
                    Interpreter.ThrowError(new Error(ErrorType.TokenNotFoundError));
                    return null;
                }

                // remove comments
                if(tokens[i].Type == TokenType.Comment)
                {
                    tokens.RemoveAt(i);
                }
            }

            if((parsingFunc || parsingLoop) && !doneParsing && tokens.Count > 0)
            {
                // get every line of code inputted until an end is given
                if(tokens[0].Type == TokenType.End)
                {
                    if(tokens.Count > 1)
                    {
                        // end token must be alone on a line
                        Interpreter.ThrowError(new Error(ErrorType.SyntaxError));
                    }
                    else
                    {
                        if(endsNeeded > 0)
                        {
                            endsNeeded--;
                        }
                        else
                        {
                            doneParsing = true;
                        }
                    }
                }

                if(new List<TokenType>() {TokenType.If, TokenType.While, TokenType.For}.Contains(tokens[0].Type))
                {
                    endsNeeded++;
                }

                if(!doneParsing)
                {
                    storedCode.Add(tokens);
                }
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
                        storedArgs = new List<string>() {};
                        if(tokens[3].Type != TokenType.RightParenthesis)
                        {
                            foreach(List<Token> arg in ParseCommaSyntax(tokens.GetRange(3, tokens.Count - 4)))
                            {
                                if(arg.Count > 1 || arg[0].Type != TokenType.ID)
                                {
                                    Interpreter.ThrowError(new Error(ErrorType.SyntaxError));
                                    return null;
                                }

                                storedArgs.Add(arg[0].Value);
                            }
                        }

                        routineName = tokens[1].Value;
                        storedCode = new List<List<Token>>();
                        parsingFunc = true;
                        doneParsing = false;
                    }
                    else if(new List<TokenType>() {TokenType.If, TokenType.While, TokenType.For}.Contains(tokens[0].Type) && tokens[tokens.Count - 1].Type == TokenType.Do)
                    {
                        // check for loops
                        loopType = tokens[0].Type;
                        loopCondition = tokens.GetRange(1, tokens.Count - 2);
                        storedCode = new List<List<Token>>();
                        parsingLoop = true;
                        doneParsing = false;
                    }
                    else if(tokens.Count > 1 && tokens[0].Type == TokenType.Return && inSubParser)
                    {
                        // check for return keyword
                        result = EvaluateExpression(tokens.GetRange(1, tokens.Count - 1)).Result();
                    }
                    else if(tokens.Count == 1 && tokens[0].Type == TokenType.Break)
                    {
                        breakOut = true;
                    }
                    else
                    {
                        result = EvaluateExpression(tokens).Result();
                        // if in a function, still evaluate but don't return the result
                        if(inSubParser)
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

            if(doneParsing && parsingFunc)
            {
                // when a function is finished parsing
                IDs.SetReference(routineName, new Routine(storedCode, storedArgs));
                parsingFunc = false;
            }

            if(doneParsing && parsingLoop)
            {
                // when a loop is finished parsing
                Parser loopParser = new Parser(IDs);
                switch(loopType)
                {
                    case TokenType.If:
                        List<List<Token>> ifCode = storedCode;
                        List<List<Token>> elseCode = new List<List<Token>>();
                        for(int i = 0; i < storedCode.Count; i++)
                        {
                            List<Token> line = storedCode[i];
                            // check for else block
                            if(line.Count == 1 && line[0].Type == TokenType.Else)
                            {
                                ifCode = storedCode.GetRange(0, i);
                                elseCode = storedCode.GetRange(i + 1, storedCode.Count - i - 1);
                            }
                        }

                        if(Expression.ToBool(EvaluateExpression(loopCondition).Result()))
                        {
                            storedCode = ifCode;
                        }
                        else if(elseCode.Count > 0)
                        {
                            storedCode = elseCode;
                        }
                        else
                        {
                            storedCode = new List<List<Token>>();
                        }

                        foreach(List<Token> line in storedCode)
                        {
                            result = loopParser.Parse(line, true);
                            if(inSubParser && result != null)
                            {
                                return result;
                            }
                        }

                        break;

                    case TokenType.While:
                        while(Expression.ToBool(EvaluateExpression(loopCondition).Result()))
                        {
                            foreach(List<Token> line in storedCode)
                            {
                                result = loopParser.Parse(new List<Token>(line), true);
                                if(inSubParser && result != null)
                                {
                                    return result;
                                }

                                if(breakOut)
                                {
                                    break;
                                }
                            }

                            if(breakOut)
                            {
                                breakOut = false;
                                break;
                            }
                        }

                        break;
                    
                    case TokenType.For:
                        List<List<Token>> loopArgs = ParseCommaSyntax(loopCondition);
                        if(loopArgs.Count == 0 || (loopArgs.Count > 0 && (loopArgs[0].Count > 1 || loopArgs[0][0].Type != TokenType.ID)))
                        {
                            Interpreter.ThrowError(new Error(ErrorType.SyntaxError));
                            return null;
                        }

                        string varName = loopArgs[0][0].Value;
                        double start = 0;
                        double end;
                        double step = 1;
                        switch(loopArgs.Count)
                        {
                            case 2:
                                end = Expression.ToDouble(EvaluateExpression(loopArgs[1]).Result());
                                break;
                            
                            case 3:
                                start = Expression.ToDouble(EvaluateExpression(loopArgs[1]).Result());
                                end = Expression.ToDouble(EvaluateExpression(loopArgs[2]).Result());
                                break;
                            
                            case 4:
                                start = Expression.ToDouble(EvaluateExpression(loopArgs[1]).Result());
                                end = Expression.ToDouble(EvaluateExpression(loopArgs[2]).Result());
                                step = Expression.ToDouble(EvaluateExpression(loopArgs[3]).Result());
                                break;

                            default:
                                Interpreter.ThrowError(new Error(ErrorType.SyntaxError));
                                return null;
                        }

                        if(step <= 0) 
                        {
                            Interpreter.ThrowError(new Error(ErrorType.InvalidArgumentError));
                            return null;
                        }

                        if(end < start)
                        {
                            step *= -1;
                        }

                        IDs.SetReference(varName, CreateObject(start));

                        while(step > 0 && Expression.ToDouble(IDs.GetReference(varName)) < end || (step < 0 && Expression.ToDouble(IDs.GetReference(varName)) > end))
                        {
                            foreach(List<Token> line in storedCode)
                            {
                                result = loopParser.Parse(new List<Token>(line), true);
                                if(inSubParser && result != null)
                                {
                                    return result;
                                }

                                if(breakOut)
                                {
                                    break;
                                }
                            }

                            if(breakOut)
                            {
                                breakOut = false;
                                break;
                            }

                            IDs.SetReference(varName, CreateObject(Expression.ToDouble(IDs.GetReference(varName)) + step));
                        }

                        break;
                }

                parsingLoop = false;
            }

            return result;
        }

        public IExpression EvaluateExpression(List<Token> expression)
        {
            // DEBUG
            /*
            string output = "";
            foreach(Token t in expression)
            {
                output += t.ToString() + " ";
            }
            Util.DebugLog(output);
            */
            // DEBUG

            if(Interpreter.StopExecution)
            {
                return new Expression((IObject) null);
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
                if(expression[i].Type == TokenType.ID && expression[i + 1].Type == TokenType.LeftParenthesis)
                {
                    // parse as function call

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
                            return new Expression(new Error(ErrorType.SyntaxError));
                        }
                    }

                    Token callAsToken = new TokenGroup(TokenType.CalledFunc, expression.GetRange(i, parenEnd - i + 1));
                    expression.RemoveRange(i, parenEnd - i + 1);
                    expression.Insert(i, callAsToken);
                }
            }

            for(int i = 0; i < expression.Count - 1; i++)
            {
                if((i == 0 || (i > 0 && !(new List<TokenType>() {TokenType.Integer, TokenType.Decimal, TokenType.Bool, 
                    TokenType.String, TokenType.Zilch, TokenType.List, TokenType.Dict, TokenType.ID, TokenType.CalledFunc,
                    TokenType.IndexedObj, TokenType.RightParenthesis, TokenType.RightBracket, TokenType.RightCurly
                    }.Contains(expression[i - 1].Type)))) && expression[i].Type == TokenType.LeftBracket)
                {
                    // parse as explicit list declaration

                    // find ending bracket
                    int brackEnd = i + 1;
                    int numBracks = 0;
                    while(true)
                    {
                        if(expression[brackEnd].Type == TokenType.LeftBracket)
                        {
                            numBracks++;
                        }
                        else if(expression[brackEnd].Type == TokenType.RightBracket)
                        {
                            if(numBracks > 0)
                            {
                                numBracks--;
                            }
                            else
                            {
                                break;
                            }
                        }

                        brackEnd++;
                        if(brackEnd >= expression.Count)
                        {
                            return new Expression(new Error(ErrorType.SyntaxError));
                        }
                    }

                    Token listAsToken = new TokenGroup(TokenType.List, expression.GetRange(i, brackEnd - i + 1));
                    expression.RemoveRange(i, brackEnd - i + 1);
                    expression.Insert(i, listAsToken);
                }
            }

            for(int i = 1; i < expression.Count - 2; i++)
            {
                if(new List<TokenType>() {TokenType.Integer, TokenType.Decimal, TokenType.Bool, TokenType.String, 
                    TokenType.Zilch, TokenType.List, TokenType.Dict, TokenType.ID, TokenType.CalledFunc,
                    TokenType.IndexedObj}.Contains(expression[i - 1].Type) && expression[i].Type == TokenType.LeftBracket)
                {
                    // parse as indexing an object

                    // find ending bracket
                    int brackEnd = i + 1;
                    int numBracks = 0;
                    while(true)
                    {
                        if(expression[brackEnd].Type == TokenType.LeftBracket)
                        {
                            numBracks++;
                        }
                        else if(expression[brackEnd].Type == TokenType.RightBracket)
                        {
                            if(numBracks > 0)
                            {
                                numBracks--;
                            }
                            else
                            {
                                break;
                            }
                        }

                        brackEnd++;
                        if(brackEnd >= expression.Count)
                        {
                            return new Expression(new Error(ErrorType.SyntaxError));
                        }
                    }

                    Token itemAsToken = new TokenGroup(TokenType.IndexedObj, expression.GetRange(i - 1, brackEnd - i + 2));
                    expression.RemoveRange(i - 1, brackEnd - i + 2);
                    expression.Insert(i - 1, itemAsToken);
                    i--;
                }
            }

            //DEBUG
            /*
            string output = "";
            foreach(Token t in expression)
            {
                output += t.ToString() + " ";
            }
            Util.DebugLog(output);
            */
            //DEBUG

            switch(expression.Count)
            {
                case 0:
                    return new Expression(new Error(ErrorType.SyntaxError));

                case 1:
                    return new Expression(CreateFromToken(expression[0]));
                
                case 2:
                    switch(expression[0].Type)
                    {
                        case TokenType.Subraction:
                            return new Expression(new Expression(CreateObject(0)),
                                                  new Expression(CreateFromToken(expression[1])),
                                                  OperationType.Subraction);

                        case TokenType.NOT:
                            return new Expression(new Expression(CreateFromToken(expression[1])),
                                                  new Expression(CreateFromToken(expression[1])),
                                                  OperationType.NOT);

                        default:
                            return new Expression(new Error(ErrorType.SyntaxError));

                    }
                
                case 3:
                    if(!Token.OperationOf.ContainsKey(expression[1].Type))
                    {
                        return new Expression(new Error(ErrorType.SyntaxError));
                    }
                    else
                    {
                        // perform operations that use two tokens
                        return new Expression(new Expression(CreateFromToken(expression[0])),
                                              new Expression(CreateFromToken(expression[2])),
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
                                                new Expression(new Zilch()),
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
                        result = new Expression(new Error(ErrorType.SyntaxError));
                    }

                    return result;

            }
        }

        public static List<List<Token>> ParseCommaSyntax(List<Token> expression)
        {
            List<List<Token>> result = new List<List<Token>>();
            int start = 0;
            bool inContainer = false;
            TokenType containerType = TokenType.Zilch;
            for(int i = 0; i <= expression.Count; i++)
            {
                if(i < expression.Count && !inContainer && new List<TokenType>() {TokenType.LeftParenthesis, TokenType.LeftBracket, TokenType.LeftCurly}.Contains(expression[i].Type))
                {
                    inContainer = true;
                    containerType = expression[i].Type;
                }

                if(!inContainer && (i == expression.Count || expression[i].Type == TokenType.Comma))
                {
                    result.Add(expression.GetRange(start, i - start));
                    start = i + 1;
                }

                if(i < expression.Count && inContainer && new List<TokenType>() {TokenType.RightParenthesis, TokenType.RightBracket, TokenType.RightCurly}.Contains(expression[i].Type))
                {
                    bool done = false;
                    switch(expression[i].Type)
                    {
                        case TokenType.RightParenthesis:
                            done = containerType == TokenType.LeftParenthesis;
                            break;

                        case TokenType.RightBracket:
                            done = containerType == TokenType.LeftBracket;
                            break;

                        case TokenType.RightCurly:
                            done = containerType == TokenType.LeftCurly;
                            break;
                    }

                    inContainer = !done;
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
                    if(Convert.ToDouble(t.Value) % 1 == 0)
                    {
                        result = new Integer(t.Value);
                    }
                    else
                    {
                        result = new Decimal(t.Value);
                    }

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
                
                case TokenType.List:
                    List<IObject> listValues = new List<IObject>();
                    if(((TokenGroup) t).Tokens.Count > 0)
                    {
                        foreach(List<Token> exp in ParseCommaSyntax(((TokenGroup) t).Tokens))
                        {
                            IObject value = EvaluateExpression(exp).Result();
                            if(value is Error)
                            {
                                return value;
                            }

                            listValues.Add(value);
                        }
                    }

                    result = new IterList(listValues);
                    break;

                case TokenType.CalledFunc:
                    List<Token> tokens = ((TokenGroup) t).Tokens;
                    IObject calledObj = IDs.GetReference(tokens[0].Value);
                    if(calledObj is ICallable)
                    {
                        List<IObject> args = new List<IObject>();

                        if(tokens.Count > 3)
                        {
                            foreach(List<Token> argExpression in ParseCommaSyntax(tokens.GetRange(2, tokens.Count - 3)))
                            {
                                IObject arg = EvaluateExpression(argExpression).Result();
                                if(arg is Error)
                                {
                                    return arg;
                                }

                                args.Add(arg);
                            }
                        }

                        result = ((ICallable) calledObj).Call(args);
                    }
                    else
                    {
                        return new Error(ErrorType.IdNotFoundError);
                    }

                    break;
                
                case TokenType.IndexedObj:
                    TokenGroup tg = (TokenGroup) t;
                    IObject indexedObj = CreateFromToken(tg.Tokens[0]);
                    IObject indexObj = EvaluateExpression(tg.Tokens.GetRange(2, tg.Tokens.Count - 3)).Result();
                    if(indexObj is Error)
                    {
                        return indexObj;
                    }

                    if(!(indexedObj is Iterable))
                    {
                        return new Error(ErrorType.TypeMismatchError);
                    }
                    else
                    {
                        if(indexedObj is IterList && !(indexObj is Integer))
                        {
                            return new Error(ErrorType.TypeMismatchError);
                        }

                        int index = Expression.ToInt(indexObj);

                        switch(indexedObj)
                        {
                            case IterList iter:
                                if(iter.RealIndex(index) >= iter.Length)
                                {
                                    return new Error(ErrorType.IndexOutOfBoundsError);
                                }

                                result = iter.Get(index);

                                break;

                            case IterDict iter:
                                break;
                        }
                    }

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
                if((double) value % 1 == 0)
                {
                    result = new Integer(value);
                }
                else
                {
                    result = new Decimal(value);
                }
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