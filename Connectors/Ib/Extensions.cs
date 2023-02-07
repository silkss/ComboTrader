namespace Connectors.Ib;

using System;
using System.Globalization;

internal static class Extensions
{
    public static DateTime ToDateTime(this string ibDateTime) => DateTime
        .ParseExact(ibDateTime, "yyyyMMdd", CultureInfo.CurrentCulture);
}
