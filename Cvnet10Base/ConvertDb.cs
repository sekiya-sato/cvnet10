// ファイル概要:
// - 旧 DB から新 DB へデータを移送する ConvertDb ユーティリティを実装します。
// - fetch/insert を行う ExDatabase インスタンスをラップし、マスター群の移行ロジックを保持します。
// 依存関係:
// - Cvnet10Base.ExDatabase およびマスターモデル、標準 System API。
// 変更ポリシー:
// - 変換対象テーブルを増やす際は ConvertAll と個別変換メソッドにトランザクションや CreateTable を忘れないでください。
// - 旧 DB の列名を参照するキー文字列を変更する前に、元システムのスキーマとマッピング表を更新します。
// COPILOT: 変換処理を追加する場合はリトライ/ログ記録の必要性を検討し、例外時に部分コミットが残らないようトランザクション設計を行うこと。

using System;
using System.Collections.Generic;
using System.Text;

namespace Cvnet10Base {
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
		private string? getString(Dictionary<string, object> rec, string key) {
			string? ret = null;
			if (rec.ContainsKey(key)) {
				ret = rec[key]?.ToString();
			}
			if (ret == ".")
				ret = null;
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

		public void ConvertAll(bool isInit=true) {
			CnvMasterSys(isInit);
			CnvMasterMeisho(isInit);
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
				ShimeBi = getDataInt(recSys,"自社締日"),
				ModifyDaysEx = getDataInt(recSys,"修正有効日数"),
				ModifyDaysPre = getDataInt(recSys,"先付有効日数"),
				BankAccount1 = getString(recSys, "振込先1"),
				BankAccount2 = getString(recSys, "振込先2"),
				BankAccount3 = getString(recSys, "振込先3"),
				FiscalStartDate = getString(recSys, "期首年月日", "19010101"),
				Jsub = new List<MasterSysTax>(),
			};
			foreach(var rec in mstTax)	 {
				var tax = new MasterSysTax() {
					Id = getDataInt(rec, "消費税CD"),
					TaxRate = getDataInt(rec, "消費税率"),
					DateFrom = getString(rec, "新消費税開始日", "19010101"),
					TaxNewRate = getDataInt(rec,"新消費税率"),
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
			var mstMeisho = _fromDb.Fetch<Dictionary<string, object>>("select * from HC$master_meisho where 名称区分>'.' order by 名称区分,名称CD");
			_toDb.CreateTable(typeof(MasterMeisho), isInit);
			if (mstMeisho.Count > 0) {
				var meishoList = new List<MasterMeisho>();
				foreach (var rec in mstMeisho) {
					var meisho = new MasterMeisho() {
						Kubun = getString(rec, "名称区分", "."),
						Code = getString(rec, "名称CD","."),
						Name = getString(rec, "名称"),
						Ryaku = getString(rec, "略称"),
						Kana = getString(rec, "カナ"),
					};
					meishoList.Add(meisho);
				}
				_toDb.BeginTransaction();
				_toDb.InsertBulk<MasterMeisho>(meishoList);
				_toDb.CompleteTransaction();
			}
			return mstMeisho.Count;
		}

	}
}
