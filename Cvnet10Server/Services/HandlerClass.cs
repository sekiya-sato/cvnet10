using CodeShare;
using Cvnet10Asset;
using Cvnet10Base.Oracle;
using Cvnet10Base.Share;
using Cvnet10DomainLogic;
using ProtoBuf.Grpc;
using System.Collections;

namespace Cvnet10Server.Services;

public partial class CvnetCoreService {
	// CvnetCoreService のハンドラ実装（元の switch の各ケースを分離）

	private CvnetMsg HandleCopyReply(CvnetMsg request, CallContext context) {
		ArgumentNullException.ThrowIfNull(request);
		_logger.LogDebug("HandleCopyReply invoked Flag:{Flag}", request.Flag);

		var result = new CvnetMsg { Flag = request.Flag };
		result.Code = 0;
		result.Flag = request.Flag;
		result.DataType = request.DataType;
		result.DataMsg = request.DataMsg;
		return result;
	}

	private CvnetMsg HandleGetVersion(CvnetMsg request, CallContext context) {
		ArgumentNullException.ThrowIfNull(request);
		_logger.LogDebug("HandleGetVersion invoked Flag:{Flag}", request.Flag);

		var result = new CvnetMsg { Flag = request.Flag };
		result.Code = 0;
		result.Flag = request.Flag;
		result.DataType = typeof(VersionInfo);
		result.DataMsg = Common.SerializeObject(AppInit.Version);
		return result;
	}

	private CvnetMsg HandleGetEnv(CvnetMsg request, CallContext context) {
		ArgumentNullException.ThrowIfNull(request);
		_logger.LogDebug("HandleGetEnv invoked Flag:{Flag}", request.Flag);

		var result = new CvnetMsg { Flag = request.Flag };
		result.Code = 0;
		result.Flag = request.Flag;

		// 環境変数を取得して Dictionary<string,string> に変換、JSON シリアライズして返す
		var envVars = Environment.GetEnvironmentVariables();
		var dict0 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		foreach (DictionaryEntry entry in envVars) {
			var key = entry.Key?.ToString() ?? string.Empty;
			var value = entry.Value?.ToString() ?? string.Empty;
			dict0[key] = value;
		}
		result.DataType = typeof(Dictionary<string, string>);
		result.DataMsg = Common.SerializeObject(dict0);
		return result;
	}

	private CvnetMsg HandleConvertDb(CvnetMsg request, CallContext context) {
		ArgumentNullException.ThrowIfNull(request);
		_logger.LogInformation("HandleConvertDb invoked Flag:{Flag}", request.Flag);

		var result = new CvnetMsg { Flag = request.Flag };
		result.Code = 0;
		var start = DateTime.Now;
		var dict0 = new Dictionary<string, string>();

		var oracleConnectionString = _configuration.GetConnectionString("oracle");
		if (string.IsNullOrWhiteSpace(oracleConnectionString))
			throw new InvalidOperationException("Connection string 'oracle' is missing. Configure it in appsettings.json under ConnectionStrings.");
		var fromDb = ExDatabaseOracle.GetDbConn(oracleConnectionString);
		var cnvDb = new ConvertDb(fromDb, _db);
		try {
			var initFlg = request.Flag == CvnetFlag.MSg041_ConvertDbInit;
			cnvDb.ConvertAll(initFlg);
			dict0["Status"] = "Success";
			var timespan = DateTime.Now - start;
			dict0["Timesec"] = timespan.TotalSeconds.ToString();
		}
		catch (Exception ex) {
			_logger.LogError(ex, "HandleConvertDb error");
			dict0["Status"] = "Error";
			dict0["Message"] = ex.Message;
		}
		result.DataType = typeof(Dictionary<string, string>);
		result.DataMsg = Common.SerializeObject(dict0);
		return result;
	}
}
