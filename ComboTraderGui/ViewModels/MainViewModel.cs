namespace ComboTraderGui.ViewModels;

using Connectors;
using Connectors.Types;
using System.Collections.ObjectModel;

using ComboTraderGui.Types;
using ComboTraderGui.Commands.Base;
using System.Linq;

internal class MainViewModel : Base.ViewModel
{
    private readonly IConnector _connector = new Connectors.Ib.Connector();
    public MainViewModel()
    {

        _connector.ConnectionEstablished += (conn) => { conn.ReqFuture(); };
        _connector.FutureAdded += (conn, args) =>
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Futures.Add(new(args.Future));
            });
        };
        _connector.OptionAdded += (conn, args) =>
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Futures.First(f => f.ParentFuture.Id == args.Option.ParentId).Options.Add(args.Option);
            });
        };

        _connector.Connect();

        Connect = new LambdaCommand(_ => { _connector.Connect(); });
        ReqOptions = new LambdaCommand(onReqOptions, canReqOptions);
    }

    public ObservableCollection<FutAndOptions> Futures { get; } = new();

    private FutAndOptions? _selectedFutures;
    public FutAndOptions? SelectedFuture
    {
        get => _selectedFutures;
        set => Set(ref _selectedFutures, value);
    }

    private string? _tradingClass = "EUU";
    public string? TradingClass
    {
        get => _tradingClass;
        set => Set(ref _tradingClass, value);
    }

    public LambdaCommand Connect { get; }

    public LambdaCommand ReqOptions { get; }
    private bool canReqOptions(object? p) => SelectedFuture != null &&
        !string.IsNullOrEmpty(TradingClass);
    private void onReqOptions(object? p)
    {
        _connector.ReqOption(
            symbol: SelectedFuture!.ParentFuture.Symbol,
            expiration: "20230303",
            tradingClass: TradingClass!,
            right: "C",
            exchange: SelectedFuture.ParentFuture.Exchange);
    }
}
