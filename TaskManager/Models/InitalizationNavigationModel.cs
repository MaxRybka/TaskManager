using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Interfaces;
using TaskManager.Views;

namespace TaskManager.Models
{
    internal class InitializationNavigationModel : NavigationModel
    {
        public InitializationNavigationModel(IContentOwner contentOwner) : base(contentOwner)
        {
        }

        protected override void InitializeView(ViewType viewType, ProcessModel processModel)
        {
            switch (viewType)
            {
                case ViewType.ProcessList:
                    ViewsDictionary.Add(viewType, new ProcessList());
                    break;
                case ViewType.ProcessInfo:
                    if (ViewsDictionary.ContainsKey(viewType))
                        ViewsDictionary[viewType] = new ProcessInfoUC(processModel);
                    else
                        ViewsDictionary.Add(viewType, new ProcessInfoUC(processModel));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(viewType), viewType, null);
            }
        }
        
    }
}
