using System;
using System.Collections.Generic;

namespace CryptScript
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
                case BuiltInRoutine.wallet:
                    if(args.Count > 0)
                    {
                        result = new Error(ErrorType.ArgumentError);
                    }
                    else
                    {
                        result = Interpreter.WalletAddr.Reference;
                    }

                    break;

                case BuiltInRoutine.write:
                    switch(args.Count)
                    {
                        case 0:
                            Console.WriteLine();
                            break;

                        case 1:
                            Console.WriteLine(args[0].ToString());
                            break;
                        
                        default:
                            result = new Error(ErrorType.ArgumentError);
                            break;
                    }

                    break;

                case BuiltInRoutine.read:
                    if(args.Count > 1)
                    {
                        result = new Error(ErrorType.ArgumentError);
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

                case BuiltInRoutine.floor:
                    if(args.Count != 1)
                    {
                        result = new Error(ErrorType.ArgumentError);
                    }
                    else if(!(args[0] is Integer || args[0] is Decimal))
                    {
                        result = new Error(ErrorType.TypeMismatchError);
                    }
                    else
                    {
                        result = new Integer(Math.Floor(Convert.ToDouble(args[0].Value)));
                    }

                    break;

                case BuiltInRoutine.round:
                    if(args.Count != 1)
                    {
                        result = new Error(ErrorType.ArgumentError);
                    }
                    else if(!(args[0] is Integer || args[0] is Decimal))
                    {
                        result = new Error(ErrorType.TypeMismatchError);
                    }
                    else
                    {
                        result = new Integer(args[0].Value);
                    }

                    break;

                case BuiltInRoutine.number:
                    double foo;
                    if(args.Count != 1)
                    {
                        result = new Error(ErrorType.ArgumentError);
                    }
                    else if(!double.TryParse((string) args[0].Value, out foo))
                    {
                        result = new Error(ErrorType.TypeMismatchError);
                    }
                    else
                    {
                        result = new Decimal(args[0].Value);
                    }

                    break;

                case BuiltInRoutine.String:
                    if(args.Count != 1)
                    {
                        result = new Error(ErrorType.ArgumentError);
                    }
                    else
                    {
                        result = new String(args[0].Value);
                    }

                    break;

                case BuiltInRoutine.length:
                    if(args.Count != 1)
                    {
                        result = new Error(ErrorType.ArgumentError);
                    }
                    else if(!(args[0] is String))
                    {
                        result = new Error(ErrorType.TypeMismatchError);
                    }
                    else
                    {
                        result = new Integer(((string) args[0].Value).Length);
                    }

                    break;                    

                case BuiltInRoutine.exit:
                    if(args.Count > 0)
                    {
                        result = new Error(ErrorType.ArgumentError);
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
        wallet,
        write,
        read,
        floor,
        round,
        number,
        String, // capitalized because lowercase 'string' is a type; ToLower() method is used later for ID name
        length,
        exit
    }
}