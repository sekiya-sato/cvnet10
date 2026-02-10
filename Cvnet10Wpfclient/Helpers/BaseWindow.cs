using CommunityToolkit.Mvvm.Messaging;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Cvnet10Wpfclient.Helpers;

public class BaseWindow : Window {
	public BaseWindow() {
		// ViewModel 側から Dialog を閉じるための共通メッセージ登録
		WeakReferenceMessenger.Default.Register<DialogCloseMessage>(this, (recipient, message) => {
			if (recipient is Window win) {
				// Show/ShowDialog の違いで DialogResult 設定が例外になる場合があるため安全に扱う
				try {
					win.DialogResult = message.DialogResult;
				}
				catch {
					// Ignore: 表示方式により DialogResult が設定できない場合がある
				}
				win.Close();
				win.Owner?.Activate();
			}
		});
	}

	protected override void OnContentRendered(EventArgs e) {
		base.OnContentRendered(e);

		// ViewModel に ICommand プロパティ "InitCommand" があれば実行する（XAML でのトリガーを共通化）
		var dc = DataContext;
		if (dc == null) return;

		var prop = dc.GetType().GetProperty("InitCommand", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (prop?.GetValue(dc) is ICommand cmd && cmd.CanExecute(null)) {
			cmd.Execute(null);
		}
	}

	protected override void OnClosed(EventArgs e) {
		base.OnClosed(e);
		// メモリリーク防止のため登録解除
		WeakReferenceMessenger.Default.UnregisterAll(this);
	}
}