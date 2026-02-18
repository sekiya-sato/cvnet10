using CodeShare;
using CommunityToolkit.Mvvm.ComponentModel;
using Cvnet10Asset;
using Cvnet10Base.Oracle;
using Cvnet10Base.Share;
using Cvnet10DomainLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.Sqlite;
using Oracle.ManagedDataAccess.Client;
using ProtoBuf.Grpc;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;


namespace Cvnet10Server.Services;

public partial class CvnetCoreService : ICvnetCoreService {
	private readonly ILogger<CvnetCoreService> _logger;
	private readonly IConfiguration _configuration;
	private readonly IWebHostEnvironment _env;
	private readonly ExDatabase _db;
	// private readonly IScheduler _scheduler;
	private readonly IHttpContextAccessor _httpContextAccessor;
	public CvnetCoreService(ILogger<CvnetCoreService> logger, IConfiguration configuration, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, ExDatabase db) {
		ArgumentNullException.ThrowIfNull(logger);
		ArgumentNullException.ThrowIfNull(configuration);
		ArgumentNullException.ThrowIfNull(env);
		ArgumentNullException.ThrowIfNull(httpContextAccessor);
		ArgumentNullException.ThrowIfNull(db);
		_logger = logger;
		_configuration = configuration;
		_env = env;
		_db = db;
		// _scheduler = scheduler;
		_httpContextAccessor = httpContextAccessor;
	}
	// ToDo : テストが終わったら、[AllowAnonymous] を [Authorize] へ変更

	[AllowAnonymous]
	//[Authorize]
	public Task<CvnetMsg> QueryMsgAsync(CvnetMsg request, CallContext context = default) {
		_logger.LogInformation($"gRPCリクエストQueryMsgAsync Flag: {request.Flag}, DataType: {request.DataType.ToString()}");
		var result = new CvnetMsg() { Flag = CvnetFlag.Msg800_Error_Start };
		result.Code = -1;
		var start = DateTime.Now;
		var dict0 = new Dictionary<string, string>();

		switch (request.Flag) {
			case CvnetFlag.Msg001_CopyReply: // エコー
				result.Code = 0;
				result.Flag = request.Flag;
				result.DataType = request.DataType;
				result.DataMsg = request.DataMsg;
				break;
			case CvnetFlag.Msg002_GetVersion: // 内部バージョン取得
				result.Code = 0;
				result.Flag = request.Flag;
				result.DataType = typeof(VersionInfo);
				result.DataMsg = Common.SerializeObject(AppInit.Version);
				break;
			case CvnetFlag.Msg003_GetEnv: // 環境変数取得
				result.Code = 0;
				result.Flag = request.Flag;
				// 環境変数を取得して Dictionary<string,string> に変換、JSON シリアライズして返す
				var envVars = Environment.GetEnvironmentVariables();
				dict0 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				foreach (DictionaryEntry entry in envVars) {
					var key = entry.Key?.ToString() ?? string.Empty;
					var value = entry.Value?.ToString() ?? string.Empty;
					dict0[key] = value;
				}
				result.DataType = typeof(Dictionary<string, string>);
				result.DataMsg = Common.SerializeObject(dict0);
				break;
			case CvnetFlag.MSg040_ConvertDb: // DB変換処理を実装する
			case CvnetFlag.MSg041_ConvertDbInit:
				result.Code = 0;
				var oracleConnectionString = _configuration.GetConnectionString("oracle");
				if (string.IsNullOrWhiteSpace(oracleConnectionString))
					throw new InvalidOperationException("Connection string 'oracle' is missing. Configure it in appsettings.json under ConnectionStrings.");
				// 
				var fromDb =  ExDatabaseOracle.GetDbConn(oracleConnectionString);
				var cnvDb = new ConvertDb(fromDb, _db);
				dict0 = new Dictionary<string, string>();
				try {
					var initFlg = request.Flag == CvnetFlag.MSg041_ConvertDbInit;
					cnvDb.ConvertAll(initFlg);
					dict0["Status"] = "Success";
					var timespan = DateTime.Now - start;
					dict0["Timesec"] = timespan.TotalSeconds.ToString();
				}
				catch (Exception ex) {
					dict0["Status"] = "Error";
					dict0["Message"] = ex.Message;
				}
				result.DataType = typeof(Dictionary<string, string>);
				result.DataMsg = Common.SerializeObject(dict0);
				break;
			case CvnetFlag.Msg101_Op_Query: // レコードの取得
				result = subLogicMsg_Op_Query(request, context);
				break;
			case CvnetFlag.Msg201_Op_Execute: // レコードの修正
				result = subLogicMsg_Op_Execute(request, context);
				break;
			case CvnetFlag.Msg700_Test_Start:
				result = subLogicMsg700(request, context);
				break;
			case CvnetFlag.Msg701_TestCase001:
				result = subLogicMsg701(request, context);
				break;
			case CvnetFlag.Msg702_TestCase002:
				result = subLogicMsg702(request, context);
				break;
			default:
				result.Code = -1;
				result.Flag = CvnetFlag.Msg800_Error_Start;
				result.DataType = typeof(string);
				result.DataMsg = "Unimplemented function.";
				break;
		}



		return Task.FromResult(result);
	}

	/// <summary>
	/// ストリーミングメッセージを処理する
	/// </summary>
	/// <param name="request"></param>
	/// <param name="context"></param>
	/// <returns></returns>
	[AllowAnonymous]
	//[Authorize]
	public async IAsyncEnumerable<StreamMsg> QueryMsgStreamAsync(CvnetMsg request, CallContext context = default) {
		ArgumentNullException.ThrowIfNull(request);
		var ct = context.CancellationToken;
		_logger.LogInformation("gRPCストリーミングリクエスト QueryMsgStreamAsync Flag: {Flag}, DataType: {DataType}", request.Flag, request.DataType);
		await Task.Yield();

		if (request.Flag is not CvnetFlag.MSg060_StreamingTest) {
			yield return new StreamMsg {
				Flag = request.Flag,
				Code = -1,
				DataType = typeof(string),
				DataMsg = "Unimplemented function.",
				Progress = 0,
				IsCompleted = true,
				IsError = true
			};
			yield break;
		}
		// 順番にメッセージを返す
		// Note: 初期化処理,dbの前処理など
		/* 
		var oracleConnectionString = _configuration.GetConnectionString("oracle");
		if (string.IsNullOrWhiteSpace(oracleConnectionString)) {
			yield return new StreamMsg {
				Flag = request.Flag,
				Code = -1,
				Message = "Connection string 'oracle' is missing. Configure it in appsettings.json under ConnectionStrings.",
				Progress = 0,
				IsCompleted = true,
				IsError = true
			};
			yield break;
		}
		*/
		var start = DateTime.Now;

		var steps = new (string Name, Func<int> Action)[] {
			("This is First Step", () => SleepTask()),
			("This is Second Step", () => SleepTask()),
			("This is Third Step", () => SleepTask()),
			("This is 4th Step", () => SleepTask()),
			("This is 5th Step", () => SleepTask()),
			("This is 6th Step", () => SleepTask()),
			("This is 7th Step", () => SleepTask()),
			("This is 8th Step", () => SleepTask()),
		};

		for (var index = 0; index < steps.Length; index++) {
			ct.ThrowIfCancellationRequested();
			var (name, action) = steps[index];
			var startProgress = index * 100 / steps.Length;
			yield return new StreamMsg {
				Flag = request.Flag,
				Code = 0,
				DataType = typeof(string),
				DataMsg = $"開始: {name} ------------",
				Progress = startProgress
			};

			var count = action();
			var endProgress = (int)Math.Round((index + 1) * 100d / steps.Length, MidpointRounding.AwayFromZero);
			yield return new StreamMsg {
				Flag = request.Flag,
				Code = 0,
				DataType = typeof(string),
				DataMsg = $"完了: {name} 件数={count}",
				Progress = endProgress
			};
		}

		var elapsed = DateTime.Now - start;
		yield return new StreamMsg {
			Flag = request.Flag,
			Code = 0,
			DataType = typeof(string),
			DataMsg = $"完了: {elapsed.TotalSeconds:0.0}s",
			Progress = 100,
			IsCompleted = true
		};
	}

	static int SleepTask() {
		for (int i = 0; i < 3; i++) {
			Thread.Sleep(1000); // await Task.Delay(1000);
		}
		return 0;
	}
}