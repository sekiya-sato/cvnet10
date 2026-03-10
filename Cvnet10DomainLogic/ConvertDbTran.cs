using Cvnet10Base;
using Cvnet10Base.Share;

namespace Cvnet10DomainLogic;

public partial class ConvertDb {
	/// <summary>
	/// 本部売上変換
	/// </summary>
	public int CnvTran00Uri(bool isInit = true) {
		var tranHeader = _fromDb.Fetch<Dictionary<string, object>>("select * from HC$tran_tori0 where 伝票処理区分=0 order by SEQ_NO");
		_toDb.CreateTable(typeof(Tran00Uriage), isInit);

		if (tranHeader.Count == 0)
			return 0;

		Dictionary<string, MasterShain?> shainCache = [];
		Dictionary<string, MasterTokui?> tokuiCache = [];
		Dictionary<string, MasterShohin?> shohinCache = [];
		Dictionary<string, MasterMeisho?> meishoCache = [];

		List<Tran00Uriage> list = new(tranHeader.Count);
		foreach (var rec in tranHeader) {
			var shain = getCodeNameView<MasterShain>(getString(rec, "入力社員CD"), out var id_shain) ?? new();
			var soko = getCodeNameView<MasterTokui>(getString(rec, "倉庫CD"), out var id_soko) ?? new();
			var tokui = getCodeNameView<MasterTokui>(getString(rec, "取引先CD1"), out var id_tokui) ?? new();

			var item = new Tran00Uriage() {
				DenDay = getString(rec, "在庫計上日", "19010101"),
				KakeDay = getString(rec, "掛計上日", "19010101"),
				Kubun = getDataInt(rec, "取引区分"),
				CalcFlag = getCalcFlag(getDataInt(rec, "取引区分")),
				ManualNo = getString(rec, "手入力伝票NO"),
				RelateNo1 = getDataInt(rec, "関連伝票NO"),
				RelateNo2 = getDataInt(rec, "関連伝票NO2"),
				SuTotal = getDataInt(rec, "数量合計"),
				KingakuTotal = getDataInt(rec, "明細金額合計"),
				JodaiTotal = getDataInt(rec, "上代合計"),
				GedaiTotal = getDataInt(rec, "下代合計"),
				Nebiki00Total = getDataInt(rec, "値引1") + getDataInt(rec, "値引2") + getDataInt(rec, "値引3"),
				Nebiki01Meisai = 0,
				Memo = getString(rec, "メモ"),
				Jdetail = new BaseDetailClass() {
					Yobi1 = getString(rec, "取引先CD2"),
					Yobi2 = getString(rec, "顧客TEL"),
				},
				IsPay = getDataInt(rec, "掛計上FLG"),
				Id_Shain = id_shain,
				VShain = shain,
				Id_Soko = id_soko,
				VSoko = soko,
				Id_Tokui = id_tokui,
				VTokui = tokui,
			};
			var detailRows = _fromDb.Fetch<Dictionary<string, object>>("select * from HC$tran_tori1 where ヘッダNO=@0 order by 行NO", getDataInt(rec, "SEQ_NO"));

			List<Tran99Meisai>? meisaiList = null;
			if (detailRows.Count > 0) {
				meisaiList = new List<Tran99Meisai>(detailRows.Count);
				foreach (var detailRec in detailRows) {
					var shohin = getMaster<MasterShohin>(getString(detailRec, "商品CD"));
					var col = getMaster<MasterMeisho>(getString(detailRec, "色CD"));
					var siz = getMaster<MasterMeisho>(getString(detailRec, "サイズCD"));

					meisaiList.Add(new Tran99Meisai() {
						No = getDataInt(detailRec, "行NO"),
						Kubun = getDataInt(detailRec, "明細取引区分"),
						Id_Shohin = shohin?.Id ?? 0,
						Code_Shohin = shohin?.Code ?? string.Empty,
						Mei_Shohin = shohin?.Name ?? string.Empty,
						JanCode = getString(detailRec, "JANCODE"),
						Id_Col = col?.Id ?? 0,
						Code_Col = col?.Code ?? string.Empty,
						Mei_Col = col?.Name ?? string.Empty,
						Id_Siz = siz?.Id ?? 0,
						Code_Siz = siz?.Code ?? string.Empty,
						Mei_Siz = siz?.Name ?? string.Empty,
						Su = getDataInt(detailRec, "数量"),
						Tanka = getDataInt(detailRec, "単価"),
						Kingaku = getDataInt(detailRec, "金額"),
						Jodai = getDataInt(detailRec, "上代金額"),
						Gedai = getDataInt(detailRec, "下代金額"),
						Nebiki00 = getDataInt(detailRec, "明細値引"),
						Nebiki01 = getDataInt(detailRec, "明細値引1"),
						Nebiki02 = getDataInt(detailRec, "小計値引") + getDataInt(detailRec, "小計値引1"),
						Memo = getString(detailRec, "明細メモ"),
					});
				}
			}

			list.Add(item);
		}
		_toDb.BeginTransaction();
		_toDb.InsertBulk<Tran00Uriage>(list);
		_toDb.CompleteTransaction();
		return list.Count;
	}
	/// <summary>
	/// 店舗売上変換
	/// </summary>
	public int CnvTran01Uri(bool isInit = true) {
		var tranHeader = _fromDb.Fetch<Dictionary<string, object>>("select * from HC$tran_tori0 where 伝票処理区分=1 order by SEQ_NO");
		_toDb.CreateTable(typeof(Tran01Tenuri), isInit);

		if (tranHeader.Count == 0)
			return 0;

		Dictionary<string, MasterShain?> shainCache = [];
		Dictionary<string, MasterTokui?> tokuiCache = [];
		Dictionary<string, MasterShohin?> shohinCache = [];
		Dictionary<string, MasterMeisho?> meishoCache = [];

		List<Tran01Tenuri> list = new(tranHeader.Count);
		foreach (var rec in tranHeader) {
			var shain = getCodeNameView<MasterShain>(getString(rec, "入力社員CD"), out var id_shain) ?? new();
			var soko = getCodeNameView<MasterTokui>(getString(rec, "倉庫CD"), out var id_soko) ?? new();
			var tokui = getCodeNameView<MasterTokui>(getString(rec, "取引先CD1"), out var id_tokui) ?? new();
			var kokyaku = getCodeNameView<MasterEndCustomer>(getString(rec, "顧客TEL"), out var id_kokyaku) ?? new();

			var item = new Tran01Tenuri() {
				DenDay = getString(rec, "在庫計上日", "19010101"),
				Kubun = getDataInt(rec, "取引区分"),
				CalcFlag = getCalcFlag(getDataInt(rec, "取引区分")),
				RelateNo1 = getDataInt(rec, "関連伝票NO"),
				SuTotal = getDataInt(rec, "数量合計"),
				KingakuTotal = getDataInt(rec, "明細金額合計"),
				JodaiTotal = getDataInt(rec, "上代合計"),
				GedaiTotal = getDataInt(rec, "下代合計"),
				Nebiki00Total = getDataInt(rec, "値引1") + getDataInt(rec, "値引2") + getDataInt(rec, "値引3"),
				Nebiki01Meisai = 0,
				Memo = getString(rec, "メモ"),
				Jdetail = new BaseDetailClass() {
					Yobi1 = getString(rec, "手入力伝票No"),
					Yobi2 = getString(rec, "関連伝票NO2"),
				},
				Id_Shain = id_shain,
				VShain = shain,
				Id_Soko = id_soko,
				VSoko = soko,
				Id_Tenpo = id_tokui,
				VTenpo = tokui,
				Id_Customer = id_kokyaku,
				VCustomer = kokyaku,
				Code_Customer = getString(rec, "顧客TEL"),
			};
			var detailRows = _fromDb.Fetch<Dictionary<string, object>>("select * from HC$tran_tori1 where ヘッダNO=@0 order by 行NO", getDataInt(rec, "SEQ_NO"));

			List<Tran99Meisai>? meisaiList = null;
			if (detailRows.Count > 0) {
				meisaiList = new List<Tran99Meisai>(detailRows.Count);
				foreach (var detailRec in detailRows) {
					var shohin = getMaster<MasterShohin>(getString(detailRec, "商品CD"));
					var col = getMaster<MasterMeisho>(getString(detailRec, "色CD"));
					var siz = getMaster<MasterMeisho>(getString(detailRec, "サイズCD"));

					meisaiList.Add(new Tran99Meisai() {
						No = getDataInt(detailRec, "行NO"),
						Kubun = getDataInt(detailRec, "明細取引区分"),
						Id_Shohin = shohin?.Id ?? 0,
						Code_Shohin = shohin?.Code ?? string.Empty,
						Mei_Shohin = shohin?.Name ?? string.Empty,
						JanCode = getString(detailRec, "JANCODE"),
						Id_Col = col?.Id ?? 0,
						Code_Col = col?.Code ?? string.Empty,
						Mei_Col = col?.Name ?? string.Empty,
						Id_Siz = siz?.Id ?? 0,
						Code_Siz = siz?.Code ?? string.Empty,
						Mei_Siz = siz?.Name ?? string.Empty,
						Su = getDataInt(detailRec, "数量"),
						Tanka = getDataInt(detailRec, "単価"),
						Kingaku = getDataInt(detailRec, "金額"),
						Jodai = getDataInt(detailRec, "上代金額"),
						Gedai = getDataInt(detailRec, "下代金額"),
						Nebiki00 = getDataInt(detailRec, "明細値引"),
						Nebiki01 = getDataInt(detailRec, "明細値引1"),
						Nebiki02 = getDataInt(detailRec, "小計値引") + getDataInt(detailRec, "小計値引1"),
						Memo = getString(detailRec, "明細メモ"),
					});
				}
			}

			list.Add(item);
		}
		_toDb.BeginTransaction();
		_toDb.InsertBulk<Tran01Tenuri>(list);
		_toDb.CompleteTransaction();
		return list.Count;
	}







	/*
	 * HC$tran_tori0 
	 * HC$tran_tori1 は "select * from HC$tran_tori1 where ヘッダNo={tranHeaderのSEQ_NO}" で読み込む。

		 * 伝票処理区分=0 のものを読み込む。
		 * 伝票処理区分=0 は売上伝票。
		 * Tran00Uriage は売上伝票のドメインモデル。
		 * Tran00Uriage のプロパティは、HC$tran_tori0 のカラムに対応する。
		 * 例えば、SEQ_NO は Tran00Uriage の SeqNo プロパティに対応する。
		 * 例えば、VDATE_CREATE は Tran00Uriage の VDateCreate プロパティに対応する。
		 * 例えば、VDATE_UPDATE は Tran00Uriage の VDateUpdate プロパティに対応する。
		 * 例えば、在庫計上日 は Tran00Uriage の ZaikoKeijoBi プロパティに対応する。
		 * 例えば、掛計上日 は Tran00Uriage の KakeKeijoBi プロパティに対応する。
		 * 例えば、納品日 は Tran00Uriage の NouhinBi プロパティに対応する。
		 * 例えば、取引区分 は Tran00Uriage の TorihikiKubun プロパティに対応する。
		 * 例えば、入力社員区分 は Tran00Uriage の NyuuryokuShainKubun プロパティに対応する。
		 * 例えば、入力社員CD は Tran00Uriage の NyuuryokuShainCd プロパティに対応する。
		 * 例えば、倉庫CD は Tran00Uriage の SoukoCd プロパティに対応する。
		 * 例えば、取引先CD1 は Tran00Uriage の TorihikisakiCd1 プロパティに対応する。
		 * 例えば、取引先CD2 は Tran00Uriage の TorihikisakiCd2 プロパティに対応する。
		 * 例えば、顧客TEL は Tran00Uriage の KokyakuTel プロパティに対応する。
		 * 例えば、掛率1 は Tran00Uriage の Kakeritsu1 プロパティに対応する。
		 * 例えば、掛率2 は Tran00Uriage の Kakeritsu2 プロパティに対応する。
	 * 
	 * 
	 * 
desc HC$tran_tori0

名前                               NULL?    型
-------------------------------- -------- ----------------------------
SEQ_NO                           NOT NULL NUMBER(14)
VDATE_CREATE                     NOT NULL NUMBER(14,8)
VDATE_UPDATE                     NOT NULL NUMBER(14,8)
伝票処理区分                           NOT NULL NUMBER(2)
在庫計上日                            NOT NULL VARCHAR2(8)
掛計上日                             NOT NULL VARCHAR2(8)
納品日                              NOT NULL VARCHAR2(8)
取引区分                             NOT NULL NUMBER(2)
入力社員区分                           NOT NULL NUMBER(2)
入力社員CD                           NOT NULL VARCHAR2(20)
倉庫CD                             NOT NULL VARCHAR2(8)
取引先CD1                           NOT NULL VARCHAR2(8)
取引先CD2                           NOT NULL VARCHAR2(8)
顧客TEL                            NOT NULL VARCHAR2(40)
掛率1                              NOT NULL NUMBER(6,3)
掛率2                              NOT NULL NUMBER(6,3)
数量合計                             NOT NULL NUMBER(9,2)
明細金額合計                           NOT NULL NUMBER(10)
内税消費税                            NOT NULL NUMBER(10)
外税消費税                            NOT NULL NUMBER(10)
上代合計                             NOT NULL NUMBER(10)
下代合計                             NOT NULL NUMBER(10)
メモ                               NOT NULL VARCHAR2(300)
掛計上FLG                           NOT NULL NUMBER(1)
手入力伝票NO                          NOT NULL VARCHAR2(30)
SESS_ID                          NOT NULL NUMBER(14)
MOD_SEQ                          NOT NULL NUMBER(14)
外税対象金額                           NOT NULL NUMBER(10)
SYSFLG                           NOT NULL NUMBER(2)
関連伝票NO                           NOT NULL NUMBER(14)
関連伝票NO2                          NOT NULL NUMBER(14)
送料                               NOT NULL NUMBER(10)
手数料                              NOT NULL NUMBER(10)
値引1                              NOT NULL NUMBER(10)
値引2                              NOT NULL NUMBER(10)
値引3                              NOT NULL NUMBER(10)
値引枚数                             NOT NULL NUMBER(9,2)
NW1                              NOT NULL VARCHAR2(16)
NW2                              NOT NULL VARCHAR2(16)
MEMO2                            NOT NULL VARCHAR2(60)
SYSFLG2                          NOT NULL NUMBER(2)
来勘FLG                            NOT NULL NUMBER(2)
為替区分                             NOT NULL VARCHAR2(20)
レート1                             NOT NULL NUMBER(9,6)
レート2                             NOT NULL NUMBER(9,6)
外貨合計                             NOT NULL NUMBER(12,2)
在庫計上FLG                          NOT NULL NUMBER(1)
売仕内税消費税                          NOT NULL NUMBER(10)
売仕外税消費税                          NOT NULL NUMBER(10)
原価合計                             NOT NULL NUMBER(10)
支払区分                             NOT NULL VARCHAR2(20)
配送区分                             NOT NULL VARCHAR2(20)
返品伝票NO                           NOT NULL NUMBER(14)
入力区分                             NOT NULL VARCHAR2(20)
注文番号                             NOT NULL VARCHAR2(100)
送信FLG                            NOT NULL NUMBER(2)
STATUS                           NOT NULL NUMBER(1)
印刷FLG                            NOT NULL NUMBER(4)
CUST01                           NOT NULL VARCHAR2(8)
CUST02                           NOT NULL VARCHAR2(8)
CUST03                           NOT NULL VARCHAR2(8)
CUST04                           NOT NULL VARCHAR2(8)
請求書NO                            NOT NULL NUMBER(14)
展示会CD                            NOT NULL VARCHAR2(20)
経費更新FLG                          NOT NULL NUMBER(2)
SET展開FLG                         NOT NULL NUMBER(2)
週NO                              NOT NULL NUMBER(6)
回数                               NOT NULL NUMBER(10,2)
備考CD                             NOT NULL VARCHAR2(20)
納品先CD                            NOT NULL VARCHAR2(8)
担当者CD                            NOT NULL VARCHAR2(20)
EOS伝票NO                          NOT NULL VARCHAR2(100)
EOS店舗CD                          NOT NULL VARCHAR2(100)
EOS項目01                          NOT NULL VARCHAR2(100)
EOS項目02                          NOT NULL VARCHAR2(100)
EOS項目03                          NOT NULL VARCHAR2(100)
EOS項目04                          NOT NULL VARCHAR2(100)
EOS項目05                          NOT NULL VARCHAR2(100)
EOS項目06                          NOT NULL VARCHAR2(100)
EOS項目07                          NOT NULL VARCHAR2(100)
EOS項目08                          NOT NULL VARCHAR2(100)
EOS項目09                          NOT NULL VARCHAR2(100)
EOS項目10                          NOT NULL VARCHAR2(100)
相手伝票NO                           NOT NULL NUMBER(14)
回収金額                             NOT NULL NUMBER(10)
拡張項目01                           NOT NULL VARCHAR2(100)
拡張項目02                           NOT NULL VARCHAR2(100)
拡張項目03                           NOT NULL VARCHAR2(100)
拡張項目04                           NOT NULL VARCHAR2(100)
拡張項目05                           NOT NULL VARCHAR2(100)
拡張項目06                           NOT NULL VARCHAR2(100)
拡張項目07                           NOT NULL VARCHAR2(100)
拡張項目08                           NOT NULL VARCHAR2(100)
拡張項目09                           NOT NULL VARCHAR2(100)
拡張項目10                           NOT NULL VARCHAR2(100)
承認FLG                            NOT NULL NUMBER(2)
TAG作成FLG                         NOT NULL NUMBER(2)
消費税率                             NOT NULL NUMBER(6,3)

desc HC$tran_tori1

名前                               NULL?    型
-------------------------------- -------- ----------------------------
SEQ_NO                           NOT NULL NUMBER(14)
VDATE_CREATE                     NOT NULL NUMBER(14,8)
VDATE_UPDATE                     NOT NULL NUMBER(14,8)
ヘッダNO                            NOT NULL NUMBER(14)
行NO                              NOT NULL NUMBER(14)
明細取引区分                           NOT NULL NUMBER(2)
商品CD                             NOT NULL VARCHAR2(20)
原価FLG                            NOT NULL NUMBER(2)
色CD                              NOT NULL VARCHAR2(20)
サイズCD                            NOT NULL VARCHAR2(20)
明細名称                             NOT NULL VARCHAR2(100)
数量                               NOT NULL NUMBER(9,2)
単価                               NOT NULL NUMBER(9,2)
金額                               NOT NULL NUMBER(10)
内税消費税                            NOT NULL NUMBER(10)
外税消費税                            NOT NULL NUMBER(10)
上代単価                             NOT NULL NUMBER(9,2)
上代金額                             NOT NULL NUMBER(10)
下代単価                             NOT NULL NUMBER(9,2)
下代金額                             NOT NULL NUMBER(10)
明細メモ                             NOT NULL VARCHAR2(300)
関連商品CD                           NOT NULL VARCHAR2(20)
HHT_SEQ_NO                       NOT NULL NUMBER(14)
SESS_ID                          NOT NULL NUMBER(14)
MOD_SEQ                          NOT NULL NUMBER(14)
消費税計算方法                          NOT NULL NUMBER(1)
在庫計上日                            NOT NULL VARCHAR2(8)
伝票処理区分                           NOT NULL NUMBER(2)
商品シリアル                           NOT NULL VARCHAR2(40)
関連伝票NO                           NOT NULL NUMBER(14)
関連伝票行NO                          NOT NULL NUMBER(14)
JANCODE                          NOT NULL VARCHAR2(20)
SYSFLG                           NOT NULL NUMBER(2)
為替区分                             NOT NULL VARCHAR2(20)
レート1                             NOT NULL NUMBER(9,6)
レート2                             NOT NULL NUMBER(9,6)
外貨単価                             NOT NULL NUMBER(12,2)
外貨金額                             NOT NULL NUMBER(12,2)
原価                               NOT NULL NUMBER(9,2)
SYSFLG2                          NOT NULL NUMBER(2)
売仕内税消費税                          NOT NULL NUMBER(10)
売仕外税消費税                          NOT NULL NUMBER(10)
原価単価                             NOT NULL NUMBER(10)
原価金額                             NOT NULL NUMBER(10)
完了FLG                            NOT NULL NUMBER(2)
印刷FLG                            NOT NULL NUMBER(4)
納品日                              NOT NULL VARCHAR2(8)
銀行CD                             NOT NULL VARCHAR2(20)
銀行支店CD                           NOT NULL VARCHAR2(20)
手形番号                             NOT NULL VARCHAR2(40)
手形期日                             NOT NULL VARCHAR2(8)
経費金額                             NOT NULL NUMBER(10)
明細値引                             NOT NULL NUMBER(10)
小計値引                             NOT NULL NUMBER(10)
ポイント                             NOT NULL NUMBER(10)
備考CD01                           NOT NULL VARCHAR2(20)
備考01                             NOT NULL VARCHAR2(100)
備考CD02                           NOT NULL VARCHAR2(20)
備考02                             NOT NULL VARCHAR2(100)
備考CD03                           NOT NULL VARCHAR2(20)
備考03                             NOT NULL VARCHAR2(100)
掛率                               NOT NULL NUMBER(6,3)
相手商品NO                           NOT NULL VARCHAR2(20)
明細倉庫CD                           NOT NULL VARCHAR2(8)
拡張摘要1                            NOT NULL VARCHAR2(100)
拡張摘要2                            NOT NULL VARCHAR2(100)
拡張摘要3                            NOT NULL VARCHAR2(100)
元数量                              NOT NULL NUMBER(10,2)
セット数量                            NOT NULL NUMBER(10,2)
TAG種                             NOT NULL NUMBER(2)
配分PTN                            NOT NULL VARCHAR2(20)
初期納品日                            NOT NULL VARCHAR2(8)
表示行NO                            NOT NULL NUMBER(14)
明細承認FLG                          NOT NULL NUMBER(2)
TAG作成FLG                         NOT NULL NUMBER(2)
TAG発行FLG                         NOT NULL NUMBER(2)
配送区分                             NOT NULL NUMBER(2)
明細値引1                            NOT NULL NUMBER(10)
小計値引1                            NOT NULL NUMBER(10)
明細取引先CD1                         NOT NULL VARCHAR2(8)
明細符号                             NOT NULL NUMBER(2)		 

*/
	T? getMaster<T>(string code) where T : class, IBaseCodeName, new() {
		if (string.IsNullOrWhiteSpace(code))
			return null;
		var current = _toDb.FirstOrDefault<T>("where Code=@0", code);
		return current;
	}
	MasterMeisho? getMeisho(string kubun, string code) {
		if (string.IsNullOrWhiteSpace(code))
			return null;
		var current = _toDb.FirstOrDefault<MasterMeisho>("where Kubun=@0 and Code=@1", [kubun, code]);
		return current;
	}
	int getCalcFlag(int kubun) {
		return kubun switch {
			>= 20 and < 40 => -1,
			_ => 1,
		};
	}

	CodeNameView? getCodeNameView<T>(string code, out long id) where T : BaseDbClass, IBaseCodeName, new() {
		id = 0;
		if (string.IsNullOrWhiteSpace(code))
			return null;
		var current = _toDb.FirstOrDefault<T>("where Code=@0", code);
		if (current == null)
			return null;
		id = current.Id;
		return new CodeNameView(current.Id, current.Code, current.Name);
	}

}
