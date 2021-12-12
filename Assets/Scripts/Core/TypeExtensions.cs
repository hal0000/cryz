using System;
using System.Collections.Generic;

public static class TypeExtensions 
{
    public static int ToInt(this object o)
    {
        if (o != null)
        {
            if (o is int)
                return (int)o;
            else if (o is uint)
                return (int)(uint)o;
            else if (o is float)
                return (int)(float)o;
            else if (o is long)
                return (int)(long)o;
            else if (o is bool)
                return (bool)o ? 1 : 0;
            else
            {
                double.TryParse(o.ToString(), out double dVal);
                return (int)dVal;
            }
        }
        return 0;
    }

    public static int ToInt(this object obj, string key)
    {
        if (obj is IDictionary<string, object> dict)
        {
            if (dict.TryGetValue(key, out object o))
                return o.ToInt();
        }
        return 0;
    }

    public static float ToFloat(this object o)
    {
        if (o != null)
        {
            if (o is int)
                return (int)o;
            else if (o is uint)
                return (float)(uint)o;
            else if (o is float)
                return (float)o;
            else if (o is long)
                return (float)(long)o;
            else
            {
                float.TryParse(o.ToString(), out float dVal);
                return (float)dVal;
            }
        }
        return 0;
    }

    public static uint ToColor(this object o)
    {
        if (o != null)
        {
            if (o is uint) return (uint)o;
            if (o is string str)
            {
                uint.TryParse(str, out uint result);
                return result;
            }
            return (uint)o.ToInt();
        }
        return 0;
    }

    public static float ToFloat(this object obj, string key)
    {
        if (obj is IDictionary<string, object> dict)
        {
            if (dict.TryGetValue(key, out object o))
                return o.ToFloat();
        }
        return 0;
    }

    public static long ToLong(this object val)
    {
        if (val != null)
        {
            if (val is int)
                return (int)val;
            else if (val is uint)
                return (long)(uint)val;
            else if (val is float)
                return (long)(float)val;
            else if (val is long)
                return (long)val;
            else
            {
                long.TryParse(val.ToString(), out long dVal);
                return dVal;
            }
        }
        return 0;
    }


    public static long ToLong(this object obj, string key)
    {
        if (obj is IDictionary<string, object> dict)
        {
            if (dict.TryGetValue(key, out object o))
                return o.ToLong();
        }
        return 0;
    }

    public static double ToDouble(this object val)
    {
        if (val != null)
        {
            if (val is int)
                return (int)val;
            else if (val is uint)
                return (uint)val;
            else if (val is float)
                return (long)(float)val;
            else if (val is long)
                return (long)val;
            else if (val is bool)
                return (bool)val ? 1 : 0;
            else
            {
                double.TryParse(val.ToString(), out double dVal);
                return dVal;
            }
        }
        return 0;
    }


    public static double ToDouble(this object obj, string key)
    {
        if (obj is IDictionary<string, object> dict)
        {
            if (dict.TryGetValue(key, out object o))
                return o.ToLong();
        }
        return 0;
    }

    public static bool ToBool(this object o)
    {
        if (o != null)
        {
            if (o is bool)
                return (bool)o;
            if (o is string)
            {
                string str = (string)o;
                return str == "True" || str == "true" || str == "yes";
            }
            else return o.ToInt() != 0;
        }
        return false;
    }

    public static bool ToBool(this object obj, string key)
    {
        if (obj is IDictionary<string, object> dict)
        {
            if (dict.TryGetValue(key, out object o))
                return o.ToBool();
        }
        return false;
    }

    public static string ToString(this object obj, string key)
    {
        if (obj is IDictionary<string, object> dict)
        {
            if (dict.TryGetValue(key, out object o))
                return o?.ToString();
        }
        return null;
    }

    public static List<T> ToList<T>(this IDictionary<string, object> dict, string key)
    {
        dict.TryGetValue(key, out object o);
        return o != null ? o as List<T> : null;
    }

    public static List<int> ToIntList(this IDictionary<string, object> dict, string key)
    {
        dict.TryGetValue(key, out object o);
        List<object> objList = (List<object>)o;
        if (objList == null) objList = new List<object>();

        List<int> intList = new List<int>();
        objList.ForEach((obj) => intList.Add(obj.ToInt()));
        return intList;
    }

    public static object ToObject(this object obj, string key)
    {
        if (obj is IDictionary<string, object> dict)
        {
            if (dict.TryGetValue(key, out object o))
                return o;
        }
        return null;
    }

    // Extension method, call for any object, eg "if (x.IsNumeric())..."
    public static bool IsNumeric(this object x) { return x != null && IsNumeric(x.GetType()); }

    // Method where you know the type of the object
    public static bool IsNumeric(Type type) { return IsNumeric(type, Type.GetTypeCode(type)); }

    // Method where you know the type and the type code of the object
    public static bool IsNumeric(Type type, TypeCode typeCode) { return typeCode == TypeCode.Decimal || (type.IsPrimitive && typeCode != TypeCode.Object && typeCode != TypeCode.Boolean && typeCode != TypeCode.Char); }
}