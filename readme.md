## プロジェクトの目的

サーバ、クライアント間をgRPCで接続し、本格的なオープンソース販売管理パッケージとして公開することを目的としています。


## Index 

- [プロジェクト構成](#P01)
- [プロジェクト進捗](#P02)
- [雑アイディア](#P10idia)
- [CVのメニュー構成](#P19menu)
- [DBテーブル定義](#P20table)
- [使用しているNUGETパッケージ](#Pnuget)
- [その他のコンセプト](#P99etc)



## <a id="P01">プロジェクト構成</a>

- CodeShare	      --- gRPC用Contract定義(server client共通)
- Cvnet10base     --- 共通Classおよび汎用処理(server client共通)
- Cvnet10Server   --- gRPCサーバ(server)
- Cvnet10Svtest   --- gRPCクライアントテスト(client)
- Cvnet10Wpftest  --- WPFクライアントテスト(client)


