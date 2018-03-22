using System;
using System.Collections.Generic;

namespace cryptscript
{
    public interface IExpression
    {
        IObject Result();
    }

    public class Expression : IExpression
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
        public Expression(IExpression value)
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
        public Expression(IExpression left, IExpression right, OperationType operation)
        {
            Left = left.Result();
            Right = right.Result();
            Operation = operation;
        }

        #endregion

        /// <summary>
        /// Gets the result of this expression by operating on two child expressions
        /// </summary>
        public IObject Result()
        {
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

            // If an error has occured, pass it up the stack
            if (Left is Error) return Left;
            if (Right is Error) return Right;

            if((Left is String || Right is String)
                    && !(new List<OperationType> {OperationType.Addition, OperationType.Equal, OperationType.Inequal, OperationType.NOT,
                                                  OperationType.AND, OperationType.OR, OperationType.XOR}.Contains(Operation)))
            {
                return new Error(ErrorType.TypeMismatchException);
            }
            else if((Left is Zilch || Right is Zilch)
                    && !(new List<OperationType> {OperationType.Equal, OperationType.Inequal, OperationType.NOT,
                                            OperationType.AND, OperationType.OR, OperationType.XOR}.Contains(Operation)))
            {
                return new Error(ErrorType.TypeMismatchException);
            }

            bool returnDecimal = Left is Decimal || Right is Decimal;

            // Perform the operation
            switch (Operation)
            {
                case OperationType.Addition:

                    // Check for a type mismatch
                    if (!(Left is String) && Right is String)
                        return new Error(ErrorType.TypeMismatchException);

                    // Return either a concatenated or an added number
                    result = Left is String
                        ? (object)(Left.Value.ToString() + Right.Value.ToString())
                        : returnDecimal
                            ? ToDouble(Left) + ToDouble(Right)
                            : ToInt(Left) + ToInt(Right);
                    break;

                case OperationType.Subraction:
                    result = returnDecimal
                        ? ToDouble(Left) - ToDouble(Right)
                        : ToInt(Left) - ToInt(Right);
                    break;

                case OperationType.Multiplication:
                    result = returnDecimal
                        ? ToDouble(Left) * ToDouble(Right)
                        : ToInt(Left) * ToInt(Right);
                    break;

                case OperationType.Division:
                    try
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
                    catch(System.DivideByZeroException)
                    {
                        return new Error(ErrorType.DivisionByZeroException);
                    }

                    break;
                
                case OperationType.Modulo:
                    try
                    {
                        result = returnDecimal
                            ? ToDouble(Left) % ToDouble(Right)
                            : ToInt(Left) % ToInt(Right);
                    }
                    catch(System.DivideByZeroException)
                    {
                        return new Error(ErrorType.DivisionByZeroException);
                    }

                    break;

                case OperationType.Exponent:
                    returnDecimal = returnDecimal || ToDouble(Right) < 0;

                    result = returnDecimal
                        ? Math.Pow(ToDouble(Left), ToDouble(Right))
                        : Math.Pow(ToInt(Left), ToInt(Right));
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

            return Parser.CreateObject(result);
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
            if(x.GetType() == y.GetType() || x is String || y is String)
            {
                return x.Value.ToString() == y.Value.ToString();
            }
            if((x is Decimal && y is Integer) || (x is Integer && y is Decimal))
            {
                return ToDouble(x) == ToDouble(y);
            } 

            return false;
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