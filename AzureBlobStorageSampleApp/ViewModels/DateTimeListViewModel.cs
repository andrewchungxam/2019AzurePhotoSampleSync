using System;

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
    public class DateTimeListViewModel : BaseViewModel
    {
        #region Fields
        bool _isRefreshing;
        bool _isBusy;
        ICommand _refreshCommand;
        ICommand _checkInternetConnectionCommand;

        ObservableCollection<PhotoModel> _allPhotosList;
        List<PhotoModel> unsortedPhotosList;
        #endregion

        #region Properties
        public ICommand RefreshCommand => _refreshCommand ??
            (_refreshCommand = new AsyncCommand(ExecuteRefreshCommand, continueOnCapturedContext: false));

        public ICommand CheckInternetConnectionCommand => _checkInternetConnectionCommand ??
            (_checkInternetConnectionCommand = new Command(ExecuteCheckInternetConnectionCommand)); //, continueOnCapturedContext: false));


        public DateTimeListViewModel()
        {
            //SearchCommand = new Command(() => ExecuteSearchCommand());

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

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public Command SearchCommand { get; set; }

        #endregion

        #region Methods

        async Task ExecuteRefreshCommand()
        {
            IsRefreshing = true;

            try
            {
//                var oneSecondTaskToShowSpinner = Task.Delay(1000);
                var oneSecondTaskToShowSpinner = Task.Delay(700);


                if (this.IsInternetConnectionActive == true) { 
                //NOT SURE WE NEED THIS ONE IN THE LOCAL ONLY SCNEARIO
                    await DatabaseSyncService.SyncRemoteAndLocalDatabases().ConfigureAwait(false);
                }

                unsortedPhotosList = await PhotoDatabase.GetAllPhotos().ConfigureAwait(false);

                //AllPhotosList = new ObservableCollection<PhotoModel>(unsortedPhotosList.GroupBy(x => x.CityState).Select(y => y.First()));

                AllPhotosList = new ObservableCollection<PhotoModel>(unsortedPhotosList.GroupBy(x => x.CreatedAtString).Select(y => y.First()).OrderBy(x=>x.CreatedAtString));

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
