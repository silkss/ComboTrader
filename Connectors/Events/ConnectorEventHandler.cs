namespace Connectors.Events;

using Connectors.Types;
using System;

public delegate void ConnectorEvent(IConnector connector);
public delegate void ConnectorEvent<EventArgs>(IConnector connector, EventArgs e);

public class FutureArgs : EventArgs
{
    public FutureArgs(Future future) => Future = future;
    public Future Future { get; }
}

public class OptionArgs : EventArgs
{
    public OptionArgs(Option option) => Option = option;
    public Option Option { get; }
}

public class OptionParamsArgs : EventArgs
{
    public OptionParamsArgs(OptionTradingClass optionTradingClass)
    {
        OptionTradingClass = optionTradingClass;
    }
    public OptionTradingClass OptionTradingClass { get; }
}
