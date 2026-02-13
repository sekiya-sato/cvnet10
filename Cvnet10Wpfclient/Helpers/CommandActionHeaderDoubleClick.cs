/*
# file name
CommandActionHeaderDoubleClick.cs

# description
DataGridColumnHeaderのダブルクリックを検知して、列ヘッダー名を引数にICommandを実行するTriggerAction
動作概要
•	Invoke(object)でMouseButtonEventArgsを受け取る
•	VisualTreeHelperでDataGridColumnHeaderを探索
•	見つけたヘッダー名をCommandに渡して実行

# example
Veiw側:
xmlns:i="http://schemas.microsoft.com/xaml/behaviors"
xmlns:behaviors="clr-namespace:CvnetClient.Behaviors"

<DataGrid>
    <i:Interaction.Triggers>
        <i:EventTrigger EventName="MouseDoubleClick">
            <behaviors:HeaderDoubleClickCommandAction Command="{Binding HeaderDoubleClickCommand}" />
        </i:EventTrigger>
    </i:Interaction.Triggers>
</DataGrid>
ViewModel側:
public ICommand HeaderDoubleClickCommand { get; }
public MainViewModel() {
	HeaderDoubleClickCommand = new RelayCommand<string>(header => {
		// ヘッダー名に応じた処理
		MessageBox.Show($"Double-clicked: {header}");
	});
}
*/
using CommunityToolkit.Mvvm.Input;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace CvnetClient.Behaviors {
	public class HeaderDoubleClickCommandAction : TriggerAction<DependencyObject> {
		public ICommand Command {
			get => (ICommand)GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(HeaderDoubleClickCommandAction));

		protected override void Invoke(object parameter) {
			if (parameter is not MouseButtonEventArgs e) return;
			if (e.OriginalSource is not DependencyObject source) return;

			// walk up the visual tree to find a DataGridColumnHeader
			while (source is not null && source is not DataGridColumnHeader)
				source = VisualTreeHelper.GetParent(source);

			if (source is not DataGridColumnHeader header) return;

			var headerName = header.Column?.Header?.ToString();
			var command = Command;

			if (command?.CanExecute(headerName) == true)
				command.Execute(headerName);
		}
	}
}
