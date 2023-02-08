namespace Connectors.Types.Base;
using System;

public class Instrument
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }
    public string Exchange { get; set; }
    public int Multiplier { get; set; }
    public DateTime LastTradeDate { get; set; }
}
