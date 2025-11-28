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
