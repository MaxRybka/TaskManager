using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TaskManager.Interfaces;
using TaskManager.Managers;
using TaskManager.Models;
using TaskManager.Tools;

namespace TaskManager.ViewModels
{
    internal class ProcessListViewModel : INotifyPropertyChanged
    {
        // Collection of all processes (Binding to DataGrid).
        private ObservableCollection<ProcessModel> _processes;
        public ObservableCollection<ProcessModel> Processes
        {
            get
            {
                return _processes;
            }
            set
            {
                _processes = value;
                OnPropertyChanged();
            }
        }

        internal ProcessListViewModel()
        {
            Processes = new ObservableCollection<ProcessModel>();
            SortBy = 0;

            new Thread(UpdateCollection).Start();
            new Thread(UpdateMetadata).Start();
        }

        private ProcessModel _selectedProcess;
        public ProcessModel SelectedProcess
        {
            get
            {
                return _selectedProcess;
            }
            set
            {
                _selectedProcess = value;
                OnPropertyChanged();
            }
        }

        private int _sortBy;
        public int SortBy
        {
            get
            {
                return _sortBy;
            }
            set
            {
                SortProcesses(_sortBy = value, Processes);
            }
        }

        private async void SortProcesses(int sortBy, ObservableCollection<ProcessModel> collection)
        {
            ObservableCollection<ProcessModel> newProcesses = null;
            switch (sortBy)
            {
                case 0:
                    Processes = collection;
                    return;
                case 1:
                    await Task.Run(() =>
                        newProcesses =
                            new ObservableCollection<ProcessModel>(collection.OrderBy(i => i.Name)));
                    break;
                case 2:
                    await Task.Run(() =>
                        newProcesses =
                            new ObservableCollection<ProcessModel>(collection.OrderBy(i => i.Id)));
                    break;
                case 3:
                    await Task.Run(() =>
                        newProcesses =
                            new ObservableCollection<ProcessModel>(collection.OrderBy(i => i.Active)));
                    break;
                case 4:
                    await Task.Run(() =>
                        newProcesses =
                            new ObservableCollection<ProcessModel>(collection.OrderBy(i => i.CPU)));
                    break;
                case 5:
                    await Task.Run(() =>
                        newProcesses =
                            new ObservableCollection<ProcessModel>(collection.OrderBy(i => i.RAMinPercents)));
                    break;
                case 6:
                    await Task.Run(() =>
                        newProcesses =
                            new ObservableCollection<ProcessModel>(collection.OrderBy(i => i.RAMinKB)));
                    break;
                case 7:
                    await Task.Run(() =>
                        newProcesses =
                            new ObservableCollection<ProcessModel>(collection.OrderBy(i => i.Streams)));
                    break;
                case 8:
                    await Task.Run(() =>
                        newProcesses =
                            new ObservableCollection<ProcessModel>(collection.OrderBy(i => i.Handles)));
                    break;
                case 9:
                    await Task.Run(() =>
                        newProcesses =
                            new ObservableCollection<ProcessModel>(collection.OrderBy(i => i.Folder)));
                    break;
                case 10:
                    await Task.Run(() =>
                        newProcesses =
                            new ObservableCollection<ProcessModel>(collection.OrderBy(i => i.StartTime)));
                    break;
            }
            Processes = newProcesses;
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
                                   SelectedProcess.Process.Kill();
                                   MessageBox.Show(SelectedProcess.Name + " was closed !");
                                   NavigationManager.Instance.Navigate(ViewType.ProcessList, null);
                               }
                               catch
                               {
                                   MessageBox.Show("We can't close this process");
                               }
                           }));
            }
        }

        private RelayCommand<object> _goToProcessInfoCommand;
        public RelayCommand<object> GoToProcessInfoCommand
        {
            get
            {
                return _goToProcessInfoCommand ?? (_goToProcessInfoCommand = new RelayCommand<object>(
                           o => NavigationManager.Instance.Navigate(ViewType.ProcessInfo, SelectedProcess)));
            }
        }

        // Update metadata of all processes every 2 seconds in another thread.
        private void UpdateMetadata()
        {
            while (true)
            {
                foreach (ProcessModel pr in Processes)
                    pr.Update();
                Processes = new ObservableCollection<ProcessModel>(Processes);
                Thread.Sleep(2000);
            }
        }

        // Update collection of processes every 5 seconds in another thread.
        private void UpdateCollection()
        {
            while (true)
            {
                List<ProcessModel> currentProcessModels = Processes.ToList();
                Process[] newProcesses = Process.GetProcesses();
                List<ProcessModel> newProcessModels =
                    (from pr in newProcesses select new ProcessModel(pr)).ToList();

                for (int i = currentProcessModels.Count - 1; i >= 0; --i)
                    if (!newProcessModels.Contains(currentProcessModels[i]))
                        currentProcessModels.RemoveAt(i);

                foreach (ProcessModel pr in newProcessModels)
                    if (!currentProcessModels.Contains(pr))
                        currentProcessModels.Add(pr);

                SortProcesses(SortBy, new ObservableCollection<ProcessModel>(currentProcessModels));

                Thread.Sleep(5000);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
