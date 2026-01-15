// ファイル概要:
// - 取引先(得意先・仕入先)関連の共通テーブルモデルと詳細オブジェクトを定義します。
// - BaseDbHasAddress を継承し、取引条件や支払情報を保持します。
// 依存関係:
// - CommunityToolkit.Mvvm.ComponentModel と NPoco、Cvnet10Base の属性群。
// 変更ポリシー:
// - EnumTokui や ColumnSizeDml の値を変更すると移行や帳票に影響するため互換性を重視してください。
// - 支払条件項目を追加する際は MasterTokuiDetail / MasterShiireDetail と関連サービスの整合を確認します。
// COPILOT: 新しい取引先区分を追加する場合は EnumTokui と関連 UI バインディング、DB 初期データを必ず同期させること。

using CommunityToolkit.Mvvm.ComponentModel;
using NPoco;

namespace Cvnet10Base;

/// <summary>
/// 共通取引先テーブル
/// </summary>
public partial class MasterTorihiki : BaseDbHasAddress, IBaseCodeName {
	/// <summary>
	/// コード
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(12)]
	string code = "";
	/// <summary>
	/// 名前
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(80)]
	string? name;
	/// <summary>
	/// 略称
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(100)]
	string? ryaku;
	/// <summary>
	/// カナ
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(100)]
	string? kana;
	/// <summary>
	/// 担当者CD
	/// </summary>
	[ObservableProperty]
	long id_Shain;
	/// <summary>
	/// 掛率
	/// </summary>
	[ObservableProperty]
	int rateProper;
	/// <summary>
	/// セール掛率
	/// </summary>
	[ObservableProperty]
	int rateSale;
	/// <summary>
	/// 締日1
	/// </summary>
	[ObservableProperty]
	int shimeBi1;
	/// <summary>
	/// 締日2
	/// </summary>
	[ObservableProperty]
	int shimeBi2;
	/// <summary>
	/// 締日3
	/// </summary>
	[ObservableProperty]
	int shimeBi3;
	/// <summary>
	/// 入金/支払月
	/// </summary>
	[ObservableProperty]
	int payMonth;
	/// <summary>
	/// 入金/支払日
	/// </summary>
	[ObservableProperty]
	int payDay;
	/// <summary>
	/// 入金/支払方法
	/// </summary>
	[ObservableProperty]
	long id_MeishoPay;
	/// <summary>
	/// 請求/支払フラグ
	/// </summary>
	[ObservableProperty]
	int isPay = 0;
	/// <summary>
	/// 請求/支払先
	/// </summary>
	[ObservableProperty]
	long id_Paysaki;
}
/// <summary>
/// 得意先マスター
/// </summary>
public partial class MasterTokui : MasterTorihiki {
	/// <summary>
	/// 得意先種別
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(ColumnType.Enum)]
	EnumTokui tenType;
	/// <summary>
	/// 在庫管理フラグ
	/// </summary>
	[ObservableProperty]
	int isZaiko = 1;
	/// <summary>
	/// 名称リスト
	/// </summary>
	[ObservableProperty]
	[property: SerializedColumn]
	[property: ColumnSizeDml(1000)]
	List<MasterMeisho>? jsub;
	/// <summary>
	/// 得意先詳細
	/// </summary>
	[ObservableProperty]
	[property: SerializedColumn]
	[property: ColumnSizeDml(1000)]
	MasterTokuiDetail? jdetail;
}
/// <summary>
/// 得意先種別
/// </summary>
public enum EnumTokui {
	/// <summary>
	/// 倉庫
	/// </summary>
	Soko = 0,
	/// <summary>
	/// 卸先
	/// </summary>
	Oroshi = 1,
	/// <summary>
	/// 売仕店
	/// </summary>
	UriShi = 3,
	/// <summary>
	/// 直営店
	/// </summary>
	Tenpo = 6,
}
/// <summary>
/// 得意先詳細
/// </summary>
public partial class MasterTokuiDetail : ObservableObject {
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
}

/// <summary>
/// 仕入先マスター
/// </summary>
public partial class MasterShiire : MasterTorihiki {
	/// <summary>
	/// 名称リスト
	/// </summary>
	[ObservableProperty]
	[property: SerializedColumn]
	[property: ColumnSizeDml(1000)]
	List<MasterMeisho>? jsub;
	/// <summary>
	/// 仕入先詳細
	/// </summary>
	[ObservableProperty]
	[property: SerializedColumn]
	[property: ColumnSizeDml(1000)]
	MasterShiireDetail? jdetail;
}

/// <summary>
/// 仕入先詳細
/// </summary>
public partial class MasterShiireDetail : ObservableObject {
	/// <summary>
	/// 支払先1
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(30)]
	string? bankAccount1;
	/// <summary>
	/// 支払先2
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(30)]
	string? bankAccount2;
	/// <summary>
	/// 支払先3
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(30)]
	string? bankAccount3;
}


