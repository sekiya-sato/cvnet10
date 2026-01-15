## プロジェクトの目的

サーバ、クライアント間をgRPCで接続し、本格的なオープンソース販売管理パッケージとして公開することを目的としています。

## 目次

- [プロジェクトの目的](#プロジェクトの目的)
- [ソリューション概要](#ソリューション概要)
- [プロジェクト別概要](#プロジェクト別概要)
  - [CodeShare](#codeshare)
  - [Cvnet10Base](#cvnet10base)
  - [Cvnet10Server](#cvnet10server)
  - [Cvnet10Svtest](#cvnet10svtest)
  - [Cvnet10Wpftest](#cvnet10wpftest)
- [Index](#index)

## ソリューション概要

本ソリューションは、販売管理ドメインを gRPC ベースで分散実装するための統合環境です。
契約定義（CodeShare）、共通ドメインロジック（Cvnet10Base）、gRPC サーバ（Cvnet10Server）、動作検証用のコンソール/GUI クライアント（Cvnet10Svtest / Cvnet10Wpftest）が役割分担し、
.NET 10 / C# 14 の最新機能と ProtoBuf.Grpc / CommunityToolkit.Mvvm などの NuGet パッケージで構築されています。
サーバー-クライアント間の通信は共通 DTO と JSON/ProtoBuf シリアライズを併用し、将来的な OSS 販売管理パッケージ公開を見据えたスケーラブルなアーキテクチャになっています。

## プロジェクト別概要

### CodeShare
- gRPC 契約（サービス/メッセージ）と ProtoBuf.Grpc 共有インターフェースを定義。
- サーバーと各種クライアントの双方が参照し、完全な型安全通信を担保。
- 変更時はサーバー/クライアント両方の再ビルドが必要なため、契約の互換性管理が重要。

### Cvnet10Base
- 共通モデル、NPoco ベースの DB エンティティ、JSON シリアライズユーティリティ、ExDatabase などの汎用ロジックを提供。
- CommunityToolkit.Mvvm を活用した `ObservableObject` / `[ObservableProperty]` により、UI/サーバ双方で同一モデルを活用可能。
- 変換バッチ（`ConvertDb`）や DTO シリアライズ（`Common.SerializeObject`）など、全レイヤ共通の基盤を担う。

### Cvnet10Server
- gRPC サーバアプリ。`CvnetService` が `ICvnetService` を実装し、バージョン取得や環境変数提供などの API を公開。
- JSON シリアライズ設定 (`JsonSerializerSettings`) を共通化し、ProtoBuf.Grpc と組み合わせて双方向通信を実現。
- 今後は売上トランザクション等の業務ロジックをこの層に実装していく。

### Cvnet10Svtest
- gRPC クライアント動作検証用のコンソールアプリ。
- `GrpcChannel` をカスタム `HttpClient`（ヘッダ・タイムアウト設定）で初期化し、API 応答を `Common.DeserializeObject` で復元。
- サーバー機能追加時の手早い E2E 検証に利用。

### Cvnet10Wpftest
- CommunityToolkit.Mvvm を利用した WPF テストクライアント。
- `MainViewModel` が SQLite（`sample.db`）からマスタデータを読み込み、`MainWindow` で `MasterSysman` などを表示。
- 実装済みコマンド（`Test01` など）を拡張することで、GUI 上での業務フロー検証が可能。



