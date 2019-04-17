using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using TaskManager.Interfaces;
using TaskManager.Managers;
using TaskManager.Models;
using TaskManager.Tools;

namespace TaskManager.ViewModels
{
    internal class ProcessInfoViewModel : INotifyPropertyChanged
    {
        private ProcessModel CurrentProcess { get; }

        // Collection of all modules in process (Binding to DataGrid).
        private ProcessModuleCollection _modules;
        public ProcessModuleCollection Modules
        {
            get => _modules;
            set
            {
                _modules = value;
                OnPropertyChanged();
            }
        }

        // Collection of all threads in process (Binding to DataGrid).
        private ProcessThreadCollection _threads;
        public ProcessThreadCollection Threads
        {
            get => _threads;
            set
            {
                _threads = value;
                OnPropertyChanged();
            }
        }

        public string ProcessName
        {
            get
            {
                return CurrentProcess.Name;
            }
        }
        public int ProcessId
        {
            get
            {
                return CurrentProcess.Id;
            }
        }

        internal ProcessInfoViewModel(ProcessModel processModel)
        {
            CurrentProcess = processModel;
            try
            {
                Modules = processModel.Process.Modules;
            }
            catch (Win32Exception)
            {
                Modules = null; // Access denied.
            }
            try
            {
                Threads = processModel.Process.Threads;
            }
            catch (Win32Exception)
            {
                Threads = null; // Access denied.
            }
        }

        private RelayCommand<object> _goBackCommand;
        public RelayCommand<object> GoBackCommand
        {
            get
            {
                return _goBackCommand ?? (_goBackCommand = new RelayCommand<object>(
                           o => NavigationManager.Instance.Navigate(ViewType.ProcessList, null)));
            }
        }

        private RelayCommand<object> _openFolderCommand;
        public RelayCommand<object> OpenFolderCommand
        {
            get
            {
                return _openFolderCommand ?? (_openFolderCommand = new RelayCommand<object>(
                           o =>
                           {
                               if (CurrentProcess.Folder == "unavailable")
                                   MessageBox.Show("The folder is unknown");
                               else
                                   Process.Start(Path.GetDirectoryName(CurrentProcess.Folder));
                           }));
            }
        }

        private RelayCommand<object> _closeProcessCommand;
        public RelayCommand<object> CloseProcessCommand
        {
            get
            {
                return _closeProcessCommand ?? (_closeProcessCommand = new RelayCommand<object>(
                           o =>
                           {
                               try
                               {
                                   CurrentProcess.Process.Kill();
                                   MessageBox.Show(CurrentProcess.Name + " was closed !");
                                   NavigationManager.Instance.Navigate(ViewType.ProcessList, null);
                               }
                               catch
                               {
                                   MessageBox.Show("We can't close this process");
                               }
                           }));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
