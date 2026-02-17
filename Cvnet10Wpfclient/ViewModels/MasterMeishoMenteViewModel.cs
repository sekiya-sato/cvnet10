using CodeShare;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cvnet10Asset;
using Cvnet10Base;
using Cvnet10Wpfclient.ViewServices;
using System.Windows;

namespace Cvnet10Wpfclient.ViewModels;

public partial class MasterMeishoMenteViewModel : Sub.BaseMenteViewModel<MasterMeisho> {
	[ObservableProperty]
	string title = "名称マスター保守";

	protected override string? ListOrder => "Kubun,Code";

	// 初期化
	[RelayCommand]
	async Task Init(CancellationToken ct) {
		//await DoList(ct);
	}

	protected override string GetInsertConfirmMessage() =>
		$"新規登録しますか？ {Current.Code}";

	protected override string GetUpdateConfirmMessage() =>
		$"更新しますか？ (CD={CurrentEdit.Code}, Id={CurrentEdit.Id})";

	protected override string GetDeleteConfirmMessage() =>
		$"削除しますか？ (CD={CurrentEdit.Code}, Id={CurrentEdit.Id})";

	protected override void AfterInsert(MasterMeisho item) {
		MessageEx.ShowInformationDialog(
			$"登録しました (CD={CurrentEdit.Code}, Id={CurrentEdit.Id})",
			owner: ClientLib.GetActiveView(this));
	}

	protected override void AfterUpdate(MasterMeisho item) {
		MessageEx.ShowInformationDialog(
			$"更新しました (CD={CurrentEdit.Code}, Id={CurrentEdit.Id})",
			owner: ClientLib.GetActiveView(this));
	}

	protected override void AfterDelete(MasterMeisho removedItem) {
		MessageEx.ShowInformationDialog(
			$"削除しました (CD={removedItem.Code}, Id={removedItem.Id})",
			owner: ClientLib.GetActiveView(this));
	}

	[RelayCommand]
	void DoSelectKubun() {
		var selWin = new Views.Sub.SelectWinView();
		var vm = selWin.DataContext as Sub.SelectWinViewModel;
		if (vm == null) return;
		vm.SetParam(typeof(MasterMeisho), "Kubun='IDX'", "Code");
		if (ClientLib.ShowDialogView(selWin, this) != true) return;
		var meisho = vm.Current as MasterMeisho;
		CurrentEdit.Kubun = meisho?.Code ?? CurrentEdit.Kubun;
		CurrentEdit.KubunName = meisho?.Name ?? CurrentEdit.KubunName;
	}
}

