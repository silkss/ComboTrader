namespace Connectors.Ib;

using IBApi;
using System;
using System.Threading;

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

    public override void contractDetails(int reqId, ContractDetails contractDetails) => Console
        .WriteLine(
            $" - {contractDetails.Contract.ConId}" +
            $" - {contractDetails.Contract.LocalSymbol}" +
            $" - {contractDetails.Contract.LastTradeDateOrContractMonth.ToDateTime():f}");
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
