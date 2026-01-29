using CodeShare;
using Cvnet10AppShared;
using Cvnet10Base;
using Cvnet10Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.Sqlite;
using Oracle.ManagedDataAccess.Client;
using ProtoBuf.Grpc;
using System.Collections;


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
		_logger = logger;
		_configuration = configuration;
		_env = env;
		/*
		var connStr = _configuration.GetConnectionString("sqlite");
		if (string.IsNullOrWhiteSpace(connStr)) {
			throw new InvalidOperationException("Connection string 'sqlite' is not configured.");
		}
		_db = ExSqliteDatabase.GetDbConn(connStr);
		 */
		_db = db;

		// _scheduler = scheduler;
		_httpContextAccessor = httpContextAccessor;
	}
	// ToDo : テストが終わったら、[AllowAnonymous] を [Authorize] へ変更

	[AllowAnonymous]
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
}