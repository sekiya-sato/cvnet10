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

	}
}

