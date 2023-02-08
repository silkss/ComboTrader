using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using Connectors;
using Connectors.Types;
using Connectors.Events;

internal class Program
{
    static Future? Future;
    static List<Option> Options = new();
    static IConnector Connector = new Connectors.Ib.Connector();

    static void OnConnected(IConnector connector)
    {
        connector.ReqFuture();
    }

    static void OnFutureAdded(IConnector connector, FutureArgs args)
    {
        Future = args.Future;
        connector.ReqOption();
    }
    static void OnOptionAdded(IConnector connector, OptionArgs args)
    {
        var opt = args.Option;
        Options.Add(opt);
    }
    static void Init()
    {
        Connector.ConnectionEstablished += OnConnected;
        Connector.FutureAdded += OnFutureAdded;
        Connector.OptionAdded += OnOptionAdded;
    }

    private static void Main(string[] args)
    {
        Init();
        Connector.Connect();
        Console.WriteLine("Enter price");
        if (Console.ReadLine() is string str)
        {
            double price = double.Parse(str, CultureInfo.InvariantCulture);

            var sortedOptions = Options.OrderBy(o => o.Strike).ToList();
            var baseOption = sortedOptions.MinBy(o => Math.Abs(o.Strike - price));
            if (baseOption != null)
            {
                var baseOptionIdx = sortedOptions.FindIndex(o => o.Strike == baseOption.Strike);

                var buyIdx = baseOptionIdx + 2;
                var sellIdx = baseOptionIdx + 5;

                var buyOption = sortedOptions[buyIdx];
                var sellOption = sortedOptions[sellIdx];

                Connector.SendStrategyOrder(buyOption, sellOption, "EUR", 1m, "BUY", "CME");
            }
        }

        Console.ReadKey();
    }
}