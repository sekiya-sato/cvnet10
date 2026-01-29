using CodeShare; // ここは生成されたgRPCクライアントのnamespaceに合わせてください
using Cvnet10Base;
using Grpc.Core;
using Grpc.Net.Client;
using Newtonsoft.Json;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Client;
using System.Collections.Generic;

class Program
{
    static async Task Main(string[] args)
    {
        // HttpClient に grpc-accept-encoding ヘッダを設定して gzip を受け入れる
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("grpc-accept-encoding", "gzip");
		// タイムアウトを延長（例: 10 分）
		httpClient.Timeout = TimeSpan.FromMinutes(10);

		using var channel = GrpcChannel.ForAddress("https://localhost:7119", // サーバ側 launchSettings.json の設定に依存
            new GrpcChannelOptions { HttpClient = httpClient }); // ここでカスタム HttpClient を渡す

		var client = channel.CreateGrpcService<ICvnetService>();

		/*
		var wrk = new SysLogin { Id_Shain=1234, LoginId = "loginaaaa" };
		var req = new CvnetMsg { Flag = CvnetFlag.Msg001_CopyReply};

        // メタデータで圧縮アルゴリズムを宣言
        var callOptions = new Grpc.Core.CallOptions(new Metadata { { "grpc-encoding", "gzip" } });
		var context = new CallContext(callOptions);

        req.Flag = CvnetFlag.Msg001_CopyReply;
        req.DataType = typeof(SysLogin);
		req.DataMsg = Common.SerializeObject(wrk);

        var reply = await client.QueryMsgAsync(req, context);

		// Type 指定でのデシリアライズ（戻り値は object）
		var ret0 = Common.DeserializeObject(reply.DataMsg!, reply.DataType!);


		req.Flag = CvnetFlag.Msg002_GetVersion;

		var reply2 = await client.QueryMsgAsync(req, context);
		req.Flag = CvnetFlag.Msg001_CopyReply;
		req.DataType = typeof((SysLogin, double));
		(SysLogin, double) ret1= (wrk, 1.24);
		req.DataMsg = Common.SerializeObject(ret1);
		var rep3 = await client.QueryMsgAsync(req, context);
		*/
		var req = new CvnetMsg { Flag = CvnetFlag.Msg003_GetEnv };
        var rep_3 = await client.QueryMsgAsync(req);
		var wrk_3 = Common.DeserializeObject(rep_3.DataMsg!, rep_3.DataType!) as Dictionary<string, string>;
		req.Flag = CvnetFlag.MSg004_ConvertDb;
		var rep_4 = await client.QueryMsgAsync(req);
		var wrk_4 = Common.DeserializeObject(rep_4.DataMsg!, rep_4.DataType!) as Dictionary<string, string>;


	}
}
