using CommunityToolkit.Mvvm.ComponentModel;
using NPoco;
using Newtonsoft.Json;


namespace Cvnet10Base;

/// <summary>
/// ログイン管理テーブル
/// [Login management table]
/// </summary>
[PrimaryKey("Id", AutoIncrement = true)]
[Comment("システム：ログインID管理テーブル")]
[TableDml(NonUnieqeKey = "SysLogin_nk1=Id_Shain,SysLogin_nk2=Id_Role")]
public partial class SysLogin : BaseDbClass {
	/// <summary>
	/// 社員ユニークキー
	/// </summary>
	[ObservableProperty]
	long id_Shain;
	/// <summary>
	/// グループロールユニークキー
	/// </summary>
	[ObservableProperty]
	long id_Role;
	/// <summary>
	/// ログインID
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(120)]
	string? loginId;
	/// <summary>
	/// パスワード 暗号化by Vdc
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(120)]
	string? cryptPassword;
	/// <summary>
	/// 有効期限 yyyyMMddHHmmss
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(14)]
	string? expDate;
	/// <summary>
	/// 最終ログイン yyyyMMddHHmmss
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(14)]
	string? lastDate;
}
/// <summary>
/// ログイン履歴テーブル
/// [Login history table]
/// </summary>
[PrimaryKey("Id", AutoIncrement = true)]
[Comment("システム：ログイン履歴テーブル")]
[TableDml(NonUnieqeKey = "SysHistJwt_nk1=Id_Login@SysHistJwt_nk2=JwtUnixTime")]
public partial class SysHistJwt : BaseDbClass {
	/// <summary>
	/// ログインユニークキー
	/// </summary>
	[ObservableProperty]
	long id_Login;
	/// <summary>
	/// JwtのUnix有効期限
	/// </summary>
	[ObservableProperty]
	long jwtUnixTime;
	/// <summary>
	/// SysHistJwtSub JSON
	/// </summary>
	[ObservableProperty]
	[property: SerializedColumn]
	[property: ColumnSizeDml(1000)]
	SysHistJwtSub? jsub;
	/// <summary>
	/// 有効期限yyyyMMddHHmmss
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(14)]
	string? expDate;
	/// <summary>
	/// IPアドレス
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(20)]
	string? ip;
	/// <summary>
	/// サービスオペレーション
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(20)]
	string? op;
}
/// <summary>
/// ログイン履歴サブテーブル Jsubプロパティ用
/// </summary>
public partial class SysHistJwtSub : ObservableObject {
	[ObservableProperty]
	string? machine;
	[ObservableProperty]
	string? user;
	[ObservableProperty]
	string? osVer;
	/// <summary>
	/// IPアドレス
	/// </summary>
	[ObservableProperty]
	[property: JsonProperty("IP")]
	string? ipAddress;
	/// <summary>
	/// MACアドレス
	/// </summary>
	[ObservableProperty]
	[property: JsonProperty("MacA")]
	string? macAddress;
}

