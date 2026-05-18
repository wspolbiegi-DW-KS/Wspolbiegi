using System;
using System.Windows;
using TP.ConcurrentProgramming.Presentation.ViewModel;

namespace TP.ConcurrentProgramming.PresentationView
{
    /// <summary>
    /// View implementation
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Raises the <seealso cref="System.Windows.Window.Closed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnClosed(EventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
                viewModel.Dispose();
            base.OnClosed(e);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            //int,TryParse to zabezpieczenie przed wpisaniem litery
            if (int.TryParse(BallCountInput.Text, out int count) && count > 0)
            {
                MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
                viewModel.Restart(count);
            }
            else
            {
                MessageBox.Show("Podaj poprawną liczbę kul.");
            }
        }
    }
}