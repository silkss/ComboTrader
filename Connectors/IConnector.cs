namespace Connectors;

using Connectors.Events;
using Connectors.Types;

public interface IConnector
{
    event ConnectorEvent ConnectionEstablished;
    event ConnectorEvent<FutureArgs> FutureAdded;
    event ConnectorEvent<OptionArgs> OptionAdded;
    event ConnectorEvent<OptionParamsArgs> OptionParamAdded;

    void Connect(string host = "127.0.0.1", int port = 7497, int client_id = 21);
    void ReqFuture(string name = "6EH3", string exchange = "CME");
    void ReqFutures(string symbol = "EUR", string exchange = "CME");
    void ReqOption(
        string symbol = "EUR", 
        string expiration = "20230303", 
        string tradingClass = "EUU", 
        string right = "C",
        string exchange = "CME");
    void ReqOptionsParam(int parentId, string symbol = "EUR", string exchange = "CME");
    void SendStrategyOrder(Option buy, Option sell, string symbol, decimal qunatity, string action, string exchange);
}
