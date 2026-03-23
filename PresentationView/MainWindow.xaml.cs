namespace ReferenceApplicationArchitecture.PresentationView;

using System.Windows;
using ReferenceApplicationArchitecture.PresentationModel;
using ReferenceApplicationArchitecture.PresentationViewModel;

public partial class MainWindow : Window
{
    private readonly TaskListViewModel _viewModel;

    public MainWindow(TaskListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    private void OnAddTask(object sender, RoutedEventArgs e)
    {
        if (_viewModel.TryAddTask(TaskTitleBox.Text))
        {
            TaskTitleBox.Clear();
        }
    }

    private void OnCompleteTask(object sender, RoutedEventArgs e)
    {
        if (TaskList.SelectedItem is TaskItem task)
        {
            _viewModel.TryCompleteTask(task.Id);
        }
    }

    private void OnRefresh(object sender, RoutedEventArgs e)
    {
        _viewModel.Refresh();
    }
}
