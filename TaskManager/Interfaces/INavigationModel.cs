
using TaskManager.Models;

namespace TaskManager.Interfaces
{

    internal enum ViewType
    {
        ProcessList,
        ProcessInfo
    }

    interface INavigationModel
    {
        void Navigate(ViewType viewType, ProcessModel processModel);
    }

}
