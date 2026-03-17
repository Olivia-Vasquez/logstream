using LogStream.Maui.ViewModels;

namespace LogStream.Maui.Views;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

	// Apply filter whenever the filter text changes
	private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
	{
		if (BindingContext is MainPageViewModel vm)
		{
			vm.ApplyFilterCommand.Execute(null);
		}
	}
}
