using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cvnet10Base;
using Cvnet10Wpfclient.ViewModels.Sub;
using Cvnet10Wpfclient.Views.Sub;
using Cvnet10Wpfclient.ViewServices;

namespace Cvnet10Wpfclient.ViewModels._01Master;

public partial class MasterEndCustomerMenteViewModel : Helpers.BaseMenteViewModel<MasterEndCustomer> {
	[ObservableProperty]
	string title = "顧客マスターメンテ";

	protected override string? ListWhere => BuildRangeWhere();
	protected override string? ListOrder => "Code";

	SelectCodeParameter? selectCodeParam;

	protected override int? ListMaxCount => string.IsNullOrWhiteSpace(selectCodeParam?.MaxCount) ? null : int.TryParse(selectCodeParam.MaxCount, out var mc) ? mc : null;

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

	protected override void AfterInsert(MasterEndCustomer item) {
		Message = $"追加しました (CD={item.Code}, Id={item.Id})";
	}

	protected override void AfterUpdate(MasterEndCustomer item) {
		Message = $"修正しました (CD={item.Code}, Id={item.Id})";
	}

	protected override void AfterDelete(MasterEndCustomer removedItem) {
		Message = $"削除しました (CD={removedItem.Code}, Id={removedItem.Id})";
	}

	protected override ValueTask<bool> BeforeListAsync(CancellationToken ct) {
		ct.ThrowIfCancellationRequested();
		var selWin = new SelectCodeView();
		if (selWin.DataContext is not SelectCodeViewModel vm) {
			return new ValueTask<bool>(true);
		}
		selectCodeParam ??= new SelectCodeParameter { CdName = "顧客" };
		vm.Initialize(selectCodeParam);
		var dialogResult = ClientLib.ShowDialogView(selWin, this, true);
		if (dialogResult != true) {
			return new ValueTask<bool>(false);
		}
		selectCodeParam = new SelectCodeParameter {
			FromId = string.IsNullOrWhiteSpace(vm.Parameter.FromId) ? null : vm.Parameter.FromId,
			ToId = string.IsNullOrWhiteSpace(vm.Parameter.ToId) ? null : vm.Parameter.ToId,
			FromCode = string.IsNullOrWhiteSpace(vm.Parameter.FromCode) ? null : vm.Parameter.FromCode,
			ToCode = string.IsNullOrWhiteSpace(vm.Parameter.ToCode) ? null : vm.Parameter.ToCode,
			Name = string.IsNullOrWhiteSpace(vm.Parameter.Name) ? null : vm.Parameter.Name,
			MaxCount = string.IsNullOrWhiteSpace(vm.Parameter.MaxCount) ? null : vm.Parameter.MaxCount,
			CdName = vm.Parameter.CdName
		};
		return new ValueTask<bool>(true);
	}

	[RelayCommand]
	void DoSelectTenpo() {
		var selWin = new Views.Sub.SelectWinView();
		var vm = selWin.DataContext as Sub.SelectWinViewModel;
		if (vm == null) return;
		vm.SetParam(typeof(MasterTokui), "TenType=6", "Code");
		if (ClientLib.ShowDialogView(selWin, this) != true) return;
		var meisho = vm.Current as MasterTokui;
		CurrentEdit.Id_Tenpo = meisho?.Id ?? 0;
		CurrentEdit.VTenpo = new() { Sid = meisho?.Id ?? 0, Cd = meisho?.Code ?? "", Mei = meisho?.Name ?? "" };
	}

	string? BuildRangeWhere() {
		if (selectCodeParam == null) return null;
		var clauses = new List<string>();
		if (!string.IsNullOrWhiteSpace(selectCodeParam.FromId)) {
			if (long.TryParse(selectCodeParam.FromId, out var fromIdVal)) {
				clauses.Add($"Id >= {fromIdVal}");
			}
		}
		if (!string.IsNullOrWhiteSpace(selectCodeParam.ToId)) {
			if (long.TryParse(selectCodeParam.ToId, out var toIdVal)) {
				clauses.Add($"Id <= {toIdVal}");
			}
		}
		if (!string.IsNullOrWhiteSpace(selectCodeParam.FromCode)) {
			clauses.Add($"Code >= '{Escape(selectCodeParam.FromCode)}'");
		}
		if (!string.IsNullOrWhiteSpace(selectCodeParam.ToCode)) {
			clauses.Add($"Code <= '{Escape(selectCodeParam.ToCode)}'");
		}
		if (!string.IsNullOrWhiteSpace(selectCodeParam.Name)) {
			clauses.Add($"Name LIKE '%{Escape(selectCodeParam.Name)}%'");
		}
		return clauses.Count == 0 ? null : string.Join(" AND ", clauses);
	}

	static string Escape(string value) => value.Replace("'", "''");
}
