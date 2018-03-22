using System;
using System.Collections.Generic;

namespace cryptscript
{
    public class BuiltIn : ICallable
    {
        public object Value { get; set; } = null;
        private BuiltInRoutine RoutineName { get; set; }

        public BuiltIn(BuiltInRoutine routine)
        {
            RoutineName = routine;
        }

        public static void AddBuiltIns(IdentifierGroup IDs)
        {
            Array routines = Enum.GetValues(typeof(BuiltInRoutine));

            foreach(BuiltInRoutine routine in routines)
            {
                IDs.SetReference(routine.ToString().ToLower(), new BuiltIn(routine));
            }
        }

        public IObject Call(List<IObject> args)
        {
            if(Interpreter.StopExecution)
            {
                return null;
            }

            IObject result = new Zilch();

            switch(RoutineName)
            {
                case BuiltInRoutine.Wallet:
                    if(args.Count > 0)
                    {
                        result = new Error(ErrorType.InvalidArguementException);
                    }
                    else
                    {
                        result = Interpreter.WalletAddr.Reference;
                    }

                    break;

                case BuiltInRoutine.Write:
                    switch(args.Count)
                    {
                        case 0:
                            Console.WriteLine();
                            break;

                        case 1:
                            Console.WriteLine(args[0].ToString());
                            break;
                        
                        default:
                            result = new Error(ErrorType.InvalidArguementException);
                            break;
                    }

                    break;

                case BuiltInRoutine.Read:
                    if(args.Count > 1)
                    {
                        result = new Error(ErrorType.InvalidArguementException);
                    }
                    else
                    {
                        if(args.Count == 1)
                        {
                            Console.Write(args[0].Value.ToString());
                        }

                        result = new String(Console.ReadLine());
                    }

                    break;

                case BuiltInRoutine.Floor:
                    if(args.Count != 1)
                    {
                        result = new Error(ErrorType.InvalidArguementException);
                    }
                    else if(!(args[0] is Integer || args[0] is Decimal))
                    {
                        result = new Error(ErrorType.TypeMismatchException);
                    }
                    else
                    {
                        result = new Integer(Math.Floor(Convert.ToDouble(args[0].Value)));
                    }

                    break;

                case BuiltInRoutine.Round:
                    if(args.Count != 1)
                    {
                        result = new Error(ErrorType.InvalidArguementException);
                    }
                    else if(!(args[0] is Integer || args[0] is Decimal))
                    {
                        result = new Error(ErrorType.TypeMismatchException);
                    }
                    else
                    {
                        result = new Integer(args[0].Value);
                    }

                    break;

                case BuiltInRoutine.Number:
                    double foo;
                    if(args.Count != 1)
                    {
                        result = new Error(ErrorType.InvalidArguementException);
                    }
                    else if(!double.TryParse((string) args[0].Value, out foo))
                    {
                        result = new Error(ErrorType.TypeMismatchException);
                    }
                    else
                    {
                        result = new Decimal(args[0].Value);
                    }

                    break;

                case BuiltInRoutine.String:
                    if(args.Count != 1)
                    {
                        result = new Error(ErrorType.InvalidArguementException);
                    }
                    else
                    {
                        result = new String(args[0].Value);
                    }

                    break;

                case BuiltInRoutine.Length:
                    if(args.Count != 1)
                    {
                        result = new Error(ErrorType.InvalidArguementException);
                    }
                    else if(!(args[0] is String))
                    {
                        result = new Error(ErrorType.TypeMismatchException);
                    }
                    else
                    {
                        result = new Integer(((string) args[0].Value).Length);
                    }

                    break;                    

                case BuiltInRoutine.Exit:
                    if(args.Count > 0)
                    {
                        result = new Error(ErrorType.InvalidArguementException);
                    }
                    else
                    {
                        Interpreter.StopExecution = true;
                    }
                    
                    break;

            }

            return result;
        }

        public override string ToString()
        {
            return "<built-in routine>";
        }
    }

    public enum BuiltInRoutine
    {
        Wallet,
        Write,
        Read,
        Floor,
        Round,
        Number,
        String,
        Length,
        Exit
    }
}