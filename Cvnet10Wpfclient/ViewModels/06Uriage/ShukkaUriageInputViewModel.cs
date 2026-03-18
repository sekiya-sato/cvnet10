	using CommunityToolkit.Mvvm.ComponentModel;
	using CommunityToolkit.Mvvm.Input;
	using Cvnet10Base;

	namespace Cvnet10Wpfclient.ViewModels._06Uriage;

	public partial class ShukkaUriageInputViewModel : Helpers.BaseMenteViewModel<Tran00Uriage> {
		int _selectedTabIndex;
		public int SelectedTabIndex {
			get => _selectedTabIndex;
			set => SetProperty(ref _selectedTabIndex, value);
		}

		[RelayCommand]
		void GoToDetailTab() {
			SelectedTabIndex = 1;
		}
	}
