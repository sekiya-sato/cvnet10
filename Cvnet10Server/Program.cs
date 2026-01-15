// ファイル概要:
// - Cvnet10Server のエントリーポイント。gRPC ホストを構成し、サービスと中間ウェアを登録します。
// - Kestrel 制限、圧縮、ForwardedHeaders などランタイム設定を集中管理します。
// 依存関係:
// - ASP.NET Core gRPC スタック、ProtoBuf.Grpc.Server、NLog。
// 変更ポリシー:
// - builder.Services への登録を変更する際は DI スコープや configure 順序に注意し、複数環境設定(appsettings)を整合させます。
// - ログや中間ウェアを追加する前にパフォーマンス/セキュリティへの影響を確認してください。
// COPILOT: 新しいサービスをマップする場合は .MapGrpcService<> とルートハンドラーを適切に配置し、ヘルスチェックやメトリクスの露出も検討すること。

using Cvnet10Server.Services;
using Grpc.Net.Compression;
using Microsoft.AspNetCore.HttpOverrides;
using NLog;
using NLog.Web;
using ProtoBuf.Grpc.Server;
using System.IO.Compression;


var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddNLogWeb();



builder.Services.AddCodeFirstGrpc((options => {
    // CompressionLevel は用途に応じて調整 (Fastest, Optimal 等)
    options.CompressionProviders.Add(new GzipCompressionProvider(CompressionLevel.Fastest));
    // サーバーから圧縮済みレスポンスを返す際に使うアルゴリズム名
    options.ResponseCompressionAlgorithm = "gzip";
    options.EnableDetailedErrors = true;
    options.MaxReceiveMessageSize = 800 * 1024 * 1024; // 800 MB
    options.MaxSendMessageSize = 800 * 1024 * 1024; // 800 MB
}));

builder.WebHost.ConfigureKestrel(serverOptions => {
    // TODO: Kestrel デフォルトのオプションは必要に応じて追加する(2024/08/15)
    // [TODO: Add default options for Kestrel as needed]
    serverOptions.Limits.MaxRequestBodySize = 838_860_800; // 800 MB
    serverOptions.Limits.MaxConcurrentConnections = 100; // 最大同時接続数 [Maximum number of simultaneous connections]
    serverOptions.Limits.Http2.MaxStreamsPerConnection = 100; // 最大ストリーム数 [Maximum number of streams]
});
builder.Services.AddHttpContextAccessor(); // HttpContextを取得可能にする [Make HttpContext accessible]

var app = builder.Build();
var logger = app.Logger;
logger.LogDebug("Application Start ------------------------------------");
// リクエスト／レスポンスヘッダをログするミドルウェア
app.Use(async (context, next) => {
    var logger = app.Logger;
    logger.LogInformation("Incoming request path: {Path}", context.Request.Path);
    foreach (var h in context.Request.Headers)
        logger.LogInformation("REQ HDR: {Key} = {Value}", h.Key, h.Value.ToString());

    await next();

    // レスポンスヘッダ（トレーラはここで見えない場合あり）
    foreach (var h in context.Response.Headers)
        logger.LogInformation("RES HDR: {Key} = {Value}", h.Key, h.Value.ToString());
});

app.UseForwardedHeaders(new ForwardedHeadersOptions {
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<CvnetService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
