
using System.Windows.Controls;

using TaskManager.ViewModels;

namespace TaskManager.Views
{
    /// <summary>
    /// Логика взаимодействия для ProcessList.xaml
    /// </summary>
    public partial class ProcessList : UserControl, Interfaces.INavigatable
    {
        public ProcessList()
        {
            InitializeComponent();
            DataContext = new ProcessListViewModel();
        }
    }
}
