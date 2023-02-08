namespace Connectors.Types;

using System;
using System.Collections.Generic;

public record OptionTradingClass(string TradingClass, DateTime ExpirationDate, string Exchange, HashSet<double> Strikes);
