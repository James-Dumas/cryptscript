using System;
using System.Collections.Generic;

namespace CryptScript
{
    public class Error : IObject
    {
        public object Value { get; }
        public ErrorType Type { get; set; }
        public int LineNumber { get; set; }

        public Error(ErrorType type)
        {
            LineNumber = Interpreter.LineNumber;
            Type = type;

            string msg = "";
            if(LineNumber > 0)
            {
                msg += System.String.Format("{0} on line {1}:", Type, LineNumber);
            }
            else
            {
                msg += Type.ToString();
            }
            
            Value = msg;
        }
    }

    public enum ErrorType
    {
        SyntaxError,
        UnknownTokenError,
        TypeMismatchError,
        DivideByZeroError,
        NoWalletAddressError
    }
}