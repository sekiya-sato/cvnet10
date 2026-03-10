using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cvnet10Wpfclient.ViewServices;

namespace Cvnet10Wpfclient.ViewModels.Sub;

public partial class SelectParamViewModel : Helpers.BaseViewModel {
	[ObservableProperty]
	SelectParameter parameter = new();

	public void Initialize(SelectParameter? param) {
		Parameter = param ?? new();
	}

	[RelayCommand]
	void Ok() {
		ClientLib.ExitDialogResult(this, true);
	}

}
