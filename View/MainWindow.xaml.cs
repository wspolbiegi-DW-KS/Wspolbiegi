namespace ReferenceApplicationArchitecture.View;

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows;
using ReferenceApplicationArchitecture.ViewModel;

public partial class MainWindow : Window
{
    private readonly BilliardTableViewModel _viewModel;
    private readonly DispatcherTimer _timer;

    public MainWindow(BilliardTableViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };
        _timer.Tick += (_, _) =>
        {
            _viewModel.Step(0.016);
            RedrawBalls();
        };

        Loaded += (_, _) => RedrawBalls();
    }

    private void OnStartSimulation(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(BallCountBox.Text, out var ballCount))
        {
            ErrorText.Text = "Podaj poprawna liczbe kul.";
            return;
        }

        var width = Math.Max(120, TableCanvas.ActualWidth);
        var height = Math.Max(120, TableCanvas.ActualHeight);

        if (_viewModel.TryInitialize(ballCount, width, height))
        {
            _timer.Start();
            RedrawBalls();
            ErrorText.Text = string.Empty;
            return;
        }

        ErrorText.Text = _viewModel.LastError;
    }

    private void OnStopSimulation(object sender, RoutedEventArgs e)
    {
        _timer.Stop();
    }

    private void RedrawBalls()
    {
        TableCanvas.Children.Clear();
        foreach (var ball in _viewModel.Balls)
        {
            var ellipse = new Ellipse
            {
                Width = 2 * ball.Radius,
                Height = 2 * ball.Radius,
                Fill = Brushes.OrangeRed,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            Canvas.SetLeft(ellipse, ball.X - ball.Radius);
            Canvas.SetTop(ellipse, ball.Y - ball.Radius);
            TableCanvas.Children.Add(ellipse);
        }
    }
}
