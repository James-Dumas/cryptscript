using System;
using System.Collections.Generic;

namespace cryptscript
{
    public interface IObject
    {
        object Value { get; }
        string Repr(bool showQuotes);
        string Repr();
    }

    /// <summary>
    /// Abstract class representing the base object
    /// </summary>
    public abstract class Object : IObject
    {
        /// <summary>
        /// The object's internal value
        /// </summary>
        public object Value { get; protected set; }

        /// <summary>
        /// Returns a string that represents the object's value
        /// </summary>
        public virtual string Repr()
        {
            return Value.ToString();
        }

        public virtual string Repr(bool showQuotes) => Repr();
    }

    /// <summary>
    /// Represents an integer as an object
    /// </summary>
    public class Integer : Object, IObject
    {
        public Integer(object value) => Value = Convert.ToInt32(value);
    }

    /// <summary>
    /// Represents a decimal as an object
    /// </summary>
    public class Decimal : Object, IObject
    {
        public Decimal(object value) => Value = Convert.ToDouble(value);
    }

    /// <summary>
    /// Represents a string as an object
    /// </summary>
    public class String : Object, IObject
    {
        public String(object value) => Value = Convert.ToString(value);

        public override string Repr(bool showQuotes)
        {
            if(showQuotes)
            {
                return "\"" + Value.ToString() + "\"";
            }
            else
            {
                return Value.ToString();
            }
        }

        public override string Repr() => Repr(false);
    }

    /// <summary>
    /// Represent a boolean as an object
    /// </summary>
    public class Boolean : Object, IObject
    {
        public Boolean(object value) => Value = Convert.ToBoolean(value);
    }

    /// <summary>
    /// Null-type value, immutable
    /// </summary>
    public class Zilch : Object, IObject
    {
        public Zilch() => Value = "Zilch";
    }
}