using System.Windows;
using RepeatableTimer.ViewModels;

namespace RepeatableTimer;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}
