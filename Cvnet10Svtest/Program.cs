using CodeShare; // ここは生成されたgRPCクライアントのnamespaceに合わせてください
using Cvnet10Base;
using Grpc.Core;
using Grpc.Net.Client;
using Newtonsoft.Json;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Client;

class Program
{
    static async Task Main(string[] args)
    {
        // HttpClient に grpc-accept-encoding ヘッダを設定して gzip を受け入れる
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("grpc-accept-encoding", "gzip");

        using var channel = GrpcChannel.ForAddress("https://localhost:7119", // サーバ側 launchSettings.json の設定に依存
            new GrpcChannelOptions { HttpClient = httpClient }); // ここでカスタム HttpClient を渡す

        var client = channel.CreateGrpcService<ICvnetService>();


		var wrk = new SysLogin();
		wrk.Id_Shain = 1234;
		wrk.LoginId = "loginaaaa";
		var req = new CvnetMsg { Flag = CvnetFlag.Msg001_CopyReply};

		// 共通オプション（必要に応じてカスタマイズ／コンバータを追加）
		var jsonOptions = new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore,
			Formatting = Formatting.None,
        };
        // メタデータで圧縮アルゴリズムを宣言
        var callOptions = new Grpc.Core.CallOptions(new Metadata { { "grpc-encoding", "gzip" } });
		var context = new CallContext(callOptions);

        req.DataType = typeof(SysLogin);
		req.DataMsg = JsonConvert.SerializeObject(wrk, jsonOptions);
		req.Flag = CvnetFlag.Msg001_CopyReply;


        var reply = await client.QueryMsgAsync(req, context);

		// Type 指定でのデシリアライズ（戻り値は object）
		var ret0 = JsonConvert.DeserializeObject(reply.DataMsg!, reply.DataType!, jsonOptions);
//		var ret2 = await client.





		req.Flag = CvnetFlag.Msg002_GetVersion;

		var reply2 = await client.QueryMsgAsync(req);
		req.Flag = CvnetFlag.Msg001_CopyReply;
		req.DataType = typeof((SysLogin, double));
		(SysLogin, double) ret1= (wrk, 1.24);
		req.DataMsg = JsonConvert.SerializeObject(ret1, jsonOptions);
		var rep3 = await client.QueryMsgAsync(req);

		req.Flag = CvnetFlag.Msg003_GetEnv;
        var rep4 = await client.QueryMsgAsync(req);
		var wrk4 = JsonConvert.DeserializeObject(rep4.DataMsg!, rep4.DataType!, jsonOptions);



        var rep = JsonConvert.DeserializeObject<List<decimal>>(reply2.DataMsg!, jsonOptions);
	}
}
