using CodeShare;
using Cvnet10DomainLogic;
using Microsoft.AspNetCore.Authorization;
using ProtoBuf.Grpc;


namespace Cvnet10Server.Services;

public partial class CvnetCoreService : ICvnetCoreService {
	private readonly ILogger<CvnetCoreService> _logger;
	private readonly IConfiguration _configuration;
	private readonly IWebHostEnvironment _env;
	private readonly ExDatabase _db;
	// private readonly IScheduler _scheduler;
	private readonly IHttpContextAccessor _httpContextAccessor;

	// フラグ -> ハンドラマップ
	private readonly Dictionary<CvnetFlag, Func<CvnetMsg, CallContext, CvnetMsg>> _handlers;

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

		// ハンドラ登録
		_handlers = new Dictionary<CvnetFlag, Func<CvnetMsg, CallContext, CvnetMsg>> {
			[CvnetFlag.Msg001_CopyReply] = HandleCopyReply,
			[CvnetFlag.Msg002_GetVersion] = HandleGetVersion,
			[CvnetFlag.Msg003_GetEnv] = HandleGetEnv,
			[CvnetFlag.MSg040_ConvertDb] = HandleConvertDb,
			[CvnetFlag.MSg041_ConvertDbInit] = HandleConvertDb,
			[CvnetFlag.Msg101_Op_Query] = (req, ctx) => subLogicMsg_Op_Query(req, ctx),
			[CvnetFlag.Msg201_Op_Execute] = (req, ctx) => subLogicMsg_Op_Execute(req, ctx),
			[CvnetFlag.Msg700_Test_Start] = (req, ctx) => subLogicMsg700(req, ctx),
			[CvnetFlag.Msg701_TestCase001] = (req, ctx) => subLogicMsg701(req, ctx),
			[CvnetFlag.Msg702_TestCase002] = (req, ctx) => subLogicMsg702(req, ctx),
		};
	}
	// ToDo : テストが終わったら、[AllowAnonymous] を [Authorize] へ変更

	[AllowAnonymous]
	//[Authorize]
	public Task<CvnetMsg> QueryMsgAsync(CvnetMsg request, CallContext context = default) {
		_logger.LogInformation($"gRPCリクエストQueryMsgAsync Flag: {request.Flag}, DataType: {request.DataType.ToString()}");
		ArgumentNullException.ThrowIfNull(request);

		if (_handlers.TryGetValue(request.Flag, out var handler)) {
			try {
				var result = handler(request, context) ?? new CvnetMsg() { Flag = CvnetFlag.Msg800_Error_Start, Code = -1, DataType = typeof(string), DataMsg = "Handler returned null." };
				return Task.FromResult(result);
			}
			catch (Exception ex) {
				_logger.LogError(ex, "QueryMsgAsync handler error Flag:{Flag}", request.Flag);
				var err = new CvnetMsg() { Flag = request.Flag, Code = -9902, Option = ex.Message, DataType = typeof(string), DataMsg = ex.Message };
				return Task.FromResult(err);
			}
		}

		// 未実装フラグ
		var defaultErr = new CvnetMsg {
			Flag = CvnetFlag.Msg800_Error_Start,
			Code = -1,
			DataType = typeof(string),
			DataMsg = "Unimplemented function."
		};
		return Task.FromResult(defaultErr);
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

		// 順番にメッセージを返す
		// Note: 初期化処理,dbの前処理など
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
		var start = DateTime.Now;
		// 処理のステップと対応するアクションを定義
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
	/// <summary>
	/// ダミーのタスク(時間がかかる処理のシミュレート)
	/// </summary>
	/// <returns></returns>
	static int SleepTask() {
		for (int i = 0; i < 3; i++) {
			Thread.Sleep(1000); // await Task.Delay(1000);
		}
		return 0;
	}
}
