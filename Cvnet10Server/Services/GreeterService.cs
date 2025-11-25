using CodeShare;
using Cvnet10Server;
using Grpc.Core;
using Microsoft.Data.Sqlite;
using NPoco;
using ProtoBuf.Grpc;
using Cvnet10Base;
using Newtonsoft.Json;

namespace Cvnet10Server.Services {
	public class GreeterService : IGreeter {
		private readonly ILogger<GreeterService> _logger;
		public GreeterService(ILogger<GreeterService> logger) {
			_logger = logger;
		}

		public Task<HelloReply> SayHello(HelloRequest request, CallContext context = default) {
			return Task.FromResult(new HelloReply {
				Message = $"Hello {request.Name} : {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}"
			});
		}
		public Task<HelloReply> SaySqlite(HelloRequest request, CallContext context = default) {
			using var connection = new SqliteConnection("Data Source=sample.db");
			ExDatabase db = new ExDatabase(connection);
			db.CreateTable(typeof(SysLogin));
			var flg = false;
			if (flg) {
				db.CreateTable(typeof(MasterSysman), true);
				var sys0 = db.Fetch<MasterSysman>("SELECT * FROM MasterSysman");
				if (sys0.Count == 0) {
					var sysman = new MasterSysman {
						Name = "株式会社ディー・ティー・ピー",
						PostalCode = "107-0052",
						Address1 = "東京都港区赤坂2丁目17-55",
						Address2 = "赤坂ツインタワー\n本館7F",
						FiscalStartDate = "20250401",
					};
					var systax1 = new MasterSysTax {
						Id = 1,
						TaxRate = 10,
					};
					var systax2 = new MasterSysTax {
						Id = 2,
						TaxRate = 8,
					};
					sysman.Jsub = new List<MasterSysTax> { systax1, systax2 };
					db.Insert<MasterSysman>(sysman);
				}
				var sys1 = db.Fetch<MasterSysman>();
			}
			var sys2 = db.Fetch<MasterSysman>();

			var ret = db.Fetch<Dictionary<string, object>>("SELECT name FROM sqlite_master WHERE type='table' AND name='users';");
			if(ret.Count == 0) {
				db.Execute(@"
			CREATE TABLE IF NOT EXISTS users (
				id INTEGER PRIMARY KEY AUTOINCREMENT,
				name TEXT(40) NOT NULL,
				cnt INTEGER DEFAULT 0
			);
			INSERT INTO users (name, cnt) VALUES ('Alice',1);
			");
			}
			else {
				db.Execute("UPDATE users SET cnt = cnt + 1 WHERE name = 'Alice';");
			}
			var ret2 = db.Fetch<Dictionary<string, object>>("SELECT * FROM users WHERE name = 'Alice'");
			var cnt = ret2.Count>0 ? ret2[0]?["cnt"] ?? 0 : 0;

			// Newtonsoft.Json でシリアライズ（配列として返るためクライアント側は List<MasterSysman> を期待するか、先頭要素を使う）
			var jsonSettings = new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore,
				Formatting = Formatting.None
			};
			var json = JsonConvert.SerializeObject(sys2[0], jsonSettings);


			return Task.FromResult(new HelloReply {
				Message = $"Hello {cnt} : {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}",
				JsonMsg = json,
				Type0 = typeof(MasterSysman),
			});
		}
	}
}
