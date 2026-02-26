using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Cvnet10Wpfclient.ViewModels;

public partial class ZorgViewModel : Helpers.BaseViewModel {
	/*
	[ObservableProperty]
	private string? orgTest01;

	[RelayCommand]
	async Task Init(CancellationToken ct) {
		await DoSomeList(ct);
	}
	[RelayCommand(IncludeCancelCommand = true)]
	private async Task DoSomeTask(CancellationToken cancellationToken) {
	}
	protected override void OnExit() {
		if (MessageEx.ShowQuestionDialog("終了しますか？", owner: ClientLib.GetActiveView(this)) == MessageBoxResult.Yes) {
			ExitWithResultFalse();
		}
	}
	*/
	[ObservableProperty]
	private ObservableCollection<ColorBalanceItem> colorItems = new()
	{
		new("#212025", "#E74545", "#742424","#FFFFFF"),
		new("#4B3A42", "#F38884", "#A45055","#FFFFFF"),
		new("#180A14", "#91242C", "#501919","#"),
		new("#FFFBF2", "#876931", "#E7DECB","#"),
		new("#FFF4E1", "#C6A260", "#F0E1CC","#"),
		new("#72563B", "#392F28", "#866E5A","#"),
		new("#65D0C4", "#FF4F84", "#B4EC38","#"),
		new("#FFCA3E", "#FF8D89", "#C9E9E5","#"),
		new("#C8F0EB", "#F2E073", "#FF90BE","#"),
		new("#46938A", "#BA8C27", "#B7255A","#"),
		new("#AF949B", "#E95774", "#9C6372","#FFFFFF"),
		new("#EEE9E4", "#D75A00", "#FAD61B","#"),
		new("#F8B97E", "#FBFBF8", "#C5BEB6","#"),
		new("#58C6F1", "#EBD6BC", "#F2B134","#227190"),
		new("#FFE4E4", "#E22B26", "#FFCBBD","#EF593C"),
		new("#F1F2EF", "#EFEC49", "#33933A","#D2D6C1"),
		new("#DCF0F8", "#2A61B3", "#FFFFFF","#B7E0E4"),
		new("#131A34", "#2F2F2F", "#F5F5F5","#ADFF00"),
		new("#F2F2F2", "#95A4B7", "#5B5F6A","#FFFFFF"),
		new("#FCE07E", "#323232", "#D4F170","#F4E5B3"),
		new("#DEF1EF", "#BAE6DF", "#FFDED5","#EBA295"),
		new("#FDFEE1", "#C9E3B4", "#F7C8B1","#EEEE89"),
		new("#D3E4F4", "#424F7A", "#DCD4E6","#E9DCED"),
		new("#019AA7", "#F5BBB9", "#F5D235","#"),
		new("#5449EA", "#FE4A87", "#FE4137","#"),
		new("#E5C1CC", "#FC6B91", "#BC8496",""),
		new("#725661", "#93344A", "#4C1E2D",""),
		new("#", "#", "#","#"),
	};

}


/// <summary>
/// カラーバランス確認用アイテムモデル
/// </summary>
public record ColorBalanceItem(string Color1, string Color2, string Color3, string Color4);

