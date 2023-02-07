using Connectors;
using System;
using System.Threading.Tasks;

IConnector connector = new Connectors.Ib.Connector();

connector.Connect();

Console.ReadKey();

connector.ReqFutures();

Console.ReadKey();
