using System;
using System.Collections.Generic;

namespace cryptscript
{
    public class Parser
    {
        public static int lineNumber { get; set; } = 1;
        public bool parsingLoop { get; set; } = false;
        public bool parsingFunc { get; set; } = false;
        public bool doneParsing { get; set; } = true;
        private IdentifierGroup IDs { get; set; }
        private List<Line> storedCode { get; set; }
        private List<string> storedArgs { get; set; }
        private string routineName { get; set; }
        private TokenType loopType { get; set; }
        private List<Token> loopCondition { get; set; }
        private int loopStartLine { get; set; }
        private int endsNeeded { get; set; } = 0;

        private static bool breakOut { get; set; } = false;

        public Parser(IdentifierGroup ids)
        {
            IDs = ids;
            BuiltIn.AddBuiltIns(IDs);
        }

        public List<Token> PreParse(List<Token> ungroupedTokens)
        {
            // Pre-Parser will convert groups of related tokens into TokenGroup objects
            // also maybe parsing out negative numbers, not sure yet

            List<Token> tokens = new List<Token>();

            for(int i = 0; i < ungroupedTokens.Count; i++)
            {
                // check for unknown tokens
                if(ungroupedTokens[i].Type == TokenType.Unknown)
                {
                    Interpreter.ThrowError(new Error(ErrorType.TokenNotFoundError));
                    return null;
                }
            }

            for(int i = 0; i < ungroupedTokens.Count; i++)
            {
                // add all tokens except comments to new list
                if(ungroupedTokens[i].Type != TokenType.Comment)
                {
                    tokens.Add(ungroupedTokens[i]);
                }
            }

            for(int i = 0; i < tokens.Count; i++)
            {
                if(i < tokens.Count - 1)
                {
                    if(Token.ObjectTokens.Contains(tokens[i].Type) && tokens[i + 1].Type == TokenType.LeftBracket)
                    {
                        // indexed object

                        int endIndex = FindClosingBrace(tokens, i + 1);

                        if(endIndex - 2 == i || endIndex == -1)
                        {
                            Interpreter.ThrowError(new Error(ErrorType.SyntaxError));
                            return null;
                        }

                        List<Token> containedTokens = new List<Token>() {tokens[i], tokens[i + 1], tokens[endIndex]};
                        containedTokens.InsertRange(2, PreParse(tokens.GetRange(i + 2, endIndex - i - 2)));

                        tokens.RemoveRange(i, endIndex - i + 1);
                        tokens.Insert(i, new TokenGroup(TokenType.IndexedObj, containedTokens));
                        i = -1;
                        continue;
                    }
                    else if(Token.ObjectTokens.Contains(tokens[i].Type) && tokens[i + 1].Type == TokenType.LeftParenthesis && (i == 0 || tokens[i - 1].Type != TokenType.Func))
                    {
                        // called object

                        int endIndex = FindClosingBrace(tokens, i + 1); 

                        if(endIndex == -1)
                        {
                            Interpreter.ThrowError(new Error(ErrorType.SyntaxError));
                            return null;
                        }

                        List<Token> containedTokens = new List<Token>() {tokens[i], tokens[i + 1], tokens[endIndex]};
                        containedTokens.InsertRange(2, PreParse(tokens.GetRange(i + 2, endIndex - i - 2)));

                        tokens.RemoveRange(i, endIndex - i + 1);
                        tokens.Insert(i, new TokenGroup(TokenType.CalledObj, containedTokens));
                        i = -1;
                        continue;
                    }
                }

                if(tokens[i].Type == TokenType.LeftBracket || tokens[i].Type == TokenType.LeftCurly)
                {
                    // lists and dictionaries

                    TokenType type = tokens[i].Type == TokenType.LeftBracket ? TokenType.List : TokenType.Dict;
                    int endIndex = FindClosingBrace(tokens, i); 

                    if(endIndex == -1)
                    {
                        Interpreter.ThrowError(new Error(ErrorType.SyntaxError));
                        return null;
                    }

                    List<Token> containedTokens = new List<Token>() {tokens[i], tokens[endIndex]};
                    containedTokens.InsertRange(1, PreParse(tokens.GetRange(i + 1, endIndex - i - 1)));

                    tokens.RemoveRange(i, endIndex - i + 1);
                    tokens.Insert(i, new TokenGroup(type, containedTokens));
                    i = -1;
                    continue;
                }

            }

            return tokens;
        }

        public IObject Parse(Line line, bool inSubParser)
        {

            List<Token> tokens = PreParse(line.Tokens);
            lineNumber = line.LineNum;

            if(Interpreter.ErrorMsg != null)
            {
                return null;
            }

            IObject result = null;

            if((parsingFunc || parsingLoop) && !doneParsing && tokens.Count > 0)
            {
                // get every line of code inputted until an end is given
                if(tokens[0].Type == TokenType.End)
                {
                    if(tokens.Count > 1)
                    {
                        // end token must be alone on a line
                        Interpreter.ThrowError(new Error(ErrorType.SyntaxError));
                        return null;
                    }

                    if(endsNeeded > 0)
                    {
                        endsNeeded--;
                    }
                    else
                    {
                        doneParsing = true;
                    }
                }

                if(Token.LoopTokens.Contains(tokens[0].Type))
                {
                    endsNeeded++;
                }

                if(!doneParsing)
                {
                    storedCode.Add(line);
                }
            }
            else
            {
                // mine until you get a block
                // NOTE: insert reference to miner here

                if(tokens.Count > 0)
                {
                    if(tokens.Count > 1 && (tokens[0].Type == TokenType.ID || tokens[0].Type == TokenType.IndexedObj) && tokens[1].Type == TokenType.Set)
                    {
                        // check for 'variable = expression' syntax

                        IObject expressionResult = EvaluateExpression(tokens.GetRange(2, tokens.Count - 2)).Result(); 
                        string IdName;
                        if(expressionResult != null)
                        {
                            if(tokens[0].Type == TokenType.ID)
                            {
                                IdName = tokens[0].Value;
                                IDs.SetReference(IdName, expressionResult);
                            }
                            else
                            {
                                TokenGroup tg = (TokenGroup) tokens[0];
                                IObject indexedObj = CreateFromToken(tg.Tokens[0]);
                                IObject indexObj = EvaluateExpression(tg.Tokens.GetRange(2, tg.Tokens.Count - 3)).Result();
                                if(indexObj == null)
                                {
                                    return null;
                                }

                                if(!(indexedObj is Iterable) || (indexedObj is IterList && !(indexObj is Integer)))
                                {
                                    Interpreter.ThrowError(new Error(ErrorType.TypeMismatchError));
                                    return null;
                                }

                                int index = Expression.ToInt(indexObj);

                                switch(indexedObj)
                                {
                                    case IterList iter:
                                        if(iter.RealIndex(index) >= iter.Length)
                                        {
                                            Interpreter.ThrowError(new Error(ErrorType.IndexOutOfBoundsError));
                                            return null;
                                        }
                                        else
                                        {
                                            iter.Set(index, expressionResult);
                                        }

                                        break;

                                    case IterDict iter:
                                        // implement dictionary indexing
                                        break;
                                }
                            }
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
                        storedCode = new List<Line>();
                        parsingFunc = true;
                        doneParsing = false;
                    }
                    else if(new List<TokenType>() {TokenType.If, TokenType.While, TokenType.For}.Contains(tokens[0].Type) && tokens[tokens.Count - 1].Type == TokenType.Do)
                    {
                        // check for loops
                        loopType = tokens[0].Type;
                        loopCondition = tokens.GetRange(1, tokens.Count - 2);
                        storedCode = new List<Line>();
                        parsingLoop = true;
                        doneParsing = false;
                        loopStartLine = lineNumber;
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

            if(tokens.Count > 0 && tokens[0].Type == TokenType.Wallet)
            {
                // check for wallet address
                Interpreter.WalletAddr.Reference = new String(tokens[0].Value);
                result = null;
            }

            if(doneParsing && parsingFunc)
            {
                // when a function is finished parsing
                IDs.SetReference(routineName, new Routine(routineName, storedCode, storedArgs));
                parsingFunc = false;
            }

            if(doneParsing && parsingLoop)
            {
                // when a loop is finished parsing
                Parser loopParser = new Parser(IDs);
                IObject loopBoolean;
                switch(loopType)
                {
                    case TokenType.If:
                        List<Line> ifCode = storedCode;
                        List<Line> elseCode = new List<Line>();
                        int numLoops = 0;
                        for(int i = 0; i < storedCode.Count; i++)
                        {
                            List<Token> lineHere = storedCode[i].Tokens;
                            // check for else block
                            if(Token.LoopTokens.Contains(lineHere[0].Type))
                            {
                                numLoops++;
                            }
                            if(lineHere[0].Type == TokenType.End && numLoops >= 0)
                            {
                                numLoops--;
                            }
                            if(lineHere.Count == 1 && lineHere[0].Type == TokenType.Else && numLoops == 0)
                            {
                                ifCode = storedCode.GetRange(0, i);
                                elseCode = storedCode.GetRange(i + 1, storedCode.Count - i - 1);
                            }
                        }

                        loopBoolean = EvaluateExpression(loopCondition).Result();
                        if(loopBoolean == null)
                        {
                            return null;
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
                            storedCode = new List<Line>();
                        }

                        foreach(Line lineHere in storedCode)
                        {
                            result = loopParser.Parse(lineHere, true);
                            if(inSubParser && result != null)
                            {
                                return result;
                            }

                            if(breakOut)
                            {
                                break;
                            }
                        }

                        break;

                    case TokenType.While:
                        lineNumber = loopStartLine;
                        loopBoolean = EvaluateExpression(loopCondition).Result();
                        if(loopBoolean == null)
                        {
                            return null;
                        }

                        while(Expression.ToBool(EvaluateExpression(loopCondition).Result()))
                        {
                            foreach(Line lineHere in storedCode)
                            {
                                result = loopParser.Parse(lineHere, true);
                                if(inSubParser && result != null)
                                {
                                    return result;
                                }

                                if(breakOut || Interpreter.ErrorMsg != null)
                                {
                                    breakOut = true;
                                    break;
                                }
                            }

                            if(breakOut)
                            {
                                breakOut = false;
                                break;
                            }

                            lineNumber = loopStartLine;
                            loopBoolean = EvaluateExpression(loopCondition).Result();
                            if(loopBoolean == null)
                            {
                                return null;
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
                        IObject temp;
                        switch(loopArgs.Count)
                        {
                            case 2:
                                temp = EvaluateExpression(loopArgs[1]).Result();
                                if(temp == null)
                                {
                                    return null;
                                }

                                end = Expression.ToDouble(temp);
                                break;
                            
                            case 3:
                                temp = EvaluateExpression(loopArgs[1]).Result();
                                if(temp == null)
                                {
                                    return null;
                                }

                                start = Expression.ToDouble(temp);

                                temp = EvaluateExpression(loopArgs[2]).Result();
                                if(temp == null)
                                {
                                    return null;
                                }
                                end = Expression.ToDouble(temp);
                                break;
                            
                            case 4:
                                temp = EvaluateExpression(loopArgs[1]).Result();
                                if(temp == null)
                                {
                                    return null;
                                }

                                start = Expression.ToDouble(temp);
                                
                                temp = EvaluateExpression(loopArgs[2]).Result();
                                if(temp == null)
                                {
                                    return null;
                                }

                                end = Expression.ToDouble(temp);

                                temp = EvaluateExpression(loopArgs[3]).Result();
                                if(temp == null)
                                {
                                    return null;
                                }
                                step = Expression.ToDouble(temp);
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
                            foreach(Line lineHere in storedCode)
                            {
                                result = loopParser.Parse(lineHere, true);
                                if(inSubParser && result != null)
                                {
                                    return result;
                                }

                                if(breakOut || Interpreter.ErrorMsg != null)
                                {
                                    breakOut = true;
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

        public Expression EvaluateExpression(List<Token> expression)
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

            while(expression.Count > 0)
            {
                // remove redundant parentheses around expression
                bool hasSurroundingParens = expression[0].Type == TokenType.LeftParenthesis && FindClosingBrace(expression, 0) == expression.Count - 1;

                if(hasSurroundingParens)
                {
                    expression = expression.GetRange(1, expression.Count - 2);
                }
                else
                {
                    break;
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
                    Interpreter.ThrowError(new Error(ErrorType.SyntaxError));
                    return new Expression((IObject) null);

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
                            Interpreter.ThrowError(new Error(ErrorType.SyntaxError));
                            return new Expression((IObject) null);

                    }
                
                case 3:
                    if(!Token.OperationOf.ContainsKey(expression[1].Type))
                    {
                        Interpreter.ThrowError(new Error(ErrorType.SyntaxError));
                        return new Expression((IObject) null);
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

                    Expression result;
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
                        Interpreter.ThrowError(new Error(ErrorType.SyntaxError));
                        result = new Expression((IObject) null);
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
            IObject result = null;

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
                            if(value == null)
                            {
                                return value;
                            }

                            listValues.Add(value);
                        }
                    }

                    result = new IterList(listValues);
                    break;

                case TokenType.CalledObj:
                    List<Token> tokens = ((TokenGroup) t).Tokens;
                    IObject calledObj = CreateFromToken(tokens[0]);
                    if(calledObj is ICallable)
                    {
                        List<IObject> args = new List<IObject>();

                        if(tokens.Count > 3)
                        {
                            foreach(List<Token> argExpression in ParseCommaSyntax(tokens.GetRange(2, tokens.Count - 3)))
                            {
                                IObject arg = EvaluateExpression(argExpression).Result();
                                if(arg == null)
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
                        Interpreter.ThrowError(new Error(ErrorType.IdNotFoundError));
                        return null;
                    }

                    break;
                
                case TokenType.IndexedObj:
                    TokenGroup tg = (TokenGroup) t;
                    IObject indexedObj = CreateFromToken(tg.Tokens[0]);
                    IObject indexObj = EvaluateExpression(tg.Tokens.GetRange(2, tg.Tokens.Count - 3)).Result();
                    if(indexObj == null)
                    {
                        return null;
                    }

                    if(!(indexedObj is Iterable || indexedObj is String))
                    {
                        Interpreter.ThrowError(new Error(ErrorType.TypeMismatchError));
                        return null;
                    }
                    else
                    {
                        if((indexedObj is IterList || indexedObj is String) && !(indexObj is Integer))
                        {
                            Interpreter.ThrowError(new Error(ErrorType.TypeMismatchError));
                            return null;
                        }

                        int index = Expression.ToInt(indexObj);

                        switch(indexedObj)
                        {
                            case String iter:
                                string objString = iter.Value.ToString();
                                if(index < 0)
                                {
                                    index = index + objString.Length;
                                }
                                if(index >= objString.Length || index < 0)
                                {
                                    Interpreter.ThrowError(new Error(ErrorType.IndexOutOfBoundsError));
                                    return null;
                                }

                                result = new String(objString.Substring(index, 1));

                                break;

                            case IterList iter:
                                if(iter.RealIndex(index) >= iter.Length || iter.RealIndex(index) < 0)
                                {
                                    Interpreter.ThrowError(new Error(ErrorType.IndexOutOfBoundsError));
                                    return null;
                                }

                                result = iter.Get(index);

                                break;

                            case IterDict iter:
                                // implement dictionary indexing
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

        public static int FindClosingBrace(List<Token> tokens, int openBrace)
        {
            // returns the index of the closing brace to the given one
            // if none is found, returns -1

            IDictionary<TokenType,TokenType> altBraces = new Dictionary<TokenType, TokenType>()
            {
                { TokenType.LeftParenthesis,    TokenType.RightParenthesis },
                { TokenType.LeftBracket,        TokenType.RightBracket },
                { TokenType.LeftCurly,          TokenType.RightCurly },
            };

            TokenType givenBrace = tokens[openBrace].Type;
            TokenType targetBrace = altBraces[givenBrace];

            int numBraces = 0;
            for(int i = openBrace + 1; i < tokens.Count; i++)
            {
                if(tokens[i].Type == givenBrace)
                {
                    numBraces++;
                }
                else if(tokens[i].Type == targetBrace)
                {
                    if(numBraces > 0)
                    {
                        numBraces--;
                    }
                    else
                    {
                        return i;
                    }
                }
            }

            return -1;
        }
    }
}