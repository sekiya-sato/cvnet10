using CommunityToolkit.Mvvm.ComponentModel;
using NPoco;
using System.Text.Json.Serialization;

namespace Cvnet10Base;

/// <summary>
/// 基底データClass(Id,Vdc,Vdu) Id_??=joinキー,Fg_=0/1フラグ,En_=enum値,Disp0=表示用
/// </summary>
public partial class BaseDbClass : ObservableObject {
	/// <summary>
	/// ユニークキー
	/// </summary>
	[ObservableProperty]
	[property: Comment("ユニークキー")]
	long id;
	/// <summary>
	/// 作成日UTC.Ticks
	/// </summary>
	[ObservableProperty]
	[property: Comment("V作成日UTC.Ticks")]
	long vdc = DateTime.Now.ToUniversalTime().Ticks;
	/// <summary>
	/// 修正日UTC.Ticks
	/// </summary>
	[ObservableProperty]
	[property: Comment("V修正日UTC.Ticks")]
	long vdu = DateTime.Now.ToUniversalTime().Ticks;
	/// <summary>
	/// 作成日(参照のみ)書式 yyyy/MM/dd HH:mm:ss.ffff DateTime(Vdc).ToLocalTime)
	/// </summary>
	[Ignore]
	[JsonIgnore]
	public DateTime VdateC {
		get => new DateTime(Vdc).ToLocalTime();
	}
	/// <summary>
	/// 修正日(参照のみ)書式 yyyy/MM/dd HH:mm:ss.ffff DateTime(Vdu).ToLocalTime
	/// </summary>
	[Ignore]
	[JsonIgnore]
	public DateTime VdateU {
		get => new DateTime(Vdu).ToLocalTime();
	}
	/// <summary>
	/// 表示専用項目
	/// </summary>
	[ObservableProperty]
	[property: ResultColumn]
	string? disp0;
}

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
/// システム：システム管理テーブル
/// </summary>
[PrimaryKey("Id", AutoIncrement = true)]
public partial class MasterSysman : BaseDbClass {
	/// <summary>
	/// 自社名
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(100)]
	string? name;
	/// <summary>
	/// 郵便番号
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(30)]
	string? postalCode;
	/// <summary>
	/// 住所1
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(60)]
	string? address1;
	/// <summary>
	/// 住所2
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(60)]
	string? address2;
	/// <summary>
	/// 住所3
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(60)]
	string? address3;
	/// <summary>
	/// 電話番号
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(20)]
	string? tel;
	/// <summary>
	/// メールアドレス
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(30)]
	string? mail;
	/// <summary>
	/// ホームページ
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(30)]
	string? hp;
	/// <summary>
	/// 自社締め日 1-31,99
	/// </summary>
	[ObservableProperty]
	int simebi;
	/// <summary>
	/// 修正有効日数
	/// </summary>
	[ObservableProperty]
	int modifyDaysEx;
	/// <summary>
	/// 先付有効日数
	/// </summary>
	[ObservableProperty]
	int modifyDaysPre;
	/// <summary>
	/// 振込先1
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(30)]
	string? bankAccount1;
	/// <summary>
	/// 振込先2
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(30)]
	string? bankAccount2;
	/// <summary>
	/// 振込先3
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(30)]
	string? bankAccount3;
	/// <summary>
	/// 期首年月日
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(14)]
	string? fiscalStartDate;
	/// <summary>
	/// 消費税率リスト
	/// </summary>
	[ObservableProperty]
	[property: SerializedColumn]
	[property: ColumnSizeDml(100)]
	List<MasterSysTax>? jsub;
}
/// <summary>
/// 消費税率テーブル
/// </summary>
public partial class MasterSysTax: ObservableObject {
	[ObservableProperty]
	long id;
	/// <summary>
	/// 消費税率 (%) 例:10
	/// </summary>
	[ObservableProperty]
	int taxRate;
	/// <summary>
	/// 新消費税開始日(yyyyMMdd)
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(14)]
	string? dateFrom;
	/// <summary>
	/// 新消費税率 (%) 例:10
	/// </summary>
	[ObservableProperty]
	int taxNewRate;
}

/// <summary>
/// 名称テーブル
/// </summary>
[PrimaryKey("Id", AutoIncrement = true)]
public partial class MasterMeisho : BaseDbClass {
	/// <summary>
	/// 区分
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(8)]
	string kubun = "";
	/// <summary>
	/// 名称コード
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(8)]
	string code = "";
	/// <summary>
	/// 名称
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(100)]
	string? name;
	/// <summary>
	/// 略称
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(100)]
	string? ryaku;
}

/// <summary>
/// 顧客テーブル
/// </summary>
[PrimaryKey("Id", AutoIncrement = true)]
public partial class MasterEndCustomer : BaseDbClass {
	/// <summary>
	/// 顧客名
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(40)]
	string? name;
	/// <summary>
	/// 顧客カナ
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(40)]
	string? kana;
	/// <summary>
	/// 郵便番号
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(30)]
	string? postalCode;
	/// <summary>
	/// 住所1
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(60)]
	string? address1;
	/// <summary>
	/// 住所2
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(60)]
	string? address2;
	/// <summary>
	/// 住所3
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(60)]
	string? address3;
	/// <summary>
	/// 電話番号
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(20)]
	string? tel;
	/// <summary>
	/// メールアドレス
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(120)]
	string? mail;
	/// <summary>
	/// ランク
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(8)]
	string? rank;



}


