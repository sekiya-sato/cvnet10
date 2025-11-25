using System;
using System.Collections.Generic;
using System.Text;

namespace Cvnet10Base {
	public class ConvertDb {
		ExDatabase _fromDb;
		ExDatabase _toDb;
		public ConvertDb(ExDatabase fromDb, ExDatabase toDb) {
			_fromDb = fromDb;
			_toDb = toDb;
		}
		public void CnvMasterSys() {
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
				Simebi = getDataInt(recSys,"自社締日"),
				ModifyDaysEx = getDataInt(recSys,"修正有効日数"),
				ModifyDaysPre = getDataInt(recSys,"先付有効日数"),
				BankAccount1 = getString(recSys, "振込先1"),
				BankAccount2 = getString(recSys, "振込先2"),
				BankAccount3 = getString(recSys, "振込先3"),
				FiscalStartDate = getString(recSys, "期首年月日"),
				Jsub = new List<MasterSysTax>(),
			};
			int cnt = 0;
			foreach(var rec in mstTax)	 {
				var tax = new MasterSysTax() {
					Id = getDataInt(rec, "消費税CD"),
					TaxRate = getDataInt(rec, "消費税率"),
					DateFrom = getString(rec, "新消費税開始日"),
					TaxNewRate = getDataInt(rec,"新消費税率"),
				};
				newSys.Jsub.Add(tax);
			}
			_toDb.CreateTable(typeof(MasterSysman));
			_toDb.Insert<MasterSysman>(newSys);


		}


		private string? getString(Dictionary<string, object> rec, string key) {
			string? ret = null;
			if (rec.ContainsKey(key)) {
				ret = rec[key]?.ToString();
			}
			if (ret == ".")
				ret = null;
			return ret;
		}
		private int getDataInt(Dictionary<string, object> rec, string key)  {
			var data = getString(rec, key);
			if(data == null)
				return 0;
			if(int.TryParse(data, out int val) == false)
				return 0;

			return val;
		}

	}
}
