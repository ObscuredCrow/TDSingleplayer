using System;
using UnityEngine;

public static class ParseExtensions
{
    #region Booleans

    public static byte ToByte(this bool value) => Convert.ToByte(value);

    public static char ToChar(this bool value) => Convert.ToChar(value);

    public static decimal ToDecimal(this bool value) => value.ToInt();

    public static double ToDouble(this bool value) => value.ToInt();

    public static float ToFloat(this bool value) => value.ToInt();

    public static int ToInt(this bool value) => value ? 1 : 0;

    public static long ToLong(this bool value) => value.ToInt();

    #endregion Booleans

    #region Bytes

    public static bool ToBoolean(this byte[] value) => BitConverter.ToBoolean(value, 0);

    public static char ToChar(this byte[] value) => BitConverter.ToChar(value, 0);

    public static double ToDouble(this byte[] value) => BitConverter.ToDouble(value, 0);

    public static float ToFloat(this byte[] value) => BitConverter.ToSingle(value, 0);

    public static int ToInt(this byte[] value) => BitConverter.ToInt32(value, 0);

    public static long ToLong(this byte[] value) => BitConverter.ToInt64(value, 0);

    public static short ToShort(this byte[] value) => BitConverter.ToInt16(value, 0);

    public static uint ToUInt(this byte[] value) => BitConverter.ToUInt32(value, 0);

    public static ulong ToULong(this byte[] value) => BitConverter.ToUInt64(value, 0);

    public static ushort ToUShort(this byte[] value) => BitConverter.ToUInt16(value, 0);

    #endregion Bytes

    #region Chars

    public static bool ToBoolean(this char value) => value == 1 ? true : false;

    public static byte ToByte(this char value) => Convert.ToByte(value);

    public static decimal ToDecimal(this char value) => Convert.ToDecimal(value);

    public static double ToDouble(this char value) => Convert.ToDouble(value);

    public static float ToFloat(this char value) => Convert.ToSingle(value);

    public static int ToInt(this char value) => Convert.ToInt32(value);

    public static long ToLong(this char value) => Convert.ToInt64(value);

    public static short ToShort(this char value) => Convert.ToInt16(value);

    public static uint ToUInt(this char value) => Convert.ToUInt32(value);

    public static ulong ToULong(this char value) => Convert.ToUInt64(value);

    public static ushort ToUShort(this char value) => Convert.ToUInt16(value);

    #endregion Chars

    #region Decimals

    public static bool ToBoolean(this decimal value) => value == 1 ? true : false;

    public static char ToChar(this decimal value) => Convert.ToChar(value);

    #endregion Decimals

    #region Doubles

    public static bool ToBoolean(this double value) => value == 1 ? true : false;

    public static byte ToByte(this double value) => Convert.ToByte(value);

    public static char ToChar(this double value) => Convert.ToChar(value);

    public static decimal ToDecimal(this double value) => (decimal)value;

    public static float ToFloat(this double value) => (float)value;

    public static int ToInt(this double value) => (int)value;

    public static long ToLong(this double value) => (long)value;

    public static short ToShort(this double value) => (short)value;

    public static uint ToUInt(this double value) => (uint)value;

    public static ulong ToULong(this double value) => (ulong)value;

    public static ushort ToUShort(this double value) => (ushort)value;

    #endregion Doubles

    #region Floats

    public static bool ToBoolean(this float value) => value == 1 ? true : false;

    public static byte ToByte(this float value) => Convert.ToByte(value);

    public static char ToChar(this float value) => Convert.ToChar(value);

    public static decimal ToDecimal(this float value) => (decimal)value;

    public static int ToInt(this float value) => (int)value;

    public static long ToLong(this float value) => (long)value;

    public static short ToShort(this float value) => (short)value;

    public static uint ToUInt(this float value) => (uint)value;

    public static ulong ToULong(this float value) => (ulong)value;

    public static ushort ToUShort(this float value) => (ushort)value;

    #endregion Floats

    #region Ints

    public static bool ToBoolean(this int value) => value == 1 ? true : false;

    public static byte ToByte(this int value) => Convert.ToByte(value);

    public static char ToChar(this int value) => Convert.ToChar(value);

    public static short ToShort(this int value) => (short)value;

    public static uint ToUInt(this int value) => (uint)value;

    public static ulong ToULong(this int value) => (ulong)value;

    public static ushort ToUShort(this int value) => (ushort)value;

    #endregion Ints

    #region Longs

    public static bool ToBoolean(this long value) => value == 1 ? true : false;

    public static byte ToByte(this long value) => Convert.ToByte(value);

    public static char ToChar(this long value) => Convert.ToChar(value);

    public static int ToInt(this long value) => (int)value;

    public static short ToShort(this long value) => (short)value;

    public static uint ToUInt(this long value) => (uint)value;

    public static ulong ToULong(this long value) => (ulong)value;

    public static ushort ToUShort(this long value) => (ushort)value;

    #endregion Longs

    #region Shorts

    public static bool ToBoolean(this short value) => value == 1 ? true : false;

    public static byte ToByte(this short value) => Convert.ToByte(value);

    public static char ToChar(this short value) => Convert.ToChar(value);

    public static uint ToUInt(this short value) => (uint)value;

    public static ulong ToULong(this short value) => (ulong)value;

    public static ushort ToUShort(this short value) => (ushort)value;

    #endregion Shorts

    #region Strings

    public static bool ToBoolean(this string value) => bool.Parse(value);

    public static byte ToByte(this string value) => byte.Parse(value);

    public static char ToChar(this string value) => char.Parse(value);

    public static DateTime ToDateTime(this string value) => DateTime.Parse(value);

    public static decimal ToDecimal(this string value) => decimal.Parse(value);

    public static double ToDouble(this string value) => double.Parse(value);

    public static float ToFloat(this string value) => float.Parse(value);

    public static int ToInt(this string value) => int.Parse(value);

    public static long ToLong(this string value) => long.Parse(value);

    public static Quaternion ToQuaternion(this string value) {
        if (value.StartsWith("(") && value.EndsWith(")"))
            value = value.Substring(1, value.Length - 2);
        string[] sArray = value.Split(',');
        Quaternion result = new Quaternion(float.Parse(sArray[0]), float.Parse(sArray[1]), float.Parse(sArray[2]), float.Parse(sArray[3]));

        return result;
    }

    public static short ToShort(this string value) => short.Parse(value);

    public static uint ToUInt(this string value) => uint.Parse(value);

    public static ulong ToULong(this string value) => ulong.Parse(value);

    public static ushort ToUShort(this string value) => ushort.Parse(value);

    public static Vector2 ToVector2(this string value) {
        if (value.StartsWith("(") && value.EndsWith(")"))
            value = value.Substring(1, value.Length - 2);
        string[] sArray = value.Split(',');
        Vector2 result = new Vector2(float.Parse(sArray[0]), float.Parse(sArray[1]));

        return result;
    }

    public static Vector2Int ToVector2Int(this string value) {
        if (value.StartsWith("(") && value.EndsWith(")"))
            value = value.Substring(1, value.Length - 2);
        string[] sArray = value.Split(',');
        Vector2Int result = new Vector2Int(int.Parse(sArray[0]), int.Parse(sArray[1]));

        return result;
    }

    public static Vector3 ToVector3(this string value) {
        if (value.StartsWith("(") && value.EndsWith(")"))
            value = value.Substring(1, value.Length - 2);
        string[] sArray = value.Split(',');
        Vector3 result = new Vector3(float.Parse(sArray[0]), float.Parse(sArray[1]), float.Parse(sArray[2]));

        return result;
    }

    public static Vector3Int ToVector3Int(this string value) {
        if (value.StartsWith("(") && value.EndsWith(")"))
            value = value.Substring(1, value.Length - 2);
        string[] sArray = value.Split(',');
        Vector3Int result = new Vector3Int(int.Parse(sArray[0]), int.Parse(sArray[1]), int.Parse(sArray[2]));

        return result;
    }

    public static Vector4 ToVector4(this string value) {
        if (value.StartsWith("(") && value.EndsWith(")"))
            value = value.Substring(1, value.Length - 2);
        string[] sArray = value.Split(',');
        Vector4 result = new Vector4(float.Parse(sArray[0]), float.Parse(sArray[1]), float.Parse(sArray[2]), float.Parse(sArray[3]));

        return result;
    }

    #endregion Strings
}