﻿using System;

using System;

using Xamarin.Forms;

using AzureBlobStorageSampleApp.Shared;
using AzureBlobStorageSampleApp.Mobile.Shared;
using Xamarin.Essentials;
using AzureBlobStorageSampleApp.ViewModels;
using AzureBlobStorageSampleApp.Shared.Models;

namespace AzureBlobStorageSampleApp.Pages
{
    public class GeographyListPage : BaseContentPage<GeographyListViewModel>
    {
        #region Constant Fields
        readonly ListView _geographyListView;

        readonly string _propertyToSort;
        #endregion

        #region Constructors
        public GeographyListPage()
        {

            _propertyToSort = nameof(PhotoModel.CityState);

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            { 
                ViewModel.IsInternetConnectionActive = true;
            }
            else 
            { 
               ViewModel.IsInternetConnectionActive = false;
            }

            _geographyListView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemTemplate = new DataTemplate(typeof(GeoViewCell)),
                IsPullToRefreshEnabled = true,
                BackgroundColor = Color.Transparent,
                AutomationId = AutomationIdConstants.PhotoListView,
                SeparatorVisibility = SeparatorVisibility.None
            };
            _geographyListView.ItemSelected += HandleItemSelected;
            _geographyListView.SetBinding(ListView.IsRefreshingProperty, nameof(ViewModel.IsRefreshing));
            _geographyListView.SetBinding(ListView.ItemsSourceProperty, nameof(ViewModel.AllPhotosList));
            _geographyListView.SetBinding(ListView.RefreshCommandProperty, nameof(ViewModel.RefreshCommand));

            //#TODO - modifying size of cells
            _geographyListView.HasUnevenRows = true;




            Title = PageTitles.LocationPage;

            var stackLayout = new StackLayout();
            stackLayout.Children.Add(_geographyListView);


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

            Device.BeginInvokeOnMainThread(_geographyListView.BeginRefresh);
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
                            ValueToSortBy = selectedPhoto.CityState,

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
