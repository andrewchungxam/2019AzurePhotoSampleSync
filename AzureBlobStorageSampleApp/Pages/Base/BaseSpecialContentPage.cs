using System;
using Xamarin.Forms;

namespace AzureBlobStorageSampleApp
{
    public class BaseSpecialContentPageWithPublicViewModel<T> : SpecialContentPage where T : BaseViewModel, new()
    {
        protected BaseSpecialContentPageWithPublicViewModel()
        {
            BindingContext = ViewModel;
            BackgroundColor = ColorConstants.PageBackgroundColor;
            this.SetBinding(IsBusyProperty, nameof(ViewModel.IsInternetConnectionActive));
        }

        public T ViewModel { get; } = new T();
    }
}
