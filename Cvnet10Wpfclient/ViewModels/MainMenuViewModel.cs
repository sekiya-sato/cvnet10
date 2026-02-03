using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cvnet8client.Views.Sub;
using Cvnet10Wpfclient.Models;
using Cvnet10Wpfclient.Util;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Cvnet10Wpfclient.ViewModels;

public partial class MainMenuViewModel : ObservableObject {

    [ObservableProperty]
    ObservableCollection<MenuData> menuItems = new();

    [ObservableProperty]
    private MenuData? selectedMenu;

	[ObservableProperty]
	private string? statusMessage;

	[ObservableProperty]
	private string? expireDate;

	[ObservableProperty]
	private string headerTitle = "Creative Vision.net 10";

	[ObservableProperty]
	private string subTitle = "Creative Vision gRPC Module";


	[ObservableProperty]
	private bool isMenuReady;

    [RelayCommand]
    private void Init() {
        if (IsMenuReady) {
            return;
        }

        MenuItems = MenuData.CreateDefault();
        StatusMessage = "メニューを選択してください。";
        ExpireDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
        IsMenuReady = true;
    }

    [RelayCommand]
    private void Exit() {
        if (MessageEx.ShowQuestionDialog("終了しますか？", owner: ClientLib.GetActiveView(this)) == MessageBoxResult.Yes) {
            ClientLib.Exit(this);
        }
    }

    [RelayCommand]
    private void Minimize() {
        var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
        if (window != null) {
            window.WindowState = WindowState.Minimized;
        }
    }
	[RelayCommand]
	private void Maximize() {
		var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
		if (window != null) {
            if(window.WindowState == WindowState.Maximized) 
                window.WindowState = WindowState.Normal;
            else 
                window.WindowState = WindowState.Maximized;
		}
	}

	[RelayCommand]
    private void SelectMenu(MenuData? menu) {
        if (menu == null) {
            return;
        }

        SelectedMenu = menu;
        StatusMessage = menu.Header;

        if (menu.SubItems != null && menu.SubItems.Count > 0 && menu.ViewType == null) {
            return;
        }

        if (menu.ViewType == null || !typeof(Window).IsAssignableFrom(menu.ViewType)) {
            return;
        }

        var view = Activator.CreateInstance(menu.ViewType) as Window;
        if (view == null) {
            StatusMessage = "画面を生成できませんでした。";
            return;
        }

        view.Title = menu.Header ?? view.Title;
        var dialogResult = ClientLib.ShowDialogView(view, this, IsDialog: menu.IsDialog);
        StatusMessage = dialogResult == true
            ? $"{menu.Header} を完了しました。"
            : $"{menu.Header} を終了しました。";
    }
}
