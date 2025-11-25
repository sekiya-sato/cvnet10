using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cvnet10Base;
using Microsoft.Data.Sqlite;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;


namespace Cvnet10Wpftest.ViewModels {
    public partial class MainViewModel : ObservableObject {
		[RelayCommand]
		private void Close(System.Windows.Window w) {
			w.Close();
		}

		[ObservableProperty]
		string? message;

		[RelayCommand]
		private async Task Test01() {
			using var connection = new SqliteConnection("Data Source=sample.db");
			ExDatabase db = new ExDatabase(connection);
			db.CreateTable(typeof(SysLogin));
			try {
				var orastr = "User Id=CV00PKG;Password=CV00PKG;Data Source=192.168.9.243/cvnet;";
				await using var con = new OracleConnection(orastr);
				await con.OpenAsync();
				ExDatabase oradb = new ExDatabase(con);
				var mstSys = oradb.Fetch<Dictionary<string, object>>("select * from HC$master_syskanri");
				var mstTax = oradb.Fetch<Dictionary<string, object>>("select * from HC$master_systax");

				// シンプルなクエリ例
				string sql = "SELECT COUNT(*) FROM HC$master_tokui";
				await using var cmd = new OracleCommand(sql, con);
				var result = await cmd.ExecuteScalarAsync();
				// 必要に応じて UI スレッドへ戻して通知等を行う
				Message = $"RowCount: {result}";
				var conv = new ConvertDb(oradb, db);
				// db.DropTable(typeof(MasterSysman));
				conv.CnvMasterSys();


			}
			catch (OracleException ex) {
				// 接続失敗や認証エラー等の処理
				Message = $"Oracle error: {ex.Message}";
			}
			catch (Exception ex) {
				Message = $"Unexpected error: {ex.Message}";
			}




		}

	}
}

