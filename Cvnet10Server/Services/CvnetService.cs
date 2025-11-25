using CodeShare;
using Cvnet10Base;
using Cvnet10Server;
using Grpc.Core;
using Microsoft.Data.Sqlite;
using NPoco;
using ProtoBuf.Grpc;
using Newtonsoft.Json;


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
		var result = new CvnetMsg() { Flag = CvnetFlag.Msg800_Error_Start};
		result.Code = -1;



        if (request.Flag == CvnetFlag.Msg001_CopyReply) {
			result.Code = 0;
            result.Flag = request.Flag;
            result.DataType = request.DataType;
			result.DataMsg = request.DataMsg;
		}
		if (request.Flag == CvnetFlag.Msg002_GetVersion) {
			result.Code = 0;
			result.DataType = typeof(List<decimal>);
			List<decimal> versions = new List<decimal>();
			versions.Add(1.002m);
			versions.Add(3.1415m);
            result.DataMsg = JsonConvert.SerializeObject(versions, jsonOptions);
		}
		if(request.Flag == CvnetFlag.Msg003_GetEnv) {
            result.Code = 0;
			//匿名型をつくる
			(decimal, string, SysLogin) envs = ( 3.1415m,  "Hello, Cvnet10!", new SysLogin { Id=2345});
            result.DataType = envs.GetType();
			result.DataMsg = JsonConvert.SerializeObject(envs, jsonOptions);
        }
        return Task.FromResult(result);
	}
}