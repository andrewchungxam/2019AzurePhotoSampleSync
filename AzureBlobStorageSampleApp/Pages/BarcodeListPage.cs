//using System;
//using Xamarin.Forms;
//namespace AzureBlobStorageSampleApp.Pages
//{
//    public class BarcodeListPage : ContentPage
//    {
//        public BarcodeListPage()
//        {
//        }
//    }
//}


////using System;
////using Xamarin.Forms;
////namespace AzureBlobStorageSampleApp
////{
////    public class DateTimeListPage : ContentPage
////    {
////        public DateTimeListPage()
////        {
////        }
////    }
////}

using System;

using System;

using Xamarin.Forms;

using AzureBlobStorageSampleApp.Shared;
using AzureBlobStorageSampleApp.Mobile.Shared;
using Xamarin.Essentials;
using AzureBlobStorageSampleApp.ViewModels;
using AzureBlobStorageSampleApp.Shared.Models;

namespace AzureBlobStorageSampleApp
{
    public class BarcodeListPage : BaseContentPage<BarcodeListViewModel>
    {
        #region Constant Fields
        readonly ListView _photosListView;

        readonly string _propertyToSort;
        #endregion

        #region Constructors
        public BarcodeListPage()
        {

            _propertyToSort = nameof(PhotoModel.BarcodeString);

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            { 
                ViewModel.IsInternetConnectionActive = true;
            }
            else 
            { 
               ViewModel.IsInternetConnectionActive = false;
            }

            _photosListView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemTemplate = new DataTemplate(typeof(IsBarcodeOrNotBarcodeViewCell)),
                IsPullToRefreshEnabled = true,
                BackgroundColor = Color.Transparent,
                AutomationId = AutomationIdConstants.PhotoListView,
                SeparatorVisibility = SeparatorVisibility.None
            };
            _photosListView.ItemSelected += HandleItemSelected;
            _photosListView.SetBinding(ListView.IsRefreshingProperty, nameof(ViewModel.IsRefreshing));
            _photosListView.SetBinding(ListView.ItemsSourceProperty, nameof(ViewModel.AllPhotosList));
            _photosListView.SetBinding(ListView.RefreshCommandProperty, nameof(ViewModel.RefreshCommand));

            //#TODO - modifying size of cells
            _photosListView.HasUnevenRows = true;




            Title = PageTitles.BarcodePage;

            var stackLayout = new StackLayout();
            stackLayout.Children.Add(_photosListView);


            var relativeLayout = new RelativeLayout();
            //relativeLayout.Children.Add(searchBar, _photosListView,

            relativeLayout.Children.Add(stackLayout,
                                       Constraint.Constant(0),
                                       Constraint.Constant(0),
                                       Constraint.RelativeToParent(parent => parent.Width),
                                       Constraint.RelativeToParent(parent => parent.Height));

            Content = relativeLayout;
        }

        #endregion

        #region Methods
        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.CheckInternetConnectionCommand.Execute(null);
            Device.BeginInvokeOnMainThread(_photosListView.BeginRefresh);
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

//            if ((ViewModel.AllPhotosList != null) && (ViewModel.AllPhotosList.Count == 0))
////                ViewModel.LoadItemsCommand.Execute(null);
                //ViewModel.RefreshCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;

        }

        void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            { 
                ViewModel.IsInternetConnectionActive = true;
            }
            else
            { 
                ViewModel.IsInternetConnectionActive = false;
            }

        }


        void HandleItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            { 
                var listView = sender as ListView;
                var selectedPhoto = e?.SelectedItem as PhotoModel;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (selectedPhoto != null)
                    {
                        //SWITCH AFER TEST - KEEP THIS FOR NOW
//                      await Navigation.PushAsync(new PhotoDetailsPage(selectedPhoto));
//                        await Navigation.PushAsync(new SelectedPhotoListPage(selectedPhoto));

                        var filterValueModel = new FilterValueModel() 
                        { 
                            PropertyToSort = _propertyToSort,
                            ValueToSortBy = selectedPhoto.BarcodeString,

                        };

                        await Navigation.PushAsync(new SelectPhotoListPage(filterValueModel));
  
                        listView.SelectedItem = null;
                    }
                });
            } else
            { 
                DisplayAlert("Please connect to the Internet ", string.Empty, "OK");
            }

        }

        //void HandleAddContactButtonClicked(object sender, EventArgs e) =>
            //Device.BeginInvokeOnMainThread(async () => await Navigation.PushModalAsync(new BaseNavigationPage(new AddPhotoPage())));
        #endregion
    }
}

