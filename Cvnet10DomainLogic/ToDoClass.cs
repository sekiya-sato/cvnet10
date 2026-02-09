namespace Cvnet10DomainLogic;

/*
 ToDo: 必要となる予定のロジック
在庫更新ロジック
買掛、売掛更新ロジック
支払、請求更新ロジック
外部連携ロジック (委託倉庫、他社POS、他社EC)
自動実行処理 (NCrontab.Scheduler.AspNetCore を利用)
検証終了(Windows上で): 印刷処理 ( <PackageReference Include="IKVM" )
	// PrintStream DLLの登録
	// IKVMの初期化コストを考慮し、インスタンス管理をDIに任せる App.xaml.cs などで登録
		builder.Services.AddScoped<IPrintService, PrintAdapter>();
	// 出力するgRPCサービスにIPrintServiceを注入(IDisposable)
	private readonly IPrintService _printService;
	public PrintController(ILogger<PrintController> logger, IPrintService printService, IWebHostEnvironment env) {
			_logger = logger;
			_printService = printService;
			_env = env;
		}
	// ubuntu上での必要ライブラリ
		sudo apt install -y libgdiplus fontconfig
		sudo apt install fonts-ipafont
 */

internal class ToDoClass {
}
