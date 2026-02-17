using CodeShare;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cvnet10Asset;
using Cvnet10Base;
using Cvnet10Wpfclient.ViewServices;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;


namespace Cvnet10Wpfclient.ViewModels;

internal partial class SysLoginViewModel : Sub.BaseMenteViewModel<SysLogin> {
	[ObservableProperty]
	string title = "ログインマスターメンテ画面";

	[ObservableProperty]
	string lastLoginDate = string.Empty;

	[RelayCommand]
	public async Task Init() {
	}

	protected override bool ConfirmAction(string message) =>
		MessageEx.ShowQuestionDialog(message, owner: ClientLib.GetActiveView(this)) == MsgBoxResult.Yes;

	protected override void OnCurrentChangedCore(SysLogin? oldValue, SysLogin newValue) {
		base.OnCurrentChangedCore(oldValue, newValue);
		LastLoginDate = $"{CurrentEdit.LastDate.ToDateStr()}";
	}

	protected override bool CanUpdate() => Current.Id > 0;

	protected override bool CanDelete() =>
		ListData.Count > 0 && Current.Id > 0;

	protected override void AfterUpdate(SysLogin item) {
		MessageEx.ShowInformationDialog(
			$"更新しました (Login={CurrentEdit.LoginId}, Id={CurrentEdit.Id})",
			owner: ClientLib.GetActiveView(this));
	}

	[RelayCommand]
	public void DoSelectShain() {
		var selWin = new Views.Sub.SelectWinView();
		var vm = selWin.DataContext as Sub.SelectWinViewModel;
		if (vm == null) return;
		vm.SetParam(typeof(MasterShain), "");
		vm.InitParam = (int)CurrentEdit.Id_Shain;
		var ret = ClientLib.ShowDialogView(selWin, this);
		if (ret != true) return;
		var meisho = vm.Current as MasterShain;
		var cur = CurrentEdit as SysLogin;
		if (meisho == null || cur == null) return;
		cur.Id_Shain = meisho.Id;
		cur.VShain?.Sid = meisho.Id;
		cur.VShain?.Cd = meisho.Code;
		cur.VShain?.Mei = meisho.Name;
	}

	[RelayCommand]
	public void DoSetPass() {
		var cur = CurrentEdit as SysLogin;
		if (cur == null) return;
		var warning = (cur.CryptPassword.Length >= 20) ? "すでに暗号化されているようですが、\n" : "";
		if (MessageEx.ShowQuestionDialog(warning + "表示されているパスワードの文字を暗号化しますか？",
			owner: ClientLib.GetActiveView(this)) != MsgBoxResult.Yes)
			return;
		var newPass = Common.EncryptLoginRequest(cur.CryptPassword, cur.VdateC);
		if (!string.IsNullOrEmpty(newPass))
			cur.CryptPassword = newPass;
	}
}
