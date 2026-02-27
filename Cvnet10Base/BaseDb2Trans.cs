using CommunityToolkit.Mvvm.ComponentModel;
using NPoco;
using Cvnet10Base.Share;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Cvnet10Base;

/// <summary>
/// 共通トランザクション（ヘッダ）
/// </summary>
public partial class Tran99All : BaseDbClass {
	/// <summary>
	/// 計上日（yyyyMMdd）
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(8)]
	string denDay = "19010101";
	/// <summary>
	/// 区分（2桁 10-19,20-29,30,99）
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(8)]
	string kubun = "10";
	/// <summary>
	/// 社員ユニークキー
	/// </summary>
	[ObservableProperty]
	long id_Shain;
	/// <summary>
	/// 社員データ
	/// </summary>
	[ObservableProperty]
	[property: SerializedColumn]
	[property: ColumnSizeDml(100)]
	CodeNameView vShain = new();
	/// <summary>
	/// 倉庫キー
	/// </summary>
	[ObservableProperty]
	long id_Soko;
	/// <summary>
	/// 倉庫データ
	/// </summary>
	[ObservableProperty]
	[property: SerializedColumn]
	[property: ColumnSizeDml(100)]
	CodeNameView vSoko = new();
	/// <summary>
	/// 計算フラグ（1:+ -1:-, 0:計算除外）
	/// </summary>
	[ObservableProperty]
	int calcFlag = 1;
	/// <summary>
	/// 数量合計
	/// </summary>
	[ObservableProperty]
	int suTotal;
	/// <summary>
	/// 金額合計
	/// </summary>
	[ObservableProperty]
	int kingakuTotal;
	/// <summary>
	/// 上代合計
	/// </summary>
	[ObservableProperty]
	int jodaiTotal;
	/// <summary>
	/// 下代合計
	/// </summary>
	[ObservableProperty]
	int gedaiTotal;
	/// <summary>
	/// 値引: 合計からの
	/// </summary>
	[ObservableProperty]
	int nebiki00Total;
	/// <summary>
	/// 値引: 明細積上げ
	/// </summary>
	[ObservableProperty]
	int nebiki01Meisai;
	/// <summary>
	/// ヘッダメモ
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(200)]
	[property: System.ComponentModel.DefaultValue("")]
	string memo = string.Empty;
	/// <summary>
	/// 詳細内容
	/// </summary>
	[ObservableProperty]
	[property: SerializedColumn]
	[property: ColumnSizeDml(1000)]
	BaseDetailClass? jdetail;
	/// <summary>
	/// 明細リスト
	/// </summary>
	[ObservableProperty]
	[property: SerializedColumn]
	[property: ColumnSizeDml(4000)]
	List<Tran99Meisai>? jmeisai;
}

/// <summary>
/// 共通トランザクション（明細）
/// </summary>
public partial class Tran99Meisai : ObservableObject {
	/// <summary>
	/// 行No
	/// </summary>
	[ObservableProperty]
	int no;
	/// <summary>
	/// 区分（2桁 10-19,20-29,30,99）
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(8)]
	string kubun = "10";
	/// <summary>
	/// 商品ユニークキー
	/// </summary>
	[ObservableProperty]
	long id_Shohin;
	/// <summary>
	/// 商品CD
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(20)]
	[property: System.ComponentModel.DefaultValue("")]
	string code_Shohin = string.Empty;
	/// <summary>
	/// 商品名
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(100)]
	[property: System.ComponentModel.DefaultValue("")]
	string mei_Shohin = string.Empty;
	/// <summary>
	/// 入力JANコード
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(20)]
	[property: System.ComponentModel.DefaultValue("")]
	string janCode = string.Empty;
	/// <summary>
	/// 色
	/// </summary>
	[ObservableProperty]
	long id_Col;
	/// <summary>
	/// カラーCD
	/// </summary>
	[ObservableProperty]
	[property: System.ComponentModel.DefaultValue("")]
	string code_Col = string.Empty;
	/// <summary>
	/// カラー名
	/// </summary>
	[ObservableProperty]
	[property: System.ComponentModel.DefaultValue("")]
	string mei_Col = string.Empty;
	/// <summary>
	/// サイズ
	/// </summary>
	[ObservableProperty]
	long id_Siz;
	/// <summary>
	/// サイズCD
	/// </summary>
	[ObservableProperty]
	[property: System.ComponentModel.DefaultValue("")]
	string code_Siz = string.Empty;
	/// <summary>
	/// サイズ名
	/// </summary>
	[ObservableProperty]
	[property: System.ComponentModel.DefaultValue("")]
	string mei_Siz = string.Empty;
	/// <summary>
	/// 数量
	/// </summary>
	[ObservableProperty]
	int su;
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
	/// <summary>
	/// 上代
	/// </summary>
	[ObservableProperty]
	int jodai;
	/// <summary>
	/// 下代
	/// </summary>
	[ObservableProperty]
	int gedai;
	/// <summary>
	/// 値引: 合計からの
	/// </summary>
	[ObservableProperty]
	int nebiki00;
	/// <summary>
	/// 値引: 明細1
	/// </summary>
	[ObservableProperty]
	int nebiki01;
	/// <summary>
	/// 値引: 明細2
	/// </summary>
	[ObservableProperty]
	int nebiki02;
	/// <summary>
	/// 社員ユニークキー
	/// </summary>
	[ObservableProperty]
	long id_Shain;
	/// <summary>
	/// 社員CD
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(20)]
	[property: System.ComponentModel.DefaultValue("")]
	string code_Shain = string.Empty;
	/// <summary>
	/// 社員名
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(100)]
	[property: System.ComponentModel.DefaultValue("")]
	string mei_Shain = string.Empty;
	/// <summary>
	/// 明細メモ
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(200)]
	[property: System.ComponentModel.DefaultValue("")]
	string memo = string.Empty;
}
/// <summary>
/// 棚卸 60 (倉庫 現在値)
/// </summary>
[PrimaryKey("Id", AutoIncrement = true)]
[KeyDml("nk1", false, "DenDay")]
public partial class Tran60Tana : Tran99All {
}

/// <summary>
/// 本部売上 00 (倉庫 出)
/// </summary>
[PrimaryKey("Id", AutoIncrement = true)]
[KeyDml("nk1", false, "DenDay")]
[KeyDml("nk2", false, "KakeDay")]
public partial class Tran00Uriage: Tran99All {
	/// <summary>
	/// 掛計上日（yyyyMMdd）
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(8)]
	string kakeDay = "19010101";
	/// <summary>
	/// 得意先キー
	/// </summary>
	[ObservableProperty]
	long id_Tokui;
	/// <summary>
	/// 得意先CD
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(20)]
	[property: System.ComponentModel.DefaultValue("")]
	string code_Tokui = string.Empty;
	/// <summary>
	/// 得意先名
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(100)]
	[property: System.ComponentModel.DefaultValue("")]
	string mei_Tokui = string.Empty;
}
/// <summary>
/// 店舗売上 01 (倉庫 出)
/// </summary>
[PrimaryKey("Id", AutoIncrement = true)]
public partial class Tran01Tenuri : Tran99All {
	/// <summary>
	/// 店舗キー
	/// </summary>
	[ObservableProperty]
	long id_Tenpo;
	/// <summary>
	/// 店舗CD
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(20)]
	[property: System.ComponentModel.DefaultValue("")]
	string code_Tenpo = string.Empty;
	/// <summary>
	/// 店舗名
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(100)]
	[property: System.ComponentModel.DefaultValue("")]
	string mei_Tenpo = string.Empty;
	/// <summary>
	/// 顧客キー
	/// </summary>
	[ObservableProperty]
	int id_Customer;
	/// <summary>
	/// 顧客CD
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(20)]
	[property: System.ComponentModel.DefaultValue("")]
	string code_Customer = string.Empty;
	/// <summary>
	/// 顧客名
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(100)]
	[property: System.ComponentModel.DefaultValue("")]
	string mei_Customer = string.Empty;
}

/// <summary>
/// 仕入 03 (倉庫 入)
/// </summary>
[PrimaryKey("Id", AutoIncrement = true)]
public partial class Tran03Shiire : Tran99All {
	/// <summary>
	/// 仕入先キー
	/// </summary>
	[ObservableProperty]
	long id_Shiire;
	/// <summary>
	/// 仕入先CD
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(20)]
	[property: System.ComponentModel.DefaultValue("")]
	string code_Shiire = string.Empty;
	/// <summary>
	/// 仕入先名
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(100)]
	[property: System.ComponentModel.DefaultValue("")]
	string mei_Shiire = string.Empty;
}

/// <summary>
/// 移動 05 (倉庫 出)
/// </summary>
[PrimaryKey("Id", AutoIncrement = true)]
public partial class Tran05Ido : Tran99All {
	/// <summary>
	/// 移動先キー
	/// </summary>
	[ObservableProperty]
	long id_Ido;
	/// <summary>
	/// 移動先CD
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(20)]
	[property: System.ComponentModel.DefaultValue("")]
	string code_Ido = string.Empty;
	/// <summary>
	/// 移動先名
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(100)]
	[property: System.ComponentModel.DefaultValue("")]
	string mei_Ido = string.Empty;
}
