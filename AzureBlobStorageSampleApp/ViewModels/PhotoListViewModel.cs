using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using AsyncAwaitBestPractices.MVVM;
using AzureBlobStorageSampleApp.Shared;
using Xamarin.Forms;
using System.Collections.Generic;
using Xamarin.Essentials;

namespace AzureBlobStorageSampleApp
{
    public class PhotoListViewModel : BaseViewModel
    {
        #region Fields
        bool _isRefreshing;
        bool _isBusy;
        ICommand _refreshCommand;
        ObservableCollection<PhotoModel> _allPhotosList;
        String _searchString;
        List<PhotoModel> unsortedPhotosList;

        ICommand _searchCommand;
        ICommand _checkInternetConnectionCommand;

        #endregion

        #region Properties
        public ICommand RefreshCommand => _refreshCommand ??
            (_refreshCommand = new AsyncCommand(ExecuteRefreshCommand, continueOnCapturedContext: false));

        public ICommand SearchCommand => _searchCommand ??
            (_searchCommand = new AsyncCommand(ExecuteSearchCommand, continueOnCapturedContext: false));

        public ICommand CheckInternetConnectionCommand => _checkInternetConnectionCommand ??
            (_checkInternetConnectionCommand = new Command(ExecuteCheckInternetConnectionCommand)); //, continueOnCapturedContext: false));

 

        public PhotoListViewModel()
        {
        }

        public ObservableCollection<PhotoModel> AllPhotosList
        {
            get => _allPhotosList;
            set => SetProperty(ref _allPhotosList, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public String SearchString
        {
            get => _searchString;
            set => SetProperty(ref _searchString, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }


        #endregion

        #region Methods
        //async void ExecuteSearchCommand()
        async Task ExecuteSearchCommand()
        {   
            if(IsBusy)
                return;
            IsBusy = true;

            AllPhotosList.Clear();

             try
            {
                if(this.SearchString==" ")
                { 
                    foreach (var individualPhotos in unsortedPhotosList)
                    {
                        AllPhotosList.Add(individualPhotos);
                    }

                    this.SearchString = "";
                }
                else
                { 
                    foreach (var individualPhotos in unsortedPhotosList.Where(x=>x.Title.Contains(this.SearchString)))
                    {
                        AllPhotosList.Add(individualPhotos);
                    }
                }

                //INCORPORATING AZURE SEARCH
                //https://github.com/xamarin/xamarin-forms-samples/blob/master/WebServices/AzureSearch/MonkeyApp/ViewModels/SearchPageViewModel.cs
                //https://docs.microsoft.com/en-us/xamarin/xamarin-forms/data-cloud/search/azure-search

                //ALTERNATIVE WAY WOULD BE TO ASSIGN THE OBSERVABLE COLLECTION TO THE ITEMS PROPERTY IN THE PAGE ( *NOT* BY ASSIGNING THE OBSERVABLE PROPERTY TO A NEW COLLECTION)

            }   
            catch (Exception e)
            {
                DebugServices.Log(e);
            }        
            finally
            { 
                IsBusy = false;
            }
        }

        async Task ExecuteRefreshCommand()
        {
            IsRefreshing = true;

            try
            {
                var oneSecondTaskToShowSpinner = Task.Delay(700);
                await oneSecondTaskToShowSpinner.ConfigureAwait(false);

                if (this.IsInternetConnectionActive == true) { 
                    await DatabaseSyncService.SyncRemoteAndLocalDatabases().ConfigureAwait(false);
                }

                unsortedPhotosList = await PhotoDatabase.GetAllPhotos().ConfigureAwait(false);

                AllPhotosList = new ObservableCollection<PhotoModel>(unsortedPhotosList.OrderBy(x => x.CreatedAt));

                await oneSecondTaskToShowSpinner.ConfigureAwait(false);
            }
            catch (Exception e)
            {
                DebugServices.Log(e);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

       private void ExecuteCheckInternetConnectionCommand()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            { 
                this.IsInternetConnectionActive = true;
            }
            else
            { 
                this.IsInternetConnectionActive = false;
            }
        }
        #endregion
    }
}