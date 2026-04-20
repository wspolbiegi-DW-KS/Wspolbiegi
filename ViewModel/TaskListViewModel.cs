namespace ReferenceApplicationArchitecture.ViewModel;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReferenceApplicationArchitecture.Logic;
using ReferenceApplicationArchitecture.Model;

/// <summary>
/// ViewModel exposing billiard simulation operations to the WPF view.
/// </summary>
public class BilliardTableViewModel : INotifyPropertyChanged
{
    private readonly IBilliardService _service;
    private string? _lastError;

    public BilliardTableViewModel(IBilliardService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        Balls = new ObservableCollection<Ball>();
    }

    public ObservableCollection<Ball> Balls { get; }

    public string? LastError
    {
        get => _lastError;
        private set
        {
            if (_lastError != value)
            {
                _lastError = value;
                OnPropertyChanged();
            }
        }
    }

    public bool TryInitialize(int ballCount, double tableWidth, double tableHeight)
    {
        try
        {
            _service.Initialize(ballCount, tableWidth, tableHeight);
            Refresh();
            LastError = null;
            return true;
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            return false;
        }
    }

    public void Step(double deltaTimeSeconds)
    {
        _service.Step(deltaTimeSeconds);
        Refresh();
    }

    private void Refresh()
    {
        var balls = _service.GetBalls();
        Balls.Clear();
        foreach (var ball in balls)
        {
            Balls.Add(new Ball(ball.Id, ball.X, ball.Y, ball.Radius));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
