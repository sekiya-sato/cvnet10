using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cvnet10Base;
using Cvnet10Wpfclient.ViewModels.Sub;
using Cvnet10Wpfclient.ViewServices;

namespace Cvnet10Wpfclient.ViewModels._01Master;

/// <summary>
/// 仕入先マスターメンテ ViewModel
/// </summary>
public partial class MasterShiireMenteViewModel : Helpers.BaseMenteViewModel<MasterShiire> {
	[ObservableProperty]
	string title = "仕入先マスターメンテ";

	protected override string? ListWhere => BuildSelectCodeWhere(selectCodeParam);
	protected override string? ListOrder => "Code";
	protected override int? ListMaxCount => selectCodeParam?.MaxCount;

	SelectParameter? selectCodeParam;

	[RelayCommand]
	async Task Init() {
		await DoList(CancellationToken.None);
	}

	protected override string GetInsertConfirmMessage() =>
		$"追加しますか？ (CD={CurrentEdit.Code})";

	protected override string GetUpdateConfirmMessage() =>
		$"修正しますか？ (CD={CurrentEdit.Code}, Id={CurrentEdit.Id})";

	protected override string GetDeleteConfirmMessage() =>
		$"削除しますか？ (CD={CurrentEdit.Code}, Id={CurrentEdit.Id})";

	protected override void AfterInsert(MasterShiire item) {
		Message = $"追加しました (CD={item.Code}, Id={item.Id})";
	}

	protected override void AfterUpdate(MasterShiire item) {
		Message = $"修正しました (CD={item.Code}, Id={item.Id})";
	}

	protected override void AfterDelete(MasterShiire removedItem) {
		Message = $"削除しました (CD={removedItem.Code}, Id={removedItem.Id})";
	}

	protected override ValueTask<bool> BeforeListAsync(CancellationToken ct) {
		ct.ThrowIfCancellationRequested();
		if (!TryShowSelectCodeDialog(selectCodeParam, "仕入先", out var parameter)) {
			return new ValueTask<bool>(false);
		}

		selectCodeParam = parameter;
		return new ValueTask<bool>(true);
	}

	[RelayCommand]
	void DoSelectShain() {
		var selWin = new Views.Sub.SelectWinView();
		var vm = selWin.DataContext as Sub.SelectWinViewModel;
		if (vm == null) return;
		vm.SetParam(typeof(MasterShain), "", "Code", startPos: CurrentEdit.Id_Shain);
		if (ClientLib.ShowDialogView(selWin, this) != true) return;
		var shain = vm.Current as MasterShain;
		if (shain == null) return;
		CurrentEdit.Id_Shain = shain.Id;
		CurrentEdit.VShain = new() { Sid = shain.Id, Cd = shain.Code ?? "", Mei = shain.Name ?? "" };
	}

	[RelayCommand]
	void DoSelectPayMethod() {
		var selWin = new Views.Sub.SelectWinView();
		var vm = selWin.DataContext as Sub.SelectWinViewModel;
		if (vm == null) return;
		vm.SetParam(typeof(MasterMeisho), "Kubun='PAY'", "Code", startPos: CurrentEdit.Id_PayMethod);
		if (ClientLib.ShowDialogView(selWin, this) != true) return;
		var meisho = vm.Current as MasterMeisho;
		if (meisho == null) return;
		CurrentEdit.Id_PayMethod = meisho.Id;
		CurrentEdit.VPayMethod = new() { Sid = meisho.Id, Cd = meisho.Code ?? "", Mei = meisho.Name ?? "" };
	}

	[RelayCommand]
	void DoSelectPaysaki() {
		var selWin = new Views.Sub.SelectWinView();
		var vm = selWin.DataContext as Sub.SelectWinViewModel;
		if (vm == null) return;
		vm.SetParam(typeof(MasterShiire), "", "Code", startPos: CurrentEdit.Id_Paysaki);
		if (ClientLib.ShowDialogView(selWin, this) != true) return;
		var shiire = vm.Current as MasterShiire;
		if (shiire == null) return;
		CurrentEdit.Id_Paysaki = shiire.Id;
		CurrentEdit.VPaysaki = new() { Sid = shiire.Id, Cd = shiire.Code ?? "", Mei = shiire.Name ?? "" };
	}
}
