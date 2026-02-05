using CodeShare;
using Cvnet10Wpfclient.Infrastructure;
using Cvnet10Wpfclient.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoBuf.Grpc.ClientFactory;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Cvnet10Wpfclient {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
		public static IHost? AppHost { get; private set; }
		public static ThemeService ThemeService { get; } = new();

		public App() {
			InitializeLanguage();
			AppHost = CreateHostBuilder().Build();
		}

		protected override async void OnStartup(StartupEventArgs e) {
			if (AppHost != null) {
				await AppHost.StartAsync();
				var configuration = AppHost.Services.GetRequiredService<IConfiguration>() as IConfigurationRoot
					?? throw new InvalidOperationException("IConfigurationRoot is not available.");
				AppCurrent.Init(configuration, AppHost.Services);
				ThemeService.ApplyTheme(AppTheme.Light);
			}
			base.OnStartup(e);
		}

		protected override async void OnExit(ExitEventArgs e) {
			if (AppHost != null) {
				await AppHost.StopAsync();
				AppHost.Dispose();
			}
			base.OnExit(e);
		}

		static void InitializeLanguage() {
			if (Thread.CurrentThread.CurrentCulture == null) {
				return;
			}
			var culture = Thread.CurrentThread.CurrentCulture;
			XmlLanguage language = XmlLanguage.GetLanguage(culture.IetfLanguageTag);
			FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(language));
		}

		/// <summary>
		/// Caution: This Logic does not rewrite !
		/// サービスはここで追加、タイムアウト指定、認証ハンドラ追加などを行う
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		static IHostBuilder CreateHostBuilder() {
			var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "_";
			return Host.CreateDefaultBuilder()
				.ConfigureAppConfiguration(builder => {
					builder.SetBasePath(Directory.GetCurrentDirectory());
					builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
					builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
				})
				.ConfigureServices((context, services) => {
					services.AddTransient<JwtAuthorizationHandler>(); // ← Singleton から変更

                    services.AddSingleton<SocketsHttpHandler>(_ => new SocketsHttpHandler {
						PooledConnectionIdleTimeout = TimeSpan.FromHours(6),
						KeepAlivePingDelay = TimeSpan.FromSeconds(120),
                        KeepAlivePingTimeout = TimeSpan.FromSeconds(15), // タイムアウト時間
						KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always, // 通信がない時でもPingを送る
						EnableMultipleHttp2Connections = true
                    });
					services.AddCodeFirstGrpcClient<ILoginService>((sp, options) => {
                        var serviceUrl = context.Configuration.GetConnectionString("Url")
                            ?? throw new InvalidOperationException("Connection string 'Url' is missing.");
                        options.Address = new Uri(serviceUrl);
                    })
                    .ConfigureChannel((sp, channelOptions) => {
                        channelOptions.HttpHandler = sp.GetRequiredService<SocketsHttpHandler>();
                    })
                    .AddHttpMessageHandler<JwtAuthorizationHandler>()
                    .ConfigureHttpClient(client => client.Timeout = Timeout.InfiniteTimeSpan);

                    services.AddCodeFirstGrpcClient<ICvnetCoreService>((sp, options) => {
                        var serviceUrl = context.Configuration.GetConnectionString("Url")
                            ?? throw new InvalidOperationException("Connection string 'Url' is missing.");
                        options.Address = new Uri(serviceUrl);
                    })
                    .ConfigureChannel((sp, channelOptions) => {
                        channelOptions.HttpHandler = sp.GetRequiredService<SocketsHttpHandler>();
                    })
                    .AddHttpMessageHandler<JwtAuthorizationHandler>()
                    .ConfigureHttpClient(client => client.Timeout = Timeout.InfiniteTimeSpan);
                });
		}
	}
}

