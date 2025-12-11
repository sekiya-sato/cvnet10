using CommunityToolkit.Mvvm.ComponentModel;
using NPoco;

namespace Cvnet10Base;

/// <summary>
/// 得意先テーブル
/// </summary>
public partial class MasterTokui : BaseDbHasAddress {
	/// <summary>
	/// コード
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(12)]
	string? code;
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
	/// 得意先種別
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(ColumnType.Enum)]
	EnumTokui tenType;
	/// <summary>
	/// 掛率
	/// </summary>
	[ObservableProperty]
	int rateProper;
	[ObservableProperty]
	int rateSale;




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






public partial class MasterShain : BaseDbClass {
	[ObservableProperty]
	[property: ColumnSizeDml(12)]
	string? code;
	[ObservableProperty]
	[property: ColumnSizeDml(40)]
	string? name;

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
	/// <summary>
	/// 店舗キー
	/// </summary>
	[ObservableProperty]
	long id_Tenpo;
	/// <summary>
	/// 誕生日 yyyyMMdd
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(8)]
	string? birthday;
	/// <summary>
	/// 誕生日 MMdd
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(4)]
	string? birthNoyear;
	/// <summary>
	/// メモ
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(120)]
	string? memo;
	/// <summary>
	/// 性別 0=不明 1=男性 2=女性
	/// </summary>
	[ObservableProperty]
	int gendar;
	/// <summary>
	/// ポイント
	/// </summary>
	[ObservableProperty]
	int point;
	/// <summary>
	/// 累計購買回数
	/// </summary>
	[ObservableProperty]
	int salesCount;
	/// <summary>
	/// 累計購買金額
	/// </summary>
	[ObservableProperty]
	int salesKingaku;

	/// <summary>
	/// 名称リスト
	/// </summary>
	[ObservableProperty]
	[property: SerializedColumn]
	[property: ColumnSizeDml(100)]
	List<MasterMeisho>? jsub;


}

