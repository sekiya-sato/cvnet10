using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cvnet10Wpfclient.ViewServices;

namespace Cvnet10Wpfclient.ViewModels.Sub;

public partial class SelectCodeViewModel : Helpers.BaseViewModel {
	[ObservableProperty]
	SelectCodeParameter parameter = new();

	public void Initialize(SelectCodeParameter? param) {
		Parameter = param ?? new();
	}

	[RelayCommand]
	void Ok() {
		ClientLib.ExitDialogResult(this, true);
	}

}
