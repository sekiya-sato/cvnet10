using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cvnet10Base;
using Microsoft.Data.Sqlite;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using static System.Net.WebRequestMethods;


namespace Cvnet10Wpftest.ViewModels {
	public partial class MainViewModel : ObservableObject {
		[RelayCommand]
		private void Close(System.Windows.Window w) {
			w.Close();
		}

		[ObservableProperty]
		string? message;

		[ObservableProperty]
		MasterSysman? mstSysman;


		[ObservableProperty]
		MasterTokui? mstTokui;


		[RelayCommand]
		private async Task Test01() {
			var dir = AppDomain.CurrentDomain.BaseDirectory.Split("Cvnet10Wpftest")[0]; // プロジェクトの一つ上のフォルダ
			using var connection = new SqliteConnection(@$"Data Source={dir}\Cvnet10Server\sample.db");
			ExDatabase db = new ExDatabase(connection);
			try {
				MstSysman = db.Fetch<MasterSysman>().FirstOrDefault();
				Message = $"完了 {DateTime.Now.ToLongTimeString()}";

				/*
				db.CreateTable(typeof(MasterTokui), true);
				MstTokui = new MasterTokui() { Code = "001", Name = "TestTokui", TenType = EnumTokui.Tenpo, Address1 = "東京都浅草" };
				db.Insert<MasterTokui>(MstTokui);
				 */
				MstTokui = db.Fetch<MasterTokui>().FirstOrDefault();


				var wrk = MstTokui;
			}
			catch (Exception ex) {
				Message = $"Unexpected error: {ex.Message}";
			}
		}
		[RelayCommand]
		private async Task Test02() {
			var path = @"C:\gitroot\wrk\cv147\cv32data-new.db";
			var sqliteps = "587625D93E8412671901B1787A2611C7";
			var builder = new SqliteConnectionStringBuilder {
				DataSource = path,
	//			Password = sqliteps,
			}.ToString();
			try {
				var connstr = $"Data Source={path}; Password={sqliteps};";




				using var connection = new SqliteConnection(builder);
				await connection.OpenAsync(); // 接続を明示的に開く（パスワード検証）

				ExDatabase db = new ExDatabase(connection);
				db.CreateTable(typeof(MasterEndCustomer),true);

				// ここでデータベース操作を実行
				// 例: var data = db.Fetch<SomeTable>();

				Message = $"DB接続成功 {DateTime.Now.ToLongTimeString()}";
			}
			catch (SqliteException ex) {
				// エラーコード 26 = "file is not a database" (パスワード不一致)
				Message = $"パスワードエラー: {ex.Message}";
			}
			catch (Exception ex) {
				Message = $"接続エラー: {ex.Message}";
			}
		}
	}
}

