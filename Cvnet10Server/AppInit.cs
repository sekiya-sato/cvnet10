using Cvnet10Base;
using Cvnet10Server.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;

namespace Cvnet10Server;
public class AppInit {
	private readonly IConfiguration _configuration;

	public AppInit(IConfiguration configuration) {
		_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
	}

	public void Init() {
		var sqliteFileName = _configuration.GetConnectionString("sqlite");
		if (string.IsNullOrWhiteSpace(sqliteFileName))
			throw new InvalidOperationException("Connection string 'sqlite' is missing. Configure it in appsettings.json under ConnectionStrings.");

		var db = ExSqliteDatabase.GetDbConn(sqliteFileName);
		db.CreateTable<SysLogin>();
		db.CreateTable<SysHistJwt>();


	}

}
