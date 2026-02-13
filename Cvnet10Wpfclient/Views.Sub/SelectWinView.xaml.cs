using CommunityToolkit.Mvvm.Messaging;
using Cvnet10Wpfclient.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Cvnet10Wpfclient.Views.Sub;

/// <summary>
/// SelectWinView.xaml の相互作用ロジック
/// [Interaction logic for SelectWinView.xaml]
/// </summary>
public partial class SelectWinView : Helpers.BaseWindow {
	public SelectWinView() {
		InitializeComponent();
	}

	private void SelectGrid_PreviewKeyDown(object sender, KeyEventArgs e) {
		if (e.Key == Key.Enter) {
			e.Handled = true; // Enterキーのデフォルト動作を無効にする [Disable the default behavior of the Enter key]
			DefaultButton?.Command?.Execute(null); // デフォルトボタンのコマンドを実行 [Execute the command of the default button]
        }
    }
	protected override void OnSourceInitialized(EventArgs e) {
		base.OnSourceInitialized(e);

		if (Owner is not Window owner) return;

		WindowStartupLocation = WindowStartupLocation.Manual;

		var ownerBounds = owner.WindowState == WindowState.Normal
			? new Rect(owner.Left, owner.Top, owner.ActualWidth, owner.ActualHeight)
			: owner.RestoreBounds;

		var width = double.IsNaN(Width) ? ActualWidth : Width;
		var height = double.IsNaN(Height) ? ActualHeight : Height;

		Left = ownerBounds.Left; // 右上合わせなら: ownerBounds.Right - width
		Top = ownerBounds.Top;
	}

	protected override void OnContentRendered(EventArgs e) {
		base.OnContentRendered(e);
		WeakReferenceMessenger.Default.Register<ShortMsg>(this, (recipient, message) => {
			FocusDataGrid(recipient, message);
		});
		}
	private void FocusDataGrid(object recipient, ShortMsg message) {
		SelectGrid.Focus(); // カレントセルがないとスクロールが効かないため、フォーカスを当てる [Focus is applied because scrolling won't work without a current cell]
		if (long.TryParse(message.Value, out var initId)) {
			if (SelectGrid.Items == null || SelectGrid.Items.Count == 0) return;
			if (SelectGrid.Columns == null || SelectGrid.Columns.Count < 2) return;
			// 選択した項目のインデックスを取得 // View上の表示行数12で調整
			// [Retrieve the index of the selected item] // [Adjusted to 12 display rows on the view]
			var selectedIndex = SelectGrid.SelectedIndex;
			if (selectedIndex > 0 && SelectGrid.Items.Count > (selectedIndex + 12)) {
				selectedIndex += 12;
			}
			SelectGrid.ScrollIntoView(SelectGrid.Items[selectedIndex]);
			// カレントセルを設定 [Set the current cell]
			SelectGrid.CurrentCell = new DataGridCellInfo(SelectGrid.Items[0], SelectGrid.Columns[0]);
			SelectGrid.CurrentItem = SelectGrid.SelectedItem;
			SelectGrid.CurrentCell = new DataGridCellInfo();
		}
	}
}
