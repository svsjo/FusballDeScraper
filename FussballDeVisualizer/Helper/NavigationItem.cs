using FussballDeVisualizer.Helper;

public class NavigationItem
{
    public string Title { get; }
    public BaseViewModel ViewModel { get; }

    public NavigationItem(string title, BaseViewModel viewModel)
    {
        Title = title;
        ViewModel = viewModel;
    }
}