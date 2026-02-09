using CommunityToolkit.Mvvm.ComponentModel;
using Cvnet10Base.Share;
using NPoco;

namespace Cvnet10Base;


/// <summary>
/// システム：システム管理テーブル
/// </summary>
[PrimaryKey("Id", AutoIncrement = true)]
public partial class MasterSysman : BaseDbHasAddress {
	/// <summary>
	/// 自社名
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(100)]
	string? name;
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
	int shimeBi;
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
	[property: ColumnSizeDml(8)]
	string fiscalStartDate="19010101";
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
[NoCreateTableJsub]
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
	[property: ColumnSizeDml(8)]
	string dateFrom = "19010101";
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
[KeyDml("MasterMeisho_uq1", false, ["Kubun","Code"])]
[KeyDml("MasterMeisho_nk2", false, ["Kubun","Odr" , "Code"])]
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
	[property: ColumnSizeDml(10)]
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
	/// <summary>
	/// よみがな
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(100)]
	string? kana;
	/// <summary>
	/// 並び順
	/// </summary>
	[ObservableProperty]
	[property: ColumnSizeDml(100)]
	int odr;
}

/// <summary>
/// 名称テーブルView
/// </summary>
[NoCreateTable]
public partial class MasterMeishoView : MasterMeisho, IBaseViewDefine {
	/// <summary>
	/// 区分名
	/// </summary>
	[ObservableProperty]
	string kubunName = "";
	readonly public static string ViewSql = """
SELECT * FROM (
    SELECT 
        T.*, 
        m1.Name AS KubunName
    FROM MasterMeisho T
    LEFT OUTER JOIN MasterMeisho m1 
        ON m1.Kubun = 'IDX' 
        AND T.Kubun = m1.Code
) MasterMeishoView
""";
}
