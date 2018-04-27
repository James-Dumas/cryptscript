using System;
using System.Collections.Generic;

namespace cryptscript
{
    public class BuiltIn : Object, ICallable
    {
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
                            Console.WriteLine(args[0].Repr());
                            break;
                        
                        case 2:
                            if(Expression.ToBool(args[1]))
                            {
                                Console.Write(args[0].Repr());
                            }
                            else
                            {
                                Console.WriteLine(args[0].Repr());
                            }

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
                        result = new String(Util.GetInput(args.Count == 1 ? args[0].Value.ToString() : ""));
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
                    else if(!(args[0] is String && args[1] is Integer && args[2] is Integer))
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
                                result = new String(str.Substring(index1, index2 - index1));
                            }
                        }
                    }

                    break;

                case BuiltInRoutine.Insert:
                    if(!(args.Count == 2 || args.Count == 3))
                    {
                        result = new Error(ErrorType.InvalidArgumentError);
                    }
                    else
                    {
                        switch(args.Count)
                        {
                            case 2:
                                if(!(args[0] is IterList))
                                {
                                    result = new Error(ErrorType.TypeMismatchError);
                                }
                                else
                                {
                                    IterList list = (IterList) args[0];
                                    int index = list.Length;
                                    list.Insert(index, args[1]);
                                }

                                break;

                            case 3:
                                if(!(args[0] is IterList && args[1] is Integer))
                                {
                                    result = new Error(ErrorType.TypeMismatchError);
                                }
                                else
                                {
                                    IterList list = (IterList) args[0];
                                    int index = list.RealIndex(Expression.ToInt(args[1]));
                                    if(index > list.Length || index < 0)
                                    {
                                        result = new Error(ErrorType.IndexOutOfBoundsError);
                                    }
                                    else
                                    {
                                        list.Insert(index, args[2]);
                                    }
                                }

                                break;
                        }
                    }

                    break;

                case BuiltInRoutine.Pop:
                    if(args.Count != 2)
                    {
                        result = new Error(ErrorType.InvalidArgumentError);
                    }
                    else
                    {
                        if(!(args[0] is IterList && args[1] is Integer))
                        {
                            result = new Error(ErrorType.TypeMismatchError);
                        }
                        else
                        {
                            IterList list = (IterList) args[0];
                            int index = list.RealIndex(Expression.ToInt(args[1]));
                            if(index >= list.Length || index < 0)
                            {
                                result = new Error(ErrorType.IndexOutOfBoundsError);
                            }
                            else
                            {
                                result = list.Pop(index);
                            }
                        }
                    }

                    break;

                case BuiltInRoutine.Length:
                    if(args.Count != 1)
                    {
                        result = new Error(ErrorType.InvalidArgumentError);
                    }
                    else if(args[0] is String)
                    {
                        result = new Integer(((string) args[0].Value).Length);
                    }
                    else if(args[0] is Iterable)
                    {
                        result = new Integer(((Iterable) args[0]).Length);
                    }
                    else
                    {
                        result = new Error(ErrorType.TypeMismatchError);
                    }

                    break;                    
                
                case BuiltInRoutine.Type:
                    if(args.Count != 1)
                    {
                        result = new Error(ErrorType.InvalidArgumentError);
                    }
                    else
                    {
                        IObject obj = args[0];
                        string type = "";
                        if(obj is Integer || obj is Decimal)
                        {
                            type = "number";
                        }
                        else if(obj is String)
                        {
                            type = "string";
                        }
                        else if(obj is Boolean)
                        {
                            type = "boolean";
                        }
                        else if(obj is Zilch)
                        {
                            type = "zilch";
                        }
                        else if(obj is IterList)
                        {
                            type = "list";
                        }
                        else if(obj is IterDict)
                        {
                            type = "dict";
                        }
                        else if(obj is ICallable)
                        {
                            type = "routine";
                        }

                        result = new String(type);
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

            if(result is Error)
            {
                Interpreter.ThrowError((Error) result);
                result = null;
            }

            return result;
        }

        public override string Repr()
        {
            return "<built-in routine>";
        }

        public override string Repr(bool showQuotes) => Repr();
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
        Insert,
        Pop,
        Length,
        Type,
        Exit,
    }
}