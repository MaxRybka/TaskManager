
using System.Collections.Generic;
using TaskManager.Interfaces;

namespace TaskManager.Models
{
    internal abstract class NavigationModel : INavigationModel
    {
        private readonly IContentOwner _contentOwner;
        private readonly Dictionary<ViewType, INavigatable> _viewsDictionary;

        protected NavigationModel(IContentOwner contentOwner)
        {
            _contentOwner = contentOwner;
            _viewsDictionary = new Dictionary<ViewType, INavigatable>();
        }

        protected IContentOwner ContentOwner
        {
            get { return _contentOwner; }
        }

        protected Dictionary<ViewType, INavigatable> ViewsDictionary
        {
            get { return _viewsDictionary; }
        }

        public void Navigate(ViewType viewType, ProcessModel processModel)
        {
            if (!ViewsDictionary.ContainsKey(viewType) || viewType == ViewType.ProcessInfo)
                InitializeView(viewType, processModel);
            ContentOwner.ContentControl.Content = ViewsDictionary[viewType];
        }

        protected abstract void InitializeView(ViewType viewType, ProcessModel processModel);
    }
}
