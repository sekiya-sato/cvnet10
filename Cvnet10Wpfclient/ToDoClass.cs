namespace Cvnet10Wpfclient;

/*
 ToDo: 必要となる予定の画面
メニューに登録する画面
- マスタ系
- 伝票入力系
- 帳票印刷系
- 各種設定系
- 更新実行系
(上の作業がある程度完了したら)Test用ViewおよびViewModelの削除
				- WPF で DI コンテナ（Microsoft.Extensions.Hosting + Generic Host）を導入し、ViewModel/サービスの注入とテスト容易性を向上。
				- UI リソースは既に分割済み (UIColors.xaml 等) だが、テーマ切替やダークモード対応を早期に設計する。
				- コマンド/非同期処理は CancellationToken を用いてキャンセル可能にする。
					- 実施例: App.xaml.cs を IHost 起動に切替え、MainWindow の DataContext を DI で解決。


 */

internal class ToDoClass {
}
