namespace ComboTraderGui.Types;

using Connectors.Types;
using System.Collections.ObjectModel;

internal class FutAndOptions
{
    public FutAndOptions(Future future)
    {
        ParentFuture = future;
    }
    public Future ParentFuture { get; }
    public ObservableCollection<Option> Options { get; } = new();

    public override string ToString() => $"{ParentFuture.Name} {ParentFuture.Exchange}";
}
