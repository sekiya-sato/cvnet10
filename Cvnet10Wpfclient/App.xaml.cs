using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace Cvnet10Wpfclient {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
		public App() {
			//XAMLでの言語表示を変更。各xamlで Language="ja"などを指定しなくても、自動で言語を切り替える
			if (Thread.CurrentThread.CurrentCulture != null) {
				var culture = Thread.CurrentThread.CurrentCulture; // "ja-JP" or "en-US"
				XmlLanguage language = XmlLanguage.GetLanguage(culture.IetfLanguageTag);
				FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(language));
			}
			var config  = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json") //appsettings.jsonは必須 [appsettings.json is required]
				.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "_"}.json", true)
				.Build();
			AppCurrent.Init(config);

		}
	}

}
