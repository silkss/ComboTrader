namespace Connectors.Ib;

using Connectors.Types;
using IBApi;
using System;
using System.Globalization;

internal static class Extensions
{
    public static DateTime ToDateTime(this string ibDateTime) => DateTime
        .ParseExact(ibDateTime, "yyyyMMdd", CultureInfo.CurrentCulture);
    public static OptionType ToOptionType(this string ibRight) => ibRight switch
    {
        "P" => OptionType.Put,
        "C" => OptionType.Call,
        _ => throw new NotSupportedException("Not supporte option type")
    };
    public static string ToIbDateTime(this DateTime datetime) => datetime.ToString("yyyyMMdd");
    public static Future ToFuture(this ContractDetails contract) => new Future
    {
        Id = contract.Contract.ConId,
        Exchange = contract.Contract.Exchange,
        LastTradeDate = contract.Contract.LastTradeDateOrContractMonth.ToDateTime(),
        Multiplier = int.Parse(contract.Contract.Multiplier),
        Name = contract.Contract.LocalSymbol,
        Symbol = contract.Contract.Symbol,
    };
    public static Option ToOption(this ContractDetails contract) => new Option
    {
        Id = contract.Contract.ConId,
        Exchange = contract.Contract.Exchange,
        LastTradeDate = contract.Contract.LastTradeDateOrContractMonth.ToDateTime(),
        Multiplier = int.Parse(contract.Contract.Multiplier),
        Name = contract.Contract.LocalSymbol,
        Symbol = contract.Contract.Symbol,
        OptionType = contract.Contract.Right.ToOptionType(),
        Strike = contract.Contract.Strike,
        ParentId = contract.UnderConId,
    };
}
