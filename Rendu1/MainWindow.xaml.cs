using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rendu1;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Person p1 = new Person("guillaume", 20);
        DataContext = p1;
        
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Txt1.Text = ("Oui tu es beau");
    }
}