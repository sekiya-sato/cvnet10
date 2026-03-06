using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cvnet10Wpfclient.ViewServices;

namespace Cvnet10Wpfclient.ViewModels.Sub;

public partial class SelectShainViewModel : Helpers.BaseViewModel {
	[ObservableProperty]
	string fromId = string.Empty;

	[ObservableProperty]
	string toId = string.Empty;

	[ObservableProperty]
	string fromCode = string.Empty;

	[ObservableProperty]
	string toCode = string.Empty;

	public void Initialize(string? fromId, string? toId, string? fromCode, string? toCode) {
		FromId = fromId?.Trim() ?? string.Empty;
		ToId = toId?.Trim() ?? string.Empty;
		FromCode = fromCode?.Trim() ?? string.Empty;
		ToCode = toCode?.Trim() ?? "99999999";
	}

	[RelayCommand]
	void Ok() {
		ClientLib.ExitDialogResult(this, true);
	}

}
