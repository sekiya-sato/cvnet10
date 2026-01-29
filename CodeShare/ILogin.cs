using ProtoBuf.Grpc;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace CodeShare;

/// <summary>
/// ログインリクエスト
/// </summary>
[DataContract]
public class LoginRequest {
	[DataMember(Order = 1)]
	public required string Name { get; set; }
	[DataMember(Order = 2)]
	public required string LoginId { get; set; }
	[DataMember(Order = 3)]
	public required string CryptPassword { get; set; }
	[DataMember(Order = 4)]
	public required DateTime LoginDate { get; set; }
	[DataMember(Order = 5)]
	public required string Info { get; set; }
}
/// <summary>
/// ログインリプライ
/// </summary>
[DataContract]
public class LoginReply {
	/// <summary>
	/// JSON Web Token Message
	/// </summary>
	[DataMember(Order = 1)]
	public required string JwtMessage { get; set; }
	/// <summary>
	/// 成功=0 失敗=-1
	/// </summary>
	[DataMember(Order = 2)]
	public int Result { get; set; }

	/// <summary>
	/// 新しいJWTの有効期限(LocalTime)
	/// [Expiration time of the new JWT (LocalTime)]
	/// </summary>
	[DataMember(Order = 3)]
	public DateTime Expire { get; set; }
}
/// <summary>
/// ログインリフレッシュ
/// </summary>
[DataContract]
public class LoginRefresh {
	/// <summary>
	/// 認証済みのトークン
	/// [Authenticated token]
	/// </summary>
	[DataMember(Order = 1)]
	public required string Token { get; set; }
	[DataMember(Order = 2)]
	public required string Info { get; set; }
}

/// <summary>
/// Contract:gRPC公開サービス
/// [Contract: gRPC Public Service]
/// </summary>
[ServiceContract]
public interface ILoginService {
	[OperationContract]
	Task<LoginReply> LoginAsync(LoginRequest UserRequest, CallContext context = default);

	[OperationContract]
	Task<LoginReply> LoginRefleshAsync(LoginRefresh UserRequest, CallContext context = default);
	[OperationContract]
	Task<LoginReply> CreateLoginAsync(LoginRequest UserRequest, CallContext context = default);
}
