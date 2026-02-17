using CodeShare;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cvnet10Asset;
using Cvnet10Base;
using Cvnet10Wpfclient.ViewServices;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace Cvnet10Wpfclient.ViewModels.Sub;

/// <summary>
/// メンテ画面共通処理
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract partial class BaseMenteViewModel<T> : Helpers.BaseViewModel where T : BaseDbClass, new() {
	[ObservableProperty]
	ObservableCollection<T> listData = [];

	[ObservableProperty]
	T current = new();

	partial void OnCurrentChanged(T? oldValue, T newValue) => OnCurrentChangedCore(oldValue, newValue);

	protected virtual void OnCurrentChangedCore(T? oldValue, T newValue) {
		if (newValue == null) {
			CurrentEdit = new();
			return;
		}
		if (oldValue?.Id != newValue.Id && newValue.Id>0) {
			CurrentEdit = Common.CloneObject(newValue);
		}
	}

	[ObservableProperty]
	T currentEdit = new();

	[ObservableProperty]
	int count;

	[ObservableProperty]
	DateTime startTime = DateTime.Now;

	[ObservableProperty]
	TimeSpan getListTime = TimeSpan.Zero;

	protected virtual Type Tabletype => typeof(T);
	protected virtual string? ListWhere => null;
	protected virtual string? ListOrder => null;

	protected virtual bool ConfirmAction(string message) =>
		MessageEx.ShowQuestionDialog(message, owner: ClientLib.GetActiveView(this)) == MessageBoxResult.Yes;

	protected virtual string GetInsertConfirmMessage() => "挿入しますか？";
	protected virtual string GetUpdateConfirmMessage() => "更新しますか？";
	protected virtual string GetDeleteConfirmMessage() => "削除しますか？";

	protected virtual bool CanUpdate() => true;

	protected virtual bool CanDelete() {
		if (ListData.Count == 0) return false;
		return Current.Id > 0;
	}

	protected virtual object CreateInsertParam() =>
		new InsertParam(Tabletype, Common.SerializeObject(CurrentEdit));

	protected virtual object CreateUpdateParam() =>
		new UpdateParam(Tabletype, Common.SerializeObject(CurrentEdit));

	protected virtual object CreateDeleteParam() =>
		new DeleteParam(Tabletype, Common.SerializeObject(CurrentEdit));

	protected virtual void AfterList(IList list) { }
	protected virtual void AfterInsert(T item) { }
	protected virtual void AfterUpdate(T item) { }
	protected virtual void AfterDelete(T removedItem) { }

	/// <summary>
	/// 一覧取得
	/// </summary>
	/// <param name="ct"></param>
	/// <returns></returns>
	[RelayCommand(IncludeCancelCommand = true)]
	protected async Task DoList(CancellationToken ct) {
		StartTime = DateTime.Now;
		try {
			var coreService = AppGlobal.GetgRPCService<ICvnetCoreService>();
			var msg = new CvnetMsg {
				Code = 0,
				Flag = CvnetFlag.Msg101_Op_Query,
				DataType = typeof(QueryListParam),
				DataMsg = Common.SerializeObject(new QueryListParam(
					itemType: Tabletype,
					where: ListWhere,
					order: ListOrder
				))
			};

			var reply = await coreService.QueryMsgAsync(msg, AppGlobal.GetDefaultCallContext(ct));

			if (Common.DeserializeObject(reply.DataMsg ?? "[]", reply.DataType) is IList list) {
				ListData = new ObservableCollection<T>(list.Cast<T>());
				Count = ListData.Count;
				Current = ListData.FirstOrDefault() ?? new T();
				AfterList(list);
			}

			GetListTime = DateTime.Now - StartTime;
		}
		catch (Exception ex) {
			MessageEx.ShowErrorDialog($"データ取得失敗: {ex.Message}", owner: ClientLib.GetActiveView(this));
		}
	}
	/// <summary>
	/// 挿入
	/// </summary>
	/// <param name="ct"></param>
	/// <returns></returns>
	[RelayCommand(IncludeCancelCommand = true)]
	protected async Task DoInsert(CancellationToken ct) {
		if (!ConfirmAction(GetInsertConfirmMessage())) return;

		try {
			ct.ThrowIfCancellationRequested();

			var coreService = AppGlobal.GetgRPCService<ICvnetCoreService>();
			var msg = new CvnetMsg {
				Code = 0,
				Flag = CvnetFlag.Msg201_Op_Execute,
				DataType = typeof(InsertParam),
				DataMsg = Common.SerializeObject(CreateInsertParam())
			};

			var reply = await coreService.QueryMsgAsync(msg, AppGlobal.GetDefaultCallContext(ct));
			var item = Common.DeserializeObject(reply.DataMsg ?? "", reply.DataType) as T;

			if (item != null) {
				ListData.Add(item);
				Count = ListData.Count;
				Current = item;
				AfterInsert(item);
			}
		}
		catch (Exception ex) {
			MessageEx.ShowErrorDialog($"登録失敗: {ex.Message}", owner: ClientLib.GetActiveView(this));
		}
	}
	/// <summary>
	/// 更新
	/// </summary>
	/// <param name="ct"></param>
	/// <returns></returns>
	[RelayCommand(IncludeCancelCommand = true)]
	protected async Task DoUpdate(CancellationToken ct) {
		if (!CanUpdate()) return;
		if (!ConfirmAction(GetUpdateConfirmMessage())) return;

		try {
			ct.ThrowIfCancellationRequested();

			var coreService = AppGlobal.GetgRPCService<ICvnetCoreService>();
			var msg = new CvnetMsg {
				Code = 0,
				Flag = CvnetFlag.Msg201_Op_Execute,
				DataType = typeof(UpdateParam),
				DataMsg = Common.SerializeObject(CreateUpdateParam())
			};

			var reply = await coreService.QueryMsgAsync(msg, AppGlobal.GetDefaultCallContext(ct));

			if (Common.DeserializeObject(reply.DataMsg ?? "", reply.DataType) is T item) {
				Common.DeepCopyValue(Tabletype, item, Current);
				CurrentEdit = Common.CloneObject(Current);
				AfterUpdate(item);
			}
		}
		catch (OperationCanceledException) {
			return;
		}
		catch (Exception ex) {
			MessageEx.ShowErrorDialog($"更新失敗: {ex.Message}", owner: ClientLib.GetActiveView(this));
		}
	}
	/// <summary>
	/// 削除
	/// </summary>
	/// <param name="ct"></param>
	/// <returns></returns>
	[RelayCommand(IncludeCancelCommand = true)]
	protected async Task DoDelete(CancellationToken ct) {
		if (!CanDelete()) {
			MessageEx.ShowWarningDialog("削除対象を選択してください", owner: ClientLib.GetActiveView(this));
			return;
		}

		if (!ConfirmAction(GetDeleteConfirmMessage())) return;

		try {
			ct.ThrowIfCancellationRequested();

			var coreService = AppGlobal.GetgRPCService<ICvnetCoreService>();
			var msg = new CvnetMsg {
				Code = 0,
				Flag = CvnetFlag.Msg201_Op_Execute,
				DataType = typeof(DeleteParam),
				DataMsg = Common.SerializeObject(CreateDeleteParam())
			};

			var reply = await coreService.QueryMsgAsync(msg, AppGlobal.GetDefaultCallContext(ct));
			if (reply.Code != 0) {
				MessageEx.ShowErrorDialog("削除できませんでした", owner: ClientLib.GetActiveView(this));
				return;
			}

			var removedItem = Current;
			var currentIndex = ListData.IndexOf(Current);
			var nextIndex = currentIndex + 1 < ListData.Count ? currentIndex + 1 : 0;
			var nextItem = ListData.ElementAtOrDefault(nextIndex);

			ListData.Remove(Current);
			Current = nextItem ?? ListData.FirstOrDefault() ?? new T();
			Count = ListData.Count;

			AfterDelete(removedItem);
		}
		catch (OperationCanceledException) {
			return;
		}
		catch (Exception ex) {
			MessageEx.ShowErrorDialog($"削除失敗: {ex.Message}", owner: ClientLib.GetActiveView(this));
		}
	}

	[RelayCommand(IncludeCancelCommand = true)]
	protected async Task DoOutputJson(CancellationToken ct) {
		ct.ThrowIfCancellationRequested();

		var outstr = JsonConvert.SerializeObject(ListData, Formatting.Indented);
		var dialog = new Microsoft.Win32.SaveFileDialog {
			FileName = Tabletype.Name + DateTime.Now.ToDtStrDate2(),
			DefaultExt = ".json",
			Filter = "Text documents (.json)|*.json",
			DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
		};

		if (dialog.ShowDialog(ClientLib.GetActiveView(this)) != true) return;

		ct.ThrowIfCancellationRequested();
		await File.WriteAllTextAsync(dialog.FileName, outstr, ct);
	}

}