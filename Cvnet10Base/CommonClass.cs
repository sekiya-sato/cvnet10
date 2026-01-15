// ファイル概要:
// - 共通ユーティリティ `Common` を定義し、バージョン情報と JSON シリアライザ設定を共有します。
// - サーバー/クライアント双方で同一のシリアライズポリシーを維持するための集中管理ポイントです。
// 依存関係:
// - Newtonsoft.Json の JsonConvert API。
// 変更ポリシー:
// - jsonOptions のデフォルトを変える場合はクライアントとの互換性影響をレビューし、バージョン番号を更新します。
// - Serialize/Deserialize のシグネチャを変更せず、必要ならオーバーロードを追加してください。
// COPILOT: JSON 設定を拡張する際は null/既定値の扱いをテストでカバーし、必要なら TypeConverter を追加検討すること。

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Cvnet10Base {
	public class Common {
		public static string Version { get; } = "1.00.00";

        // JsonConvert共通オプション（必要に応じてカスタマイズ／コンバータを追加）
        private static readonly JsonSerializerSettings jsonOptions = new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None,
            DefaultValueHandling = DefaultValueHandling.Ignore,
        };
        /// <summary>
        /// 共通オプションを使ってオブジェクトをシリアライズ
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeObject(object obj){
            var ret = JsonConvert.SerializeObject(obj, jsonOptions);
            return ret;
        }
        /// <summary>
        /// 共通オプションを使ってオブジェクトをデシリアライズ
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object? DeserializeObject(string obj, Type t) {
            var ret = JsonConvert.DeserializeObject(obj, t, jsonOptions);
            return ret;
        }









    }
}
