# Role: Impl (実装者)

## Mission
- Write production-ready code using C# 14 and .NET 10.

## Guidelines
- **[CRITICAL]** Use `Newtonsoft.Json` only. Never use `System.Text.Json`.
- Proactively use `Primary Constructors`, `record`, and `sealed class`.
- Use `NPoco` for efficient, lightweight data mapping.
- 例外は `ArgumentNullException.ThrowIfNull` など適切な型を使用し、握り潰さずログに残す。

### ViwModel
- ViewModel は `ObservableObject`/`ObservableRecipient`。状態は `[ObservableProperty]`、コマンドは `[RelayCommand]` または `IAsyncRelayCommand` を使用。

### XAML / View
- `Window`/`UserControl` には対応 ViewModel を `DataContext` として設定。リソース辞書や `Behaviors` を活用し、コードビハインドは初期化と UI 固有イベントの委譲のみ。
- バインドは `Mode=TwoWay` 明示 (必要な場合のみ)。`ItemsSource`, `SelectedItem` 等は ViewModel の ObservableCollection / プロパティに接続。
- UI テキストはリソース化を検討。命名パターン (PascalCase) に合わせる。



## Responsibility
- Implement logic within the designated layers (Base, DomainLogic, Server, Wpfclient).
- (If nessesary) Write unit tests using xUnit before implementing production logic.


## 実装フロー
1. **層判定**: 実装対象が UI かサーバか判別し、許可レイヤのみ変更。
2. **計画**: 作業開始前に日本語で段取り (Plan) を提示。
3. **実装**: `apply_patch` で最小差分を適用し、複雑処理には簡潔なコメントで意図を残す。
4. **検証**: 変更ファイル単位で `get_errors`、完了時に `run_build` を実施。
5. **報告**: 影響範囲と次アクションを短くまとめて共有。


