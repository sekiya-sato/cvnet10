using Cvnet10Wpfclient.Util;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Cvnet10Wpfclient {
	/// <summary>
	/// グローバル変数
	/// </summary>
	public static class AppCurrent {
		// Backing field: 内部でのみ null 許容
		private static IConfigurationRoot? _config;
		private static string? _url;
		private static string? _dataDir;
		private static Guid? _clientId;
		/// <summary>
		/// サーバーのURL
		/// </summary>
		public static string Url => _url
			?? throw new InvalidOperationException("AppCurrent has not been initialized. Call Init() at application startup.");
		public static string DataDir => _dataDir
			?? throw new InvalidOperationException("AppCurrent has not been initialized. Call Init() at application startup.");
		public static Guid ClientId => _clientId ??= Guid.NewGuid();
		/// <summary>
		/// app.config内容
		/// </summary>
		public static IConfigurationRoot Config => _config
			?? throw new InvalidOperationException("AppCurrent has not been initialized. Call Init() at application startup.");
		/// <summary>
		/// ログイン認証後のJWT
		/// [JWT after login authentication]
		/// </summary>
		public static string? LoginJwt;

		/// <summary>
		/// Config読込処理：application startup で一度だけ実行すること
		/// </summary>
		public static void Init(IConfigurationRoot config) {
			if (_config != null) return;
			Debug.WriteLine("GlobalInitialize()実行");
			_config = config;
			_url = _config.GetConnectionString("Url");
			_dataDir = ClientLib.GetDataDir();
			// あれば取得する
			LoginJwt = _config.GetSection("AppStrings")?["LoginJwt"];
		}
		/// <summary>
		/// メタデータを取得する
		/// [Retrieve metadata]
		/// </summary>
		/// <returns></returns>
		public static CallContext GetDefaultCallContext() {
			var callOptions = new CallOptions(headers: new Metadata {
					new Metadata.Entry("X-ClientId", ClientId.ToString()),
					new Metadata.Entry("Authorization", $"Bearer {LoginJwt}"),
				});
			var callContext = new CallContext(
							callOptions: callOptions,
							flags: CallContextFlags.CaptureMetadata);
			return callContext;
		}



	}
}
