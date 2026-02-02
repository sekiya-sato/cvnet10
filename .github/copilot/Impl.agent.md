# Impl Agent Playbook

## 0. 目的
Cvnet10 プロジェクトにおける WPF クライアントおよび gRPC サーバ実装を、既存コードの命名・レイヤリング規約に沿って高速かつ安全に拡張するための実務指針を示す。常に日本語で説明し、`.slnx` 構成と既存層依存ルールを尊重すること。

## 1. 共通前提
- 開発対象は .NET 10 / C# 14。Primary Constructor、コレクション式、改良パターンマッチ等が利用可能。
- 通信は `protobuf-net.Grpc` コードファースト。スキーマは CodeShare で定義されるため **CodeShare / Cvnet10AppShared は読み取り専用**。
- 永続化は `NPoco` + `ExDatabase`。DB アクセスは `Cvnet10Base` → `Cvnet10DomainLogic` → `Cvnet10Server` の順で閉じ込める。
- JSON は必ず `Newtonsoft.Json`。`System.Text.Json` を追加しない。
- 例外は `ArgumentNullException.ThrowIfNull` など適切な型を使用し、握り潰さずログに残す。

## 2. レイヤリングと命名
| レイヤ | 役割 | 命名/配置ルール |
| --- | --- | --- |
| Layer0 (`CodeShare`, `Cvnet10AppShared`) | 契約/共通定義 | 変更禁止。型・列挙名は既存に追従 (`CvnetFlag`, `CvnetMsg` 等)。 |
| Layer1 (`Cvnet10Base`) | DB モデル | NPoco の `TableName`, `PrimaryKey` 属性を利用。PascalCase クラス、プロパティ名は DB 列の既存表記に倣う。 |
| Layer1.5 (`Cvnet10DomainLogic`) | ドメインサービス | `*Logic`, `*Service` 命名。同期 API でも内部 DB 呼び出しは `async/await` 化を検討。 |
| Layer2 (`Cvnet10Server`, `Cvnet10Wpfclient`) | gRPC, UI | サーバ側サービスは `*Service`, クライアント View は `*View.xaml`, ViewModel は `*ViewModel`。 |

新規ファイルも同階層の命名規約 (PascalCase, サフィックス) を踏襲する。

## 3. WPF クライアント実装指針 (`Cvnet10Wpfclient`)
### 3.1 MVVM + CommunityToolkit
- ViewModel は `ObservableObject`/`ObservableRecipient`。状態は `[ObservableProperty]`、コマンドは `[RelayCommand]` または `IAsyncRelayCommand` を使用。
- DI 登録済みサービスは `App.xaml.cs` または既存ブートストラップに沿って `IServiceProvider` から取得。
- コマンド名は `*Command`、戻り値 `Task` の非同期では `Async` サフィックスを必須 (`LoadAsync`).

### 3.2 XAML / View
- `Window`/`UserControl` には対応 ViewModel を `DataContext` として設定。リソース辞書や `Behaviors` を活用し、コードビハインドは初期化と UI 固有イベントの委譲のみ。
- バインドは `Mode=TwoWay` 明示 (必要な場合のみ)。`ItemsSource`, `SelectedItem` 等は ViewModel の ObservableCollection / プロパティに接続。
- UI テキストはリソース化を検討。`TestConnectButtonText` など既存命名パターン (PascalCase) に合わせる。

### 3.3 gRPC クライアント呼び出し
- クライアント生成は既存 `Grpc.Core` ラッパまたは `Grpc.Net.Client` の DI サービスを再利用。HttpClientFactory を通し、エンドポイントは設定ファイルから取得。
- すべてのネットワーク呼び出しは `async/await`、`CancellationToken` を ViewModel コマンドから受け渡し UI キャンセルを尊重。
- 例外は `Grpc.Core.RpcException` を個別に捕捉し、ユーザー通知 (`INotificationService` 等) + ログ出力。

### 3.4 テスト
- ViewModel ロジックは `Cvnet10Wpfclient.Tests` (存在する場合) に xUnit で追加。UI 依存を避け、`RelayCommand` の `CanExecute` や状態遷移を検証。

## 4. gRPC サーバ実装指針 (`Cvnet10Server`)
### 4.1 サービス構造
- `partial class XxxService : IXxxService` で実装し、依存は DI コンストラクタで受け取る。`ILogger<T>` と設定 (`IConfiguration`) を必須で受け、`ArgumentNullException.ThrowIfNull` を適用。
- ロジックは細分化 (`subLogicMsg700` のような private メソッド) し、Switch/Pattern Matching で `CvnetFlag` ごとに分岐。
- 公開メソッドは `Task<TResponse>` で `Async` サフィックス。

### 4.2 セキュリティ
- 実装完了後は `[Authorize]` が既定。テスト目的の `[AllowAnonymous]` は必ずコメント付きで残し、不要になり次第削除。
- JWT 設定 (`Microsoft.AspNetCore.Authentication.JwtBearer`) を前提に Claims を `_httpContextAccessor.HttpContext.User` から取得。権限チェックを行う。

### 4.3 DB / ドメイン呼び出し
- 直接 SQL を書かず、`Cvnet10DomainLogic` のサービスを介してドメイン操作。必要な場合のみ `ExDatabase` + NPoco を利用。
- 取引境界は `using var scope = await _db.BeginTransactionAsync(ct);` のように明示。`ConfigureAwait(false)` をライブラリ層で適用。

### 4.4 監視・ログ
- `ILogger` のスコープを使用し、`using var scope = _logger.BeginScope("MsgFlag:{Flag}", request.Flag);` のように文脈を付与。
- エラー時は `LogError(ex, "...")` でリクエスト識別子と Flag を記録し、クライアントには一般化したメッセージを返す。

### 4.5 テストと検証
- gRPC サービスの振る舞いは `Cvnet10Server.Tests` にて契約テストを行い、ドメインサービスは `Cvnet10DomainLogic.Tests` でカバー。
- テスト名は `WhenXxx_ShouldYyy` 形式。Arrange-Act-Assert を厳守。

## 5. 実装フロー
1. **層判定**: 実装対象が UI かサーバか判別し、許可レイヤのみ変更。
2. **計画**: 作業開始前に日本語で段取り (Plan) を提示。
3. **実装**: `apply_patch` で最小差分を適用し、複雑処理には簡潔なコメントで意図を残す。
4. **検証**: 変更ファイル単位で `get_errors`、完了時に `run_build` を実施。
5. **報告**: 影響範囲と次アクションを短くまとめて共有。

## 6. 品質チェックリスト
- [ ] 新規/更新 ViewModel で `async` 処理に `CancellationToken` を受け渡したか。
- [ ] サーバ側で `ILogger` スコープと適切な `try/catch` を入れたか。
- [ ] ユーザー向け文字列をハードコードしていないか (必要に応じてリソース化)。
- [ ] 共有ライブラリ (`CodeShare`, `Cvnet10AppShared`) を変更していないか。
- [ ] xUnit テストを追加/更新し `dotnet test` を実行したか。

この Playbook に従い、WPF クライアントと gRPC サーバの追加実装を既存規則に沿って行うこと。