using Cvnet10Base;

namespace Cvnet10DomainLogic;
/// <summary>
/// データベースを変換するクラス
/// </summary>
public class ConvertDb {
	ExDatabase _fromDb;
	ExDatabase _toDb;
	public ConvertDb(ExDatabase fromDb, ExDatabase toDb) {
		_fromDb = fromDb;
		_toDb = toDb;
	}
	#region 文字列変換サブロジック
	private string getString(Dictionary<string, object> rec, string key) {
		string? ret = String.Empty;
		if (rec.ContainsKey(key)) {
			ret = rec[key]?.ToString();
		}
		if (ret == "." || ret == null)
			ret = String.Empty;
		return ret;
	}
	// 新規：常に非 null を返すオーバーロード（デフォルト値を指定）
	private string getString(Dictionary<string, object> rec, string key, string defaultValue) {
		return getString(rec, key) ?? defaultValue;
	}

	private int getDataInt(Dictionary<string, object> rec, string key) {
		var data = getString(rec, key);
		if (data == null)
			return 0;
		if (int.TryParse(data, out int val) == false)
			return 0;

		return val;
	}
	#endregion
	/// <summary>
	/// 共通変換処理
	/// </summary>
	/// <typeparam name="T">対象テーブル型</typeparam>
	/// <param name="sql">元DBに対する取得SQL</param>
	/// <param name="isInit"></param>
	/// <param name="createItem"></param>
	/// <returns></returns>
	private int ConvertMaster<T>(string sql, bool isInit, Func<Dictionary<string, object>, T> createItem) {
		var rows = _fromDb.Fetch<Dictionary<string, object>>(sql);
		_toDb.CreateTable(typeof(T), isInit);

		if (rows.Count == 0)
			return 0;

		var list = new List<T>(rows.Count);
		foreach (var rec in rows) {
			list.Add(createItem(rec));
		}

		_toDb.BeginTransaction();
		_toDb.InsertBulk<T>(list);
		_toDb.CompleteTransaction();

		return rows.Count;
	}

	public void ConvertAll(bool isInit = true) {
		/*
		CnvMasterSys(isInit);
		CnvMasterMeisho(isInit);
		 */
	}

	/// <summary>
	/// システム管理マスタ変換 HC$master_syskanri HC$master_systax
	/// </summary>
	public int CnvMasterSys(bool isInit = true) {
		var mstSys = _fromDb.Fetch<Dictionary<string, object>>("select * from HC$master_syskanri");
		var recSys = mstSys[0];
		var mstTax = _fromDb.Fetch<Dictionary<string, object>>("select * from HC$master_systax order by 消費税CD");
		var newSys = new MasterSysman() {
			Name = getString(recSys, "自社名"),
			PostalCode = getString(recSys, "郵便番号"),
			Address1 = getString(recSys, "住所1"),
			Address2 = getString(recSys, "住所2"),
			Address3 = getString(recSys, "住所3"),
			Tel = getString(recSys, "TEL"),
			Mail = getString(recSys, "管理者MAIL"),
			Hp = getString(recSys, "ホームページ"),
			ShimeBi = getDataInt(recSys, "自社締日"),
			ModifyDaysEx = getDataInt(recSys, "修正有効日数"),
			ModifyDaysPre = getDataInt(recSys, "先付有効日数"),
			BankAccount1 = getString(recSys, "振込先1"),
			BankAccount2 = getString(recSys, "振込先2"),
			BankAccount3 = getString(recSys, "振込先3"),
			FiscalStartDate = getString(recSys, "期首年月日", "19010101"),
			Jsub = new List<MasterSysTax>(),
		};
		foreach (var rec in mstTax) {
			var tax = new MasterSysTax() {
				Id = getDataInt(rec, "消費税CD"),
				TaxRate = getDataInt(rec, "消費税率"),
				DateFrom = getString(rec, "新消費税開始日", "19010101"),
				TaxNewRate = getDataInt(rec, "新消費税率"),
			};
			newSys.Jsub.Add(tax);
		}
		_toDb.CreateTable(typeof(MasterSysman), isInit);
		_toDb.Insert<MasterSysman>(newSys);
		return 1;
	}
	/// <summary>
	/// 名称マスタ変換 HC$master_meisho
	/// </summary>
	public int CnvMasterMeisho(bool isInit = true) {
		const string sql = "select * from HC$master_meisho where 名称区分>'.' order by 名称区分,名称CD";// 名称区分 'IDX'
		return ConvertMaster(sql, isInit, rec => new MasterMeisho() {
			Kubun = getString(rec, "名称区分"),
			Code = getString(rec, "名称CD"),
			Name = getString(rec, "名称"),
			Ryaku = getString(rec, "略称"),
			Kana = getString(rec, "カナ"),
		});
	}
	/// <summary>
	/// 社員マスター変換
	/// </summary>
	/// <param name="isInit"></param>
	/// <returns></returns>
	public int CnvMasterShain(bool isInit = true) {
		const string sql = "select * from HC$master_shain where 社員CD>'.' order by 社員CD"; // 部門 'BMN' 社員分類 'E01'-'E10'
		return ConvertMaster(sql, isInit, rec => {
			var bumonMeisho = _toDb.FirstOrDefault<MasterMeisho>("where Kubun=@0 and Code=@1", ["BMN", getString(rec, "部門", ".")]);
			var item = new MasterShain() {
				Code = getString(rec, "社員CD"),
				Name = getString(rec, "名前"),
				Kana = getString(rec, "フリカナ"),
				Mail = getString(rec, "メール"),
				Code_Tenpo = getString(rec, "店舗CD"),
				Id_Bumon = bumonMeisho?.Id ?? 0,
				Code_Bumon = getString(rec, "部門CD"),
				Mei_Bumon = bumonMeisho?.Name??string.Empty,
			};
			var meiList = new List<MasterGeneralMeisho>();
			foreach (var i in Enumerable.Range(1, 5)) {
				var meisho = _toDb.FirstOrDefault<MasterMeisho>("where Kubun=@0 and Code=@1", [$"E0{i}", getString(rec, $"名称CD0{i}", ".")]);
				if (meisho != null) {
					meiList.Add(new MasterGeneralMeisho() {
						Kubun = meisho.Kubun,
						KubunName = meisho.KubunName,
						Code = meisho.Code,
						Name = meisho.Name,
					});
				}
			}
			if (meiList.Count > 0)
				item.Jsub = meiList;
			return item;
		}
		);
	}
	/// <summary>
	/// 顧客マスター変換
	/// </summary>
	/// <param name="isInit"></param>
	/// <returns></returns>
	public int CnvMasterEndCustomer(bool isInit = true) {
		const string sql = "select * from HC$master_kokyaku where 顧客CD>'.' order by 顧客CD"; // 顧客分類 'K01'-'K10'
		return 0;
	}
}
