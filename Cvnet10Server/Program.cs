using Cvnet10Server.Services;
using Grpc.Net.Compression;
using ProtoBuf.Grpc.Server;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCodeFirstGrpc((options => {
    // CompressionLevel は用途に応じて調整 (Fastest, Optimal 等)
    options.CompressionProviders.Add(new GzipCompressionProvider(CompressionLevel.Fastest));
    // サーバーから圧縮済みレスポンスを返す際に使うアルゴリズム名
    options.ResponseCompressionAlgorithm = "gzip";
    options.EnableDetailedErrors = true;
    options.MaxReceiveMessageSize = 100 * 1024 * 1024; // 100 MB
    options.MaxSendMessageSize = 100 * 1024 * 1024; // 100 MB
}));

// Add services to the container.
//builder.Services.AddGrpc();

var app = builder.Build();
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

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<CvnetService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
