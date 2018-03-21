using System;
using System.Collections.Generic;

namespace CryptScript
{
    public interface IObject
    {
        object Value { get; }
    }

    public class Integer : IObject
    {
        public object Value { get; }

        public Integer(object value)
        {
            Value = Convert.ToInt32(value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Decimal : IObject
    {
        public object Value { get; }

        public Decimal(object value)
        {
            Value = Convert.ToDouble(value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class String : IObject
    {
        public object Value { get; }

        public String(object value)
        {
            Value = Convert.ToString(value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Boolean : IObject
    {
        public object Value { get; }

        public Boolean(object value)
        {
            Value = Convert.ToBoolean(value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Zilch : IObject
    {
        public object Value { get; } = "Zilch";

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}