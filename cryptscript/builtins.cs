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
                        result = new Error(ErrorType.InvalidArgumentError);
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
                            result = new Error(ErrorType.InvalidArgumentError);
                            break;
                    }

                    break;

                case BuiltInRoutine.Read:
                    if(args.Count > 1)
                    {
                        result = new Error(ErrorType.InvalidArgumentError);
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
                        result = new Error(ErrorType.InvalidArgumentError);
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

                case BuiltInRoutine.Round:
                    if(args.Count != 1)
                    {
                        result = new Error(ErrorType.InvalidArgumentError);
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

                case BuiltInRoutine.Number:
                    double foo;
                    if(args.Count != 1)
                    {
                        result = new Error(ErrorType.InvalidArgumentError);
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
                        result = new Error(ErrorType.InvalidArgumentError);
                    }
                    else
                    {
                        result = new String(args[0].Value);
                    }

                    break;

                case BuiltInRoutine.Strip:
                    if(args.Count != 1)
                    {
                        result = new Error(ErrorType.InvalidArgumentError);
                    }
                    else if(!(args[0] is String))
                    {
                        result = new Error(ErrorType.TypeMismatchError);
                    }
                    else
                    {
                        result = new String(((string) args[0].Value).Trim());
                    }

                    break;

                case BuiltInRoutine.Slice:
                    if(args.Count != 3)
                    {
                        result = new Error(ErrorType.InvalidArgumentError);
                    }
                    else if(!(args[0] is String) || !(args[1] is Integer) || !(args[2] is Integer))
                    {
                        result = new Error(ErrorType.TypeMismatchError);
                    }
                    else 
                    {
                        string str = (string) args[0].Value;
                        int index1 = (int) args[1].Value;
                        int index2 = (int) args[2].Value;
                        if(index1 >= str.Length || index1 < str.Length * -1 || index2 >= str.Length + 1 || index2 < str.Length * -1 - 1)
                        {
                            result = new Error(ErrorType.IndexOutOfBoundsError);
                        }
                        else
                        {
                            // convert negative indices to positive
                            index1 = index1 < 0 ? str.Length + index1 : index1;
                            index2 = index2 < 0 ? str.Length + index2 : index2;
                            if(index2 <= index1)
                            {
                                result = new Error(ErrorType.IndexOutOfBoundsError);
                            }
                            else
                            {
                                result = new String(str.Substring(index1, index2));
                            }
                        }
                    }

                    break;

                case BuiltInRoutine.Length:
                    if(args.Count != 1)
                    {
                        result = new Error(ErrorType.InvalidArgumentError);
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

                case BuiltInRoutine.Exit:
                    if(args.Count > 0)
                    {
                        result = new Error(ErrorType.InvalidArgumentError);
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
        Strip,
        Slice,
        Length,
        Exit
    }
}