namespace Connectors.Ib;

using Connectors.Types;
using Connectors.Events;
using IBApi;
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

public class Connector : DefaultEWrapper, IConnector
{
    private readonly EClientSocket _client;
    private readonly EReaderSignal _signal;
    private int _orderId;

    public Connector()
    {
        _signal = new EReaderMonitorSignal();
        _client = new EClientSocket(this, _signal);
    }

    public event ConnectorEvent ConnectionEstablished = delegate { };
    public event ConnectorEvent<FutureArgs> FutureAdded = delegate { };
    public event ConnectorEvent<OptionArgs> OptionAdded = delegate { };
    public event ConnectorEvent<OptionParamsArgs> OptionParamAdded = delegate { };

    public void Connect(string host = "127.0.0.1", int port = 7497, int client_id = 21)
    {
        _client.eConnect(host, port, client_id);
        var reader = new EReader(_client, _signal);
        reader.Start();
        new Thread(() =>
        {
            while (_client.IsConnected())
            {
                _signal.waitForSignal();
                reader.processMsgs();
            }
        })
        { IsBackground = true }
        .Start();

        _client.reqMarketDataType(3);
    }
    public void ReqFuture(string name = "6EH3", string exchange = "CME")
    {
        var contract = new Contract
        {
            LocalSymbol = name.Trim().ToUpper(),
            Exchange = exchange,
            Currency = "USD",
            SecType = "FUT",
        };
        _client.reqContractDetails(_orderId++, contract);
    }
    public void ReqFutures(string symbol = "EUR", string exchange = "CME")
    {
        var contract = new Contract
        {
            Symbol = symbol.Trim().ToUpper(),
            Exchange = exchange.Trim().ToUpper(),
            Currency = "USD",
            SecType = "FUT"
        };
        _client.reqContractDetails(_orderId++, contract);
    }
    public void ReqOption(
        string symbol = "EUR",
        string expiration = "20230303",
        string tradingClass = "EUU",
        string right = "C",
        string exchange = "CME")
    {
        var contract = new Contract
        {
            Symbol = symbol,
            Exchange = exchange,
            Currency = "USD",
            SecType = "FOP",
            TradingClass = tradingClass,
            Right = right,
            LastTradeDateOrContractMonth = expiration
        };
        _client.reqContractDetails(_orderId++, contract);
    }
    public void ReqOptions()
    {
        var contract = new Contract
        {
            SecType = "FOP",
            Symbol = "EUR",
            Exchange = "CME",
            Currency = "USD",
            TradingClass = "EUU",
        };
        _client.reqContractDetails(_orderId++, contract);
    }
    public void ReqOptionsParam(int parentId, string symbol = "EUR", string exchange = "CME")
    {
        _client.reqSecDefOptParams(_orderId++, symbol, exchange, "FUT", parentId);
    }
    public void SendStrategyOrder(Option buy, Option sell, string symbol, decimal qunatity, string action, string exchange)
    {
        var contract = new Contract
        {
            SecType = "BAG",
            Exchange = exchange,
            Symbol = symbol,
            Currency = "USD",
            ComboLegs = new()
        };

        var buyLeg = new ComboLeg
        {
            ConId = buy.Id,
            Action = "BUY",
            Ratio = 1,
        };
        var sellLeg = new ComboLeg
        {
            ConId = sell.Id,
            Action = "SELL",
            Ratio = 1,
        };

        contract.ComboLegs.Add(buyLeg);
        contract.ComboLegs.Add(sellLeg);

        var order = new Order
        {
            Action = action,
            TotalQuantity = qunatity,
            OrderType = "MKT"
        };
        _client.placeOrder(_orderId++, contract, order);
    }

    public override void openOrder(int orderId, Contract contract, Order order, OrderState orderState)
    {
        Console.WriteLine($"{orderId}\t" +
            $"{contract.LocalSymbol}\t" +
            $"{contract.SecType}");
        Console.WriteLine($"{orderState.Commission}\t" +
            $"{orderState.Status}");
    }
    public override void orderStatus(int orderId, string status, decimal filled, decimal remaining, double avgFillPrice, int permId, int parentId, double lastFillPrice, int clientId, string whyHeld, double mktCapPrice)
    {
        Console.WriteLine($"{orderId}\t{status}");
    }
    public override void securityDefinitionOptionParameter(int reqId, string exchange, int underlyingConId, string tradingClass, string multiplier, HashSet<string> expirations, HashSet<double> strikes)
    {
        foreach (var expiration in expirations)
        {
            var args = new OptionTradingClass(tradingClass, expiration.ToDateTime(), exchange, strikes);
            OptionParamAdded?.Invoke(this, new(args));
        }
    }
    public override void connectAck() => ConnectionEstablished?.Invoke(this);
    public override void contractDetails(int reqId, ContractDetails contractDetails)
    {
        switch (contractDetails.Contract.SecType)
        {
            case "FUT":
                var fut = contractDetails.ToFuture();
                FutureAdded?.Invoke(this, new(fut));
                break;
            case "FOP":
                var opt = contractDetails.ToOption();
                OptionAdded?.Invoke(this, new(opt));
                break;
            default:
                break;
        }
    }
    public override void nextValidId(int orderId) => _orderId = orderId;
    public override void error(int id, int errorCode, string errorMsg, string advancedOrderRejectJson)
    {
        Console.WriteLine($"{errorCode}::{errorMsg}");
    }
    public override void error(Exception e)
    {
        Console.WriteLine(e.Message);
    }
    public override void error(string str)
    {
        Console.WriteLine(str);
    }
}
