namespace Connectors;
public interface IConnector
{
    void Connect(string host = "127.0.0.1", int port = 7497, int client_id = 21);
    void ReqFuture(string name = "6EH3", string exchange = "CME");
    void ReqFutures(string symbol = "EUR", string exchange = "CME");
}
