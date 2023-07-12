using System.Globalization;

namespace PutridParrot.Expressions;

internal static class ObjectComparer
{
    public static bool Contains(object value, object[] items) =>
        items.Any(o => Compare(value, o) == 0);

    public static int Compare(object a, object b)
    {
        if (a == null || b == null)
            return a == null && b == null ? 0 : a == null ? -1 : 1;

        if (a == b)
            return 0;

        return TryCompare(a, b);
    }

    private static int TryCompare(object a, object b)
    {
        switch (a)
        {
            case DateTime aDateTime:
                return CompareDateTime(aDateTime, b);
            case int aInteger:
                return CompareNumber(aInteger, b);
            case float aFloat:
                return CompareNumber(aFloat, b);
            case double aDouble:
                return CompareNumber(aDouble, b);
            case bool aBoolean:
                return CompareBoolean(aBoolean, b);
            case string aString:
                return CompareString(aString, b);
        }

        throw new NotSupportedException();
    }

    private static int TryCompare<T>(T a, object b)
    {
        switch (a)
        {
            case DateTime aDateTime:
                return CompareDateTime(aDateTime, b);
            case double aDouble:
                return CompareNumber(aDouble, b);
            case bool aBoolean:
                return CompareBoolean(aBoolean, b);
            case string aString:
                return CompareString(aString, b);
        }

        throw new NotSupportedException();
    }

    private static int Clamp(int i) =>
        i < 0 ? -1 : i > 0 ? 1 : 0;

    public static int CompareString(string a, object b)
    {
        switch (b)
        {
            case DateTime bDateTime:
                return CompareDateTime(a, bDateTime);
            case int bInteger:
                return CompareNumber(a, bInteger);
            case float bFloat:
                return CompareNumber(a, bFloat);
            case double bDouble:
                return CompareNumber(a, bDouble);
            case bool bBoolean:
                return CompareBoolean(a, bBoolean);
            case string bString:
                return Clamp(String.Compare(a, bString, StringComparison.Ordinal));
        }

        throw new NotSupportedException();
    }

    private static int CompareDouble(double a, double b) =>
        a < b ? -1 : a > b ? 1 : 0;

    public static int CompareNumber(double a, object b)
    {
        switch (b)
        {
            case int bInteger:
                return CompareDouble(a, bInteger);
            case float bFloat:
                return CompareDouble(a, bFloat);
            case double bDouble:
                return CompareDouble(a, bDouble);
            case string bString:
                if (Double.TryParse(bString, out var bValue))
                    return CompareDouble(a, bValue);
                break;
        }

        throw new NotSupportedException();
    }

    public static int CompareNumber(object a, double b)
    {
        switch (a)
        {
            case int aInteger:
                return CompareDouble(aInteger, b);
            case float aFloat:
                return CompareDouble(aFloat, b);
            case double aDouble:
                return CompareDouble(aDouble, b);
            case string aString:
                if (Double.TryParse(aString, out var aValue))
                    return CompareDouble(aValue, b);
                break;
        }

        throw new NotSupportedException();
    }

    private static int CompareBoolean(bool a, bool b)
    {
        var aInt = a ? 1 : 0;
        var bInt = b ? 1 : 0;
        return CompareDouble(aInt, bInt);
    }

    public static int CompareBoolean(bool a, object b)
    {
        switch (b)
        {
            case bool bBoolean:
                return CompareBoolean(a, bBoolean);
            case string bString:
                if (bool.TryParse(bString, out var bValue))
                    return CompareBoolean(a, bValue);
                break;
        }

        throw new NotSupportedException();
    }

    public static int CompareBoolean(object a, bool b)
    {
        switch (a)
        {
            case bool aBoolean:
                return CompareBoolean(aBoolean, b);
            case string aString:
                if (bool.TryParse(aString, out var aValue))
                    return CompareBoolean(aValue, b);
                break;
        }

        throw new NotSupportedException();
    }

    public static int CompareDateTime(DateTime a, object b)
    {
        switch (b)
        {
            case DateTime bDateTime:
                return Clamp(DateTime.Compare(a, bDateTime));
            case string bString:
                if (DateTime.TryParse(bString, out var bValue))
                    return Clamp(DateTime.Compare(a, bValue));
                if (DateTime.TryParseExact(bString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out bValue))
                    return Clamp(DateTime.Compare(a, bValue));
                break;
        }

        return TryCompare(a, b);
    }

    public static int CompareDateTime(object a, DateTime b)
    {
        switch (a)
        {
            case DateTime aDateTime:
                return Clamp(DateTime.Compare(aDateTime, b));
            case string aString:
                if (DateTime.TryParse(aString, out var aValue))
                    return Clamp(DateTime.Compare(aValue, b));
                if (DateTime.TryParseExact(aString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out aValue))
                    return Clamp(DateTime.Compare(aValue, b));
                break;
        }

        return TryCompare(a, b);
    }
}