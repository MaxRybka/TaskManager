using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Interfaces;
using TaskManager.Managers;
using TaskManager.Models;

namespace TaskManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IContentOwner
    {
        public static event Action StopThreads;
        public ContentControl ContentControl
        {
            get { return _contentControl; }
        }

        public MainWindow()
        {
            InitializeComponent();

            NavigationManager.Instance.Initialize(new InitializationNavigationModel(this));
            NavigationManager.Instance.Navigate(ViewType.ProcessList, null);
        }

        // Close all Threads after completion of the program.
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            StopThreads?.Invoke();
            Environment.Exit(1);
        }
    }
}
