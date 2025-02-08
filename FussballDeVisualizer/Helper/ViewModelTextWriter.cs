using FussballDeVisualizer.ViewModels;
using FussballDeVisualizer.ViewModels.MainWindowTabs;
using System.IO;
using System.Text;
using System.Windows;

public class ViewModelTextWriter : TextWriter
{
    private readonly ConfigTabViewModel _viewModel;

    public ViewModelTextWriter(ConfigTabViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public override void WriteLine(string value)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            _viewModel.ConsoleOutput.Add(value);
        });
    }

    public override void Write(string value)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            _viewModel.ConsoleOutput.Add(value);
        });
    }

    public override Encoding Encoding => Encoding.UTF8;
}