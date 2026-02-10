using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Cvnet10Wpfclient.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Cvnet10Wpfclient.Helpers;
public partial class BaseViewModel : ObservableObject {
	public int InitParam { get; set; }
	public string? AddInfo { get; set; }


	// Base が提供する Exit コマンド（派生は OnExit をオーバーライドして振る舞いを変えられる）
	public ICommand ExitCommand { get; }

	public BaseViewModel() {
		ExitCommand = new RelayCommand(OnExit);
	}

	protected virtual void OnExit() {
		// デフォルト動作: Dialog を閉じる（false）
		WeakReferenceMessenger.Default.Send(new DialogCloseMessage(false));
	}

	public void ExitWithResultTrue() {
		WeakReferenceMessenger.Default.Send(new DialogCloseMessage(true));
	}
	public void ExitWithResultFalse() {
		WeakReferenceMessenger.Default.Send(new DialogCloseMessage(false));
	}
}

public record class DialogCloseMessage(bool DialogResult);