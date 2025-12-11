using CodeShare;
using Cvnet10Base;
using Newtonsoft.Json;
using ProtoBuf.Grpc;
using System.Collections;
using Microsoft.Data.Sqlite;
using Oracle.ManagedDataAccess.Client;


namespace Cvnet10Server.Services;

public class CvnetService : ICvnetService {
	private readonly ILogger<GreeterService> _logger;
	public CvnetService(ILogger<GreeterService> logger) {
		_logger = logger;
	}

	// 共通オプション（必要に応じてカスタマイズ／コンバータを追加）
	private readonly JsonSerializerSettings jsonOptions = new JsonSerializerSettings {
		NullValueHandling = NullValueHandling.Ignore,
		Formatting = Formatting.None,
	};

	public Task<CvnetMsg> QueryMsgAsync(CvnetMsg request, CallContext context = default) {
		var result = new CvnetMsg() { Flag = CvnetFlag.Msg800_Error_Start };
		result.Code = -1;

		if (request.Flag == CvnetFlag.Msg001_CopyReply) {
			result.Code = 0;
			result.Flag = request.Flag;
			result.DataType = request.DataType;
			result.DataMsg = request.DataMsg;
		}
		else if (request.Flag == CvnetFlag.Msg002_GetVersion) {
			result.Code = 0;
			result.DataType = typeof(string);
			result.DataMsg = Common.Version;
		}
		else if (request.Flag == CvnetFlag.Msg003_GetEnv) {
			result.Code = 0;
            // 環境変数を取得して Dictionary<string,string> に変換、JSON シリアライズして返す
            var envVars = Environment.GetEnvironmentVariables();
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (DictionaryEntry entry in envVars) {
                var key = entry.Key?.ToString() ?? string.Empty;
                var value = entry.Value?.ToString() ?? string.Empty;
                dict[key] = value;
            }

            result.DataType = typeof(Dictionary<string, string>);
            result.DataMsg = Common.SerializeObject(dict);
        }
		else if (request.Flag == CvnetFlag.MSg004_ConvertDb) {
			result.Code = 0;
			var fromDb = new ExDatabase(new OracleConnection("User Id=CV00PKG;Password=CV00PKG;Data Source=192.168.9.243/cvnet;"));
			var toDb = new ExDatabase(new SqliteConnection("Data Source=sample.db"));
			var start = DateTime.Now;
			var cnvDb = new ConvertDb(fromDb, toDb);
			var dict = new Dictionary<string, string>();
			try {
				cnvDb.ConvertAll();
				dict["Status"] = "Success";
				var timespan = DateTime.Now - start;
				dict["Timesec"] = timespan.TotalSeconds.ToString();
			}
			catch (Exception ex) {
				dict["Status"] = "Error";
				dict["Message"] = ex.Message;
			}
			result.DataType = typeof(Dictionary<string, string>);
			result.DataMsg = Common.SerializeObject(dict);
		}
		return Task.FromResult(result);
	}
	public Task<LoginReply> LoginAsync(LoginRequest UserRequest, CallContext context = default) {
		var reply = new LoginReply {
			JwtMessage = string.Empty, // 必須プロパティを初期化
			Result = 0,
			Expire = DateTime.MinValue
		};
		// 未実装エラーを返す
		throw new NotImplementedException("LoginAsync is not implemented yet.");

		// 必要なロジックをここに追加
		// return Task.FromResult(reply);
	}

	public Task<LoginReply> LoginRefleshAsync(LoginRefresh UserRequest, CallContext context = default) {
		var reply = new LoginReply {
			JwtMessage = string.Empty, // 必須プロパティを初期化
			Result = 0,
			Expire = DateTime.MinValue
		};
		// 未実装エラーを返す
		throw new NotImplementedException("LoginRefleshAsync is not implemented yet.");

		// 必要なロジックをここに追加
		// return Task.FromResult(reply);
	}
}