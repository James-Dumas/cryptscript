using System;
using System.Collections.Generic;

namespace CryptScript
{
    public interface IExpression
    {
        IObject Result();
    }

    public class Expression : IExpression
    {
        private IExpression Value1 { get; set ;}
        private IExpression Value2 { get; set; }
        private OperationType Operation { get; set; }

        public Expression(IExpression value1, IExpression value2, OperationType operation)
        {
            Value1 = value1;
            Value2 = value2;
            Operation = operation;
        }

        public IObject Result()
        {
            object result = null;

            IObject obj1 = Value1.Result();
            IObject obj2 = Value2.Result();

            if(obj1 is Error)
            {
                return obj1;
            }
            if(obj2 is Error)
            {
                return obj2;
            }

            if((obj1 is String || obj2 is String)
                    && !(new List<OperationType> {OperationType.Addition, OperationType.Equal, OperationType.Inequal, OperationType.NOT,
                                                  OperationType.AND, OperationType.OR, OperationType.XOR}.Contains(Operation)))
            {
                return new Error(ErrorType.TypeMismatchError);
            }
            else if((obj1 is Zilch || obj2 is Zilch)
                    && !(new List<OperationType> {OperationType.Equal, OperationType.Inequal, OperationType.NOT,
                                            OperationType.AND, OperationType.OR, OperationType.XOR}.Contains(Operation)))
            {
                return new Error(ErrorType.TypeMismatchError);
            }

            bool returnDecimal = obj1 is Decimal || obj2 is Decimal;

            // Perform the operation
            switch (Operation)
            {
                case OperationType.Addition:
                    if(obj1 is String)
                    {
                        result = obj1.Value.ToString() + obj2.Value.ToString();
                    }
                    else if(obj2 is String)
                    {
                        return new Error(ErrorType.TypeMismatchError);
                    }
                    else if(returnDecimal)
                    {
                        result = ToDouble(obj1) + ToDouble(obj2);
                    }
                    else
                    {
                        result = ToInt(obj1) + ToInt(obj2);
                    }

                    // result = obj1 is String
                    //     ? obj1.Value.ToString() + obj2.Value.ToString()
                    //     : returnDecimal
                    //         ? ToDouble(obj1) + ToDouble(obj2)
                    //         : ToInt(obj1) + ToInt(obj2);

                    // this ternary gives an error for some reason even though it should be the same?

                    break;

                case OperationType.Subraction:
                    result = returnDecimal
                        ? ToDouble(obj1) - ToDouble(obj2)
                        : ToInt(obj1) - ToInt(obj2);
                    break;

                case OperationType.Multiplication:
                    result = returnDecimal
                        ? ToDouble(obj1) * ToDouble(obj2)
                        : ToInt(obj1) * ToInt(obj2);
                    break;

                case OperationType.Division:
                    try
                    {
                        // only allow integer return type when the numbers divide evenly
                        if(!returnDecimal && ToInt(obj1) % ToInt(obj2) != 0)
                        {
                            returnDecimal = true;
                        }

                        result = returnDecimal
                            ? ToDouble(obj1) / ToDouble(obj2)
                            : ToInt(obj1) / ToInt(obj2);
                    }
                    catch(System.DivideByZeroException)
                    {
                        return new Error(ErrorType.DivideByZeroError);
                    }

                    break;
                
                case OperationType.Modulo:
                    try
                    {
                        result = returnDecimal
                            ? ToDouble(obj1) % ToDouble(obj2)
                            : ToInt(obj1) % ToInt(obj2);
                    }
                    catch(System.DivideByZeroException)
                    {
                        return new Error(ErrorType.DivideByZeroError);
                    }

                    break;

                case OperationType.Exponent:
                    returnDecimal = returnDecimal || ToDouble(obj2) < 0;

                    result = returnDecimal
                        ? Math.Pow(ToDouble(obj1), ToDouble(obj2))
                        : Math.Pow(ToInt(obj1), ToInt(obj2));
                    break;

                case OperationType.AND:
                    result = ToBool(obj1) && ToBool(obj2);
                    break;
                    
                case OperationType.OR:
                    result = ToBool(obj1) || ToBool(obj2);
                    break;

                case OperationType.XOR:
                    result = ToBool(obj1) && !ToBool(obj2) || (!ToBool(obj1) && ToBool(obj2));
                    break;
                    
                case OperationType.NOT:
                    result = !ToBool(obj1);
                    break;

                case OperationType.Equal:
                    result = Equal(obj1, obj2);
                    break;
                
                case OperationType.Inequal:
                    result = !Equal(obj1, obj2);
                    break;
                    
                case OperationType.Greater:
                    result = Greater(obj1, obj2);
                    break;
                    
                case OperationType.Less:
                    result = Less(obj1, obj2);
                    break;
                    
                case OperationType.GreaterEqual:
                    result = !Less(obj1, obj2);
                    break;
                    
                case OperationType.LessEqual:
                    result = !Greater(obj1, obj2);
                    break;
            }

            return Parser.CreateObject(result);
        }

        public static double ToDouble(IObject x) =>
            x is Boolean
                ? ToBool(x)
                    ? 1.0
                    : 0.0
                : Convert.ToDouble(x.Value);

        public static int ToInt(IObject x) =>
            x is Boolean
                ? ToBool(x)
                    ? 1
                    : 0
                : Convert.ToInt32(x.Value);

        public static bool ToBool(IObject x) =>
            x is Boolean
                ? (bool) x.Value
                : x is Integer
                    ? ToInt(x) != 0
                    : x is Decimal
                        ? ToDouble(x) != 0.0
                        : !(x is Zilch);

        private static bool Equal(IObject x, IObject y)
        {
            if(x is Boolean || y is Boolean)
            {
                return ToBool(x) == ToBool(y);
            }
            else if((x is Zilch && !(y is Zilch)) || (!(x is Zilch) && y is Zilch))
            {
                return false;
            }
            else if(x.GetType() == y.GetType() || x is String || y is String)
            {
                return x.Value.ToString() == y.Value.ToString();
            }
            else if((x is Decimal && y is Integer) || (x is Integer && y is Decimal))
            {
                return ToDouble(x) == ToDouble(y);
            }

            return false;
        }

        private static bool Greater(IObject x, IObject y)
        {
            if(x is Decimal || y is Decimal)
            {
                return ToDouble(x) > ToDouble(y);
            }
            else
            {
                return ToInt(x) > ToInt(y);
            }
        }

        private static bool Less(IObject x, IObject y)
        {
            if(x is Decimal || y is Decimal)
            {
                return ToDouble(x) < ToDouble(y);
            }
            else
            {
                return ToInt(x) < ToInt(y);
            }
        }
    }
    
    public class BaseExpression : IExpression
    {
        public IObject Value { get; set; }

        public BaseExpression(IObject value)
        {
            Value = value;
        }

        public IObject Result()
        {
            if(Value is Decimal && (double) Value.Value % 1 == 0)
            {
                return new Integer(Value.Value);
            }
            else
            {
                return this.Value;
            }
        }
    }

    /// <summary>
    /// Possible types of operations
    /// </summary>
    public enum OperationType
    {
        Addition,
        Subraction,
        Multiplication,
        Division,
        Modulo,
        Exponent,
        AND,
        OR,
        XOR,
        NOT,
        Equal,
        Inequal,
        Greater,
        Less,
        GreaterEqual,
        LessEqual
    }
}