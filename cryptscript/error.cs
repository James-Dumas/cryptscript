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
            if(LineNumber > 0)
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

        public string Repr(bool showQuotes) => Repr();
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
        FileError,
        KeyboardInterrupt,
    }
}