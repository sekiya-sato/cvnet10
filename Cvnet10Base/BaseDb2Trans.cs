
using CommunityToolkit.Mvvm.ComponentModel;
using NPoco;

namespace Cvnet10Base;

/// <summary>
/// 売上トランザクション（ヘッダ）
/// </summary>
[PrimaryKey("Id", AutoIncrement = true)]
public partial class Tran0Uriage : BaseDbClass {
	/// <summary>
	/// 売上日（yyyyMMdd）
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(8)]
	string dayUriage = "19010101";

	/// <summary>
	/// 掛計上日（yyyyMMdd）
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(8)]
	string dayKake = "19010101";

	/// <summary>
	/// 得意先キー
	/// </summary>
	[ObservableProperty]
	long id_Tokui;

	/// <summary>
	/// 倉庫キー
	/// </summary>
	[ObservableProperty]
	long id_Soko;

	/// <summary>
	/// 入力社員キー
	/// </summary>
	[ObservableProperty]
	long id_ShainInput;

	/// <summary>
	/// 明細リスト
	/// </summary>
	[ObservableProperty]
	[property: SerializedColumn]
	[property: ColumnSizeDml(4000)]
	List<Tran0UriageMeisai>? jmeisai;
}

/// <summary>
/// 売上トランザクション（明細）
/// </summary>
public partial class Tran0UriageMeisai : ObservableObject {
	/// <summary>
	/// 行No
	/// </summary>
	[ObservableProperty]
	int no;

	/// <summary>
	/// 商品CD
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(12)]
	string? codeShohin;

	/// <summary>
	/// JANコード
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(13)]
	string? janCode;

	/// <summary>
	/// 数量
	/// </summary>
	[ObservableProperty]
	int suryo;

	/// <summary>
	/// 単価
	/// </summary>
	[ObservableProperty]
	int tanka;

	/// <summary>
	/// 金額
	/// </summary>
	[ObservableProperty]
	int kingaku;
}