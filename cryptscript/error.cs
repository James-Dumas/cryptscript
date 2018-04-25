using System;
using System.Collections.Generic;

namespace cryptscript
{
    public class Error : IObject
    {
        public object Value { get; }
        public ErrorType Type { get; set; }
        public int LineNumber { get; set; }

        public Error(ErrorType type)
        {
            LineNumber = Parser.lineNumber;
            Type = type;

            string msg = "";
            if(!Interpreter.IsInteractive)
            {
                msg += System.String.Format("{0} on line {1}", Type, LineNumber);
            }
            else
            {
                msg += Type.ToString();
            }
            
            Value = msg;
        }

        public string Repr()
        {
            return Type.ToString();
        }
    }

    public enum ErrorType
    {
        SyntaxError,
        TokenNotFoundError,
        TypeMismatchError,
        DivisionByZeroError,
        InvalidArgumentError,
        IndexOutOfBoundsError,
        IdNotFoundError,
        KeyError,
        KeyboardInterrupt,
    }
}