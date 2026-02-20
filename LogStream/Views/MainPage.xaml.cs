using LogStream.ViewModels;

namespace LogStream;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
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
