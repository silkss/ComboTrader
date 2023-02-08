namespace Connectors.Types;
public class Option : Base.Instrument
{
    public OptionType OptionType { get; set; }
    public double Strike { get; set; }
    public int ParentId { get; set; }
}
