// ファイル概要:
// - Cvnet10 サーバーとクライアント間で共有する gRPC 契約です。
// - ProtoBuf 互換 DTO (HelloRequest/HelloReply) と IGreeter サービス契約を含みます。
// - このファイルに記述されているDateContractおよびServiceContractは主にgRPCの動作テスト用に定義されています
// 依存関係:
// - HTTP/2 上でクロスプラットフォーム gRPC を実現するための ProtoBuf.Grpc と System.ServiceModel 属性。
// 変更ポリシー:
// - デプロイ済みクライアントとのバイナリ互換性を保つため DataMember の Order 値は固定してください。
// - 新しい操作を追加する際は IGreeter に定義し、Cvnet10Server.Services.GreeterService に実装してください。
// COPILOT: このファイルを拡張する際はイミュータブル DTO を優先し、クライアント側 JSON サンプルも更新すること。

using ProtoBuf.Grpc;
using System.Runtime.Serialization;
using System.ServiceModel;


namespace CodeShare;

[DataContract]
public class HelloRequest {
	[DataMember(Order = 1)]
	public string Name { get; set; } = "";
}
[DataContract]
public class HelloReply {
	[DataMember(Order = 1)]
	public string Message { get; set; } = "";

	[DataMember(Order = 2)]
	public Type Type0 { get; set; } = typeof(string);

	[DataMember(Order = 3)]
	public string JsonMsg { get; set; } ="";
}

[ServiceContract]
public interface IGreeter {
	[OperationContract]
	Task<HelloReply> SayHello(HelloRequest request, CallContext context = default);
}
