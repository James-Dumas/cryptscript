using System;
using System.Collections.Generic;
using System.Text;

namespace cryptscript
{
    public class Expression
    {
        #region Operation Data

        /// <summary>
        /// The left value of the expression
        /// </summary>
        private IObject Left { get; set; }

        /// <summary>
        /// The right value of the expression
        /// </summary>
        private IObject Right { get; set; }

        /// <summary>
        /// The type of operation to perform
        /// </summary>
        private OperationType Operation { get; set; }

        private bool IsBaseExpression => Operation == OperationType.None;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new one-value expression
        /// </summary>
        /// <param name="value">The value to operate on</param>
        /// <param name="operation">The type of operation to perform</param>
        public Expression(Expression value)
            : this(value, value, OperationType.None) { }

        public Expression(IObject value)
        {
            Left = value;
            Right = value;
            Operation = OperationType.None;
        }

        /// <summary>
        /// Initializes a new two-value expression
        /// </summary>
        /// <param name="left">The left value</param>
        /// <param name="right">The right value</param>
        /// <param name="operation">The type of operation to perform</param>
        public Expression(Expression left, Expression right, OperationType operation)
        {
            Left = left.Result();
            Right = right.Result();
            Operation = operation;
        }

        public Expression(IObject left, IObject right, OperationType operation)
        {
            Left = left;
            Right = right;
            Operation = operation;
        }

        #endregion

        /// <summary>
        /// Gets the result of this expression by operating on two child expressions
        /// </summary>
        public IObject Result()
        {
            // If an error has occured (value == null), pass it up the stack
            if (Left == null || Right == null || Interpreter.StopExecution) return null;

            // Check if the expression is a base expression
            if (IsBaseExpression)
            {
                // Check if Left is a natural decimal number
                if (Left is Decimal && (double)Left.Value % 1 == 0)
                    return new Integer(Left.Value);   

                // Otherwise just return Left
                return Left;
            }

            object result = null;

            if((Left is String || Right is String)
                    && !(new List<OperationType> {OperationType.Addition, OperationType.Multiplication, OperationType.Equal, OperationType.Inequal, OperationType.NOT,
                                                  OperationType.AND, OperationType.OR, OperationType.XOR}.Contains(Operation)))
            {
                Interpreter.ThrowError(new Error(ErrorType.TypeMismatchError));
                return null;
            }
            else if((Left is Zilch || Right is Zilch || Left is ICallable || Right is ICallable || Left is IterDict || Right is IterDict)
                    && !(new List<OperationType> {OperationType.Equal, OperationType.Inequal, OperationType.NOT,
                                            OperationType.AND, OperationType.OR, OperationType.XOR}.Contains(Operation)))
            {
                Interpreter.ThrowError(new Error(ErrorType.TypeMismatchError));
                return null;
            }
            else if((Left is IterList || Right is IterList)
                    && new List<OperationType> {OperationType.Less, OperationType.LessEqual, OperationType.Greater, OperationType.GreaterEqual}.Contains(Operation))
            {
                Interpreter.ThrowError(new Error(ErrorType.TypeMismatchError));
                return null;
            }

            bool returnDecimal = Left is Decimal || Right is Decimal;

            // Perform the operation
            switch (Operation)
            {
                case OperationType.Addition:
                    // Check for a type mismatch
                    if(!(Left is String) && Right is String || (!(Left is IterList) && Right is IterList
                    || (Left is IterList && !(Right is IterList || Right is Integer || Right is Decimal || Right is Boolean))))
                    {
                        Interpreter.ThrowError(new Error(ErrorType.TypeMismatchError));
                        return null;
                    }

                    if(Left is IterList && Right is IterList)
                    {
                        List<IObject> combinedItems = new List<IObject>();
                        for(int i = 0; i < ((IterList) Left).Length; i++)
                        {
                            // add first list's items
                            combinedItems.Add(((IterList) Left).Get(i));
                        }

                        for(int i = 0; i < ((IterList) Right).Length; i++)
                        {
                            // add second list's items
                            combinedItems.Add(((IterList) Right).Get(i));
                        }

                        result = new IterList(combinedItems);
                    }
                    else if(Left is IterList)
                    {
                        List<IObject> newList = new List<IObject>();
                        foreach(IObject item in ((IterList) Left).Items)
                        {
                            IObject itemResult = new Expression(item, Right, OperationType.Addition).Result();
                            if(itemResult == null)
                            {
                                return null;
                            }

                            newList.Add(itemResult);
                        }

                        result = new IterList(newList);
                    }
                    else
                    {
                        // Return either a concatenated or an added number
                        result = Left is String
                            ? (object)(Left.Value.ToString() + Right.Value.ToString())
                            : returnDecimal
                                ? ToDouble(Left) + ToDouble(Right)
                                : ToInt(Left) + ToInt(Right);
                    }

                    break;

                case OperationType.Subraction:
                    // Check for a type mismatch
                    if(Right is IterList || (Left is IterList && !(Right is Integer || Right is Decimal || Right is Boolean)))
                    {
                        Interpreter.ThrowError(new Error(ErrorType.TypeMismatchError));
                        return null;
                    }

                    if(Left is IterList)
                    {
                        List<IObject> newList = new List<IObject>();
                        foreach(IObject item in ((IterList) Left).Items)
                        {
                            IObject itemResult = new Expression(item, Right, OperationType.Subraction).Result();
                            if(itemResult == null)
                            {
                                return null;
                            }

                            newList.Add(itemResult);
                        }

                        result = new IterList(newList);
                    }
                    else
                    {
                        result = returnDecimal
                            ? ToDouble(Left) - ToDouble(Right)
                            : ToInt(Left) - ToInt(Right);
                    }

                    break;

                case OperationType.Multiplication:
                    // Check for a type mismatch
                    if((Right is String || Right is IterList) || (Left is String && !(Right is Integer || Right is Boolean))
                    || (Left is IterList && !(Right is Integer || Right is Decimal || Right is Boolean)))
                    {
                        Interpreter.ThrowError(new Error(ErrorType.TypeMismatchError));
                        return null;
                    }

                    if(Left is String)
                    {
                        string initStr = Left.Value.ToString();
                        StringBuilder newStr = new StringBuilder();
                        int count = ToInt(Right);
                        if(count < 0)
                        {
                            count *= -1;
                            char[] arr =  initStr.ToCharArray();
                            Array.Reverse(arr);
                            initStr = new string(arr);
                        }

                        for(int i = 0; i < count; i++)
                        {
                            newStr.Append(initStr);
                        }

                        result = newStr.ToString();
                    }
                    else if(Left is IterList)
                    {
                        List<IObject> newList = new List<IObject>();
                        foreach(IObject item in ((IterList) Left).Items)
                        {
                            IObject itemResult = new Expression(item, Right, OperationType.Multiplication).Result();
                            if(itemResult == null)
                            {
                                return null;
                            }

                            newList.Add(itemResult);
                        }

                        result = new IterList(newList);
                    }
                    else
                    {
                        result = returnDecimal
                            ? ToDouble(Left) * ToDouble(Right)
                            : ToInt(Left) * ToInt(Right);
                    }

                    break;

                case OperationType.Division:
                    // Check for a type mismatch
                    if(Right is IterList || (Left is IterList && !(Right is Integer || Right is Decimal || Right is Boolean)))
                    {
                        Interpreter.ThrowError(new Error(ErrorType.TypeMismatchError));
                        return null;
                    }

                    try
                    {
                        if(Left is IterList)
                        {
                            List<IObject> newList = new List<IObject>();
                            foreach(IObject item in ((IterList) Left).Items)
                            {
                                IObject itemResult = new Expression(item, Right, OperationType.Division).Result();
                                if(itemResult == null)
                                {
                                    return null;
                                }

                                newList.Add(itemResult);
                            }

                            result = new IterList(newList);
                        }
                        else
                        {
                            // Only allow integer return type when the numbers divide evenly
                            if(!returnDecimal && ToInt(Left) % ToInt(Right) != 0)
                            {
                                returnDecimal = true;
                            }

                            result = returnDecimal
                                ? ToDouble(Left) / ToDouble(Right)
                                : ToInt(Left) / ToInt(Right);
                        }
                    }
                    catch(System.DivideByZeroException)
                    {
                        Interpreter.ThrowError(new Error(ErrorType.DivisionByZeroError));
                        return null;
                    }

                    break;
                
                case OperationType.Modulo:
                    // Check for a type mismatch
                    if(Right is IterList || (Left is IterList && !(Right is Integer || Right is Decimal || Right is Boolean)))
                    {
                        Interpreter.ThrowError(new Error(ErrorType.TypeMismatchError));
                        return null;
                    }

                    try
                    {
                        if(Left is IterList)
                        {
                            List<IObject> newList = new List<IObject>();
                            foreach(IObject item in ((IterList) Left).Items)
                            {
                                IObject itemResult = new Expression(item, Right, OperationType.Modulo).Result();
                                if(itemResult == null)
                                {
                                    return null;
                                }

                                newList.Add(itemResult);
                            }

                            result = new IterList(newList);
                        }
                        else
                        {
                            result = returnDecimal
                                ? (ToDouble(Left) % ToDouble(Right) + ToDouble(Right)) % ToDouble(Right)
                                : (ToInt(Left) % ToInt(Right) + ToInt(Right)) % ToInt(Right);
                        }
                    }
                    catch(System.DivideByZeroException)
                    {
                        Interpreter.ThrowError(new Error(ErrorType.DivisionByZeroError));
                        return null;
                    }

                    break;

                case OperationType.Exponent:
                    // Check for a type mismatch
                    if(Right is IterList || (Left is IterList && !(Right is Integer || Right is Decimal || Right is Boolean)))
                    {
                        Interpreter.ThrowError(new Error(ErrorType.TypeMismatchError));
                        return null;
                    }

                    if(Left is IterList)
                    {
                        List<IObject> newList = new List<IObject>();
                        foreach(IObject item in ((IterList) Left).Items)
                        {
                            IObject itemResult = new Expression(item, Right, OperationType.Modulo).Result();
                            if(itemResult == null)
                            {
                                return null;
                            }

                            newList.Add(itemResult);
                        }

                        result = new IterList(newList);
                    }
                    else
                    {
                        returnDecimal = returnDecimal || ToDouble(Right) < 0;

                        result = returnDecimal
                            ? Math.Pow(ToDouble(Left), ToDouble(Right))
                            : Math.Pow(ToInt(Left), ToInt(Right));
                    }

                    break;

                case OperationType.AND:
                    result = ToBool(Left) && ToBool(Right);
                    break;
                    
                case OperationType.OR:
                    result = ToBool(Left) || ToBool(Right);
                    break;

                case OperationType.XOR:
                    result = ToBool(Left) && !ToBool(Right) || (!ToBool(Left) && ToBool(Right));
                    break;
                    
                case OperationType.NOT:
                    result = !ToBool(Left);
                    break;

                case OperationType.Equal:
                    result = Equals(Left, Right);
                    break;
                
                case OperationType.Inequal:
                    result = !Equals(Left, Right);
                    break;
                    
                case OperationType.Greater:
                    result = Greater(Left, Right);
                    break;
                    
                case OperationType.Less:
                    result = Less(Left, Right);
                    break;
                    
                case OperationType.GreaterEqual:
                    result = !Less(Left, Right);
                    break;
                    
                case OperationType.LessEqual:
                    result = !Greater(Left, Right);
                    break;
            }

            return result is IObject ? (IObject) result : Parser.CreateObject(result);
        }

        /// <summary>
        /// Converts the given object to a standard <seealso cref="double"/>
        /// </summary>
        /// <param name="x">The object to convert</param>
        public static double ToDouble(IObject x) =>
            Convert.ToDouble(x is Boolean
                ? ToBool(x)
                : x.Value);

        /// <summary>
        /// Converts the given object to a standard <seealso cref="int"/>
        /// </summary>
        /// <param name="x">The object to convert</param>
        public static int ToInt(IObject x) =>
            Convert.ToInt32(x is Boolean
                ? ToBool(x)
                : x.Value);

        /// <summary>
        /// Returns the boolean value of the given object
        /// </summary>
        /// <param name="obj">The object to convert into a boolean</param>
        public static bool ToBool(IObject obj)
        {
            switch(obj)
            {
                case Boolean b: return (bool)b.Value;
                case Integer i: return ToInt(i) != 0;
                case Decimal d: return ToDouble(d) != 0;
                case Iterable i: return i.Length > 0;
                default: return !(obj is Zilch);
            }
        }

        /// <summary>
        /// Returns whether <paramref name="x"/> is equal to <paramref name="y"/>
        /// </summary>
        /// <param name="x">The first <seealso cref="IObject"/> to compare</param>
        /// <param name="y">The second <seealso cref="IObject"/> to compare</param>
        /// <returns></returns>
        private static bool Equals(IObject x, IObject y)
        {
            if(x is Boolean || y is Boolean)
            {
                return ToBool(x) == ToBool(y);
            }
            else if(x is ICallable || y is ICallable)
            {
                return x.Equals(y);
            }
            else if(x.GetType() == y.GetType() || x is String || y is String)
            {
                return x.Value.ToString() == y.Value.ToString();
            }
            else if((x is Decimal && y is Integer) || (x is Integer && y is Decimal))
            {
                return ToDouble(x) == ToDouble(y);
            } 
            else if((x is IterList && y is IterList))
            {
                IterList a = (IterList) x;
                IterList b = (IterList) y;
                if(a.Length != b.Length)
                {
                    return false;
                }

                bool equal = true;
                for(int i = 0; i < a.Length; i++)
                {
                    equal = equal && Equals(a.Get(i), b.Get(i));
                }

                return equal;
            } 
            else if((x is IterDict && y is IterDict))
            {
                IterDict a = (IterDict) x;
                IterDict b = (IterDict) y;
                if(a.Length != b.Length)
                {
                    return false;
                }

                bool equal = true;
                foreach(IObject key in a.GetKeys())
                {
                    equal = equal && b.HasKey(key) && Equals(a.Get(key), b.Get(key));
                }

                return equal;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns whether <paramref name="x"/> is greater than <paramref name="y"/>
        /// </summary>
        /// <param name="x">The first <seealso cref="IObject"/> to compare</param>
        /// <param name="y">The second <seealso cref="IObject"/> to compare</param>
        private static bool Greater(IObject x, IObject y) => ToDouble(x) > ToDouble(y);

        /// <summary>
        /// Returns whether <paramref name="x"/> is less than <paramref name="y"/>
        /// </summary>
        /// <param name="x">The first <seealso cref="IObject"/> to compare</param>
        /// <param name="y">The second <seealso cref="IObject"/> to compare</param>
        private static bool Less(IObject x, IObject y) => ToDouble(x) < ToDouble(y);
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
        LessEqual,
        None
    }
}