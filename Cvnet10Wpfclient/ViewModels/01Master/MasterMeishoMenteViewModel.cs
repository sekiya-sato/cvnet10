using CodeShare;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cvnet10Asset;
using Cvnet10Base;
using Cvnet10Wpfclient.ViewServices;
using System.Collections.ObjectModel;

namespace Cvnet10Wpfclient.ViewModels._01Master;

public partial class MasterMeishoMenteViewModel : Helpers.BaseMenteViewModel<MasterMeisho> {
	[ObservableProperty]
	string title = "名称マスターメンテ";

	[ObservableProperty]
	ObservableCollection<MasterMeisho> kubunList = new();

	[ObservableProperty]
	MasterMeisho? selectedKubun;


	bool suppressSelectedKubunChanged;

	protected override string? ListOrder => "Kubun,Code";

	protected override string? ListWhere {
		get {
			var code = SelectedKubun?.Code;
			if (string.IsNullOrWhiteSpace(code))
				return null;
			var safeCode = code.Replace("'", "''");
			return $"Kubun='{safeCode}'";
		}
	}

	// 初期化
	[RelayCommand]
	async Task Init(CancellationToken ct) {
		await LoadKubunListAsync(ct);
		if (KubunList.Count == 0) {
			ListData.Clear();
			Count = 0;
			return;
		}
		suppressSelectedKubunChanged = true;
		SelectedKubun = KubunList.First(c => c.Code == "BRD") ?? KubunList.FirstOrDefault();
		suppressSelectedKubunChanged = false;
		if (SelectedKubun != null) {
			await RefreshListForSelectedKubunAsync(ct);
		}
	}

	async Task LoadKubunListAsync(CancellationToken ct) {
		try {
			ClientLib.Cursor2Wait();
			var coreService = AppGlobal.GetgRPCService<ICvnetCoreService>();
			var msg = new CvnetMsg {
				Code = 0,
				Flag = CvnetFlag.Msg101_Op_Query,
				DataType = typeof(QueryListParam),
				DataMsg = Common.SerializeObject(new QueryListParam(
					itemType: typeof(MasterMeisho),
					where: "Kubun='IDX'",
					order: "Code"
				))
			};

			var reply = await coreService.QueryMsgAsync(msg, AppGlobal.GetDefaultCallContext(ct));
			if (Common.DeserializeObject(reply.DataMsg ?? "[]", reply.DataType) is System.Collections.IList list) {
				KubunList = new ObservableCollection<MasterMeisho>(list.Cast<MasterMeisho>());
			}
		}
		catch (OperationCanceledException) {
			Message = "区分一覧取得がキャンセルされました";
		}
		catch (Exception ex) {
			Message = $"区分一覧取得失敗: {ex.Message}";
			MessageEx.ShowErrorDialog(Message, owner: ClientLib.GetActiveView(this));
		}
		finally {
			ClientLib.Cursor2Normal();
		}
	}

	async Task RefreshListForSelectedKubunAsync(CancellationToken ct) {
		if (SelectedKubun == null) {
			ListData.Clear();
			Count = 0;
			return;
		}
		await DoListCommand.ExecuteAsync(ct);
	}

	protected override string GetInsertConfirmMessage() =>
		$"追加しますか？ (CD={Current.Code})";

	protected override string GetUpdateConfirmMessage() =>
		$"修正しますか？ (CD={CurrentEdit.Code}, Id={CurrentEdit.Id})";

	protected override string GetDeleteConfirmMessage() =>
		$"削除しますか？ (CD={CurrentEdit.Code}, Id={CurrentEdit.Id})";

	protected override void AfterInsert(MasterMeisho item) {
		Message = $"追加しました (CD={item.Code}, Id={item.Id})";
	}

	protected override void AfterUpdate(MasterMeisho item) {
		Message = $"修正しました (CD={item.Code}, Id={item.Id})";
	}

	protected override void AfterDelete(MasterMeisho removedItem) {
		Message = $"削除しました (CD={removedItem.Code}, Id={removedItem.Id})";
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

	partial void OnSelectedKubunChanged(MasterMeisho? oldValue, MasterMeisho? newValue) {
		if (suppressSelectedKubunChanged) return;
		_ = RefreshListForSelectedKubunAsync(CancellationToken.None);
	}
}

