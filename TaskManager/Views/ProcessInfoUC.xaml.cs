using System.Windows.Controls;
using TaskManager.Interfaces;
using TaskManager.Models;
using TaskManager.ViewModels;

namespace TaskManager.Views
{
    /// <summary>
    /// Логика взаимодействия для ProcessInfoUC.xaml
    /// </summary>
    public partial class ProcessInfoUC : UserControl, INavigatable
    {
        public ProcessInfoUC(ProcessModel processModel)
        {
            InitializeComponent();
            DataContext = new ProcessInfoViewModel(processModel);
        }
    }
}
