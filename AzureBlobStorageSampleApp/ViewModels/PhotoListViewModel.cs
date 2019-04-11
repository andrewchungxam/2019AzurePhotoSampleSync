using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using AsyncAwaitBestPractices.MVVM;
using AzureBlobStorageSampleApp.Shared;
using Xamarin.Forms;
using System.Collections.Generic;

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
                //public Command SearchCommand { get; set; }

        ICommand _searchCommand;


        #endregion

        #region Properties
        public ICommand RefreshCommand => _refreshCommand ??
            (_refreshCommand = new AsyncCommand(ExecuteRefreshCommand, continueOnCapturedContext: false));

        public ICommand SearchCommand => _searchCommand ??
            (_searchCommand = new AsyncCommand(ExecuteSearchCommand, continueOnCapturedContext: false));

        public PhotoListViewModel()
        {
            //SearchCommand = new Command(async () => await ExecuteSearchCommand());
            //SearchCommand = new Command(async () => await ExecuteSearchCommand());

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
                    //AllPhotosList = new ObservableCollection<PhotoModel>(unsortedPhotosList.Where(x=>x.Title.Any()));//.Where(x => x.Title.Contains(this.SearchString)));
                    //return;


                    foreach (var individualPhotos in unsortedPhotosList)
                    {
                        AllPhotosList.Add(individualPhotos);
                    }

                    this.SearchString = "";
                    //var halfSecondSpiner = Task.Delay(1000);
                    //await halfSecondSpiner.ConfigureAwait(false);
                }
                else
                { 
                    //var halfSecondSpiner = Task.Delay(500);
                    //await halfSecondSpiner.ConfigureAwait(false);

                    foreach (var individualPhotos in unsortedPhotosList.Where(x=>x.Title.Contains(this.SearchString)))
                    {
                        AllPhotosList.Add(individualPhotos);
                    }
                }

                //INCORPORATING AZURE SEARCH
                //https://github.com/xamarin/xamarin-forms-samples/blob/master/WebServices/AzureSearch/MonkeyApp/ViewModels/SearchPageViewModel.cs
                //https://docs.microsoft.com/en-us/xamarin/xamarin-forms/data-cloud/search/azure-search

                //ALTERNATIVE WAY WOULD BE TO ASSIGN THE OBSERVABLE COLLECTION TO THE ITEMS PROPERTY IN THE PAGE (AND *NOT* BY ASSIGNING THE OBSERVABLE PROPERTY TO A NEW COLLECTION)

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
//                var oneSecondTaskToShowSpinner = Task.Delay(1000);
                var oneSecondTaskToShowSpinner = Task.Delay(700);
                await oneSecondTaskToShowSpinner.ConfigureAwait(false);



                if (this.IsInternetConnectionActive == true) { 
                //NOT SURE WE NEED THIS ONE IN THE LOCAL ONLY SCNEARIO
                    await DatabaseSyncService.SyncRemoteAndLocalDatabases().ConfigureAwait(false);
                }



                //var unsortedPhotosList = await PhotoDatabase.GetAllPhotos().ConfigureAwait(false);
                unsortedPhotosList = await PhotoDatabase.GetAllPhotos().ConfigureAwait(false);

                //AllPhotosList = new ObservableCollection<PhotoModel>(unsortedPhotosList.OrderBy(x => x.Title));
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
        #endregion
    }
}