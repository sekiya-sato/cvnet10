using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cvnet10Base;
using Cvnet10Wpfclient.ViewModels.Sub;
using Cvnet10Wpfclient.Views.Sub;
using Cvnet10Wpfclient.ViewServices;

namespace Cvnet10Wpfclient.ViewModels._01Master;

public partial class MasterShohinMenteViewModel : Helpers.BaseMenteViewModel<MasterShohin> {
	[ObservableProperty]
	string title = "商品マスターメンテ";

	protected override string? ListWhere => BuildRangeWhere();
	protected override string? ListOrder => "Code";

	SelectCodeParameter? selectCodeParam;

	protected override int? ListMaxCount => selectCodeParam?.MaxCount;

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

	protected override void AfterInsert(MasterShohin item) {
		Message = $"追加しました (CD={item.Code}, Id={item.Id})";
	}

	protected override void AfterUpdate(MasterShohin item) {
		Message = $"修正しました (CD={item.Code}, Id={item.Id})";
	}

	protected override void AfterDelete(MasterShohin removedItem) {
		Message = $"削除しました (CD={removedItem.Code}, Id={removedItem.Id})";
	}

	protected override ValueTask<bool> BeforeListAsync(CancellationToken ct) {
		ct.ThrowIfCancellationRequested();
		var selWin = new SelectCodeView();
		if (selWin.DataContext is not SelectCodeViewModel vm) {
			return new ValueTask<bool>(true);
		}
		selectCodeParam ??= new SelectCodeParameter { DisplayName = "商品" };
		vm.Initialize(selectCodeParam);
		var dialogResult = ClientLib.ShowDialogView(selWin, this, true);
		if (dialogResult != true) {
			return new ValueTask<bool>(false);
		}
		selectCodeParam = new SelectCodeParameter {
			FromId = vm.Parameter.FromId,
			ToId = vm.Parameter.ToId,
			FromCode = string.IsNullOrWhiteSpace(vm.Parameter.FromCode) ? null : vm.Parameter.FromCode,
			ToCode = string.IsNullOrWhiteSpace(vm.Parameter.ToCode) ? null : vm.Parameter.ToCode,
			Name = string.IsNullOrWhiteSpace(vm.Parameter.Name) ? null : vm.Parameter.Name,
			MaxCount = vm.Parameter.MaxCount,
			DisplayName = vm.Parameter.DisplayName
		};
		return new ValueTask<bool>(true);
	}

	string? BuildRangeWhere() {
		if (selectCodeParam == null) return null;
		var clauses = new List<string>();
		if (selectCodeParam.FromId.HasValue) {
			clauses.Add($"Id >= {selectCodeParam.FromId.Value}");
		}
		if (selectCodeParam.ToId.HasValue) {
			clauses.Add($"Id <= {selectCodeParam.ToId.Value}");
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
