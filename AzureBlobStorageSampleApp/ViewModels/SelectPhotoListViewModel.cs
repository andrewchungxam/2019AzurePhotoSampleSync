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
using System.Linq.Expressions;
using AzureBlobStorageSampleApp.Shared.Models;

namespace AzureBlobStorageSampleApp
{
    public class SelectPhotoListViewModel : BaseViewModel
    {
        #region Fields
        bool _isRefreshing;
        bool _isBusy;
        ICommand _refreshCommand;
        ObservableCollection<PhotoModel> _allPhotosList;
        List<PhotoModel> unsortedPhotosList;
        #endregion

        #region Properties
        public Command LoadItemsCommandWithFilterValue { get; set; }

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

        string _propertyToSort;
        public string PropertyToSort
        {
            get => _propertyToSort;
            set => SetProperty(ref _propertyToSort, value);
        }

        string _valueToSortBy;
        public string ValueToSortBy
        {
            get => _valueToSortBy;
            set => SetProperty(ref _valueToSortBy, value);
        }
        public Command SearchCommand { get; set; }

        #endregion

        public SelectPhotoListViewModel()
        {
            LoadItemsCommandWithFilterValue = new Command<FilterValueModel>(async (FilterValueModel filterValue) => await ExecuteLoadItemsCommandWithFilterValue(filterValue));
        }

        async Task ExecuteLoadItemsCommandWithFilterValue(FilterValueModel filterValue)
        {
            IsRefreshing = true;

            try
            {
                var oneSecondTaskToShowSpinner = Task.Delay(700);

                if (this.IsInternetConnectionActive == true) { 
                    await DatabaseSyncService.SyncRemoteAndLocalDatabases().ConfigureAwait(false);
                }

                unsortedPhotosList = await PhotoDatabase.GetAllPhotos().ConfigureAwait(false);

                var propertyToSort = filterValue.PropertyToSort;
                var valueToSortBy = filterValue.ValueToSortBy;

                IEnumerable<PhotoModel> currentPhotoList;

                switch (propertyToSort) 
                {
                   case nameof(PhotoModel.CityState):
                        if (filterValue.ValueToSortBy == null) { 
                        currentPhotoList = unsortedPhotosList.Where(x => string.IsNullOrEmpty(x.CityState));
                        }
                        else { 
                        valueToSortBy = filterValue.ValueToSortBy;
                        currentPhotoList = unsortedPhotosList.Where(x => x.CityState == $"{valueToSortBy}");
                        }

                        break;
                   case nameof(PhotoModel.BarcodeString):
                        if (filterValue.ValueToSortBy == null) { 
                        currentPhotoList = unsortedPhotosList.Where(x => string.IsNullOrEmpty(x.BarcodeString));
                        }
                        else { 
                        currentPhotoList = unsortedPhotosList.Where(x => !string.IsNullOrEmpty(x.BarcodeString));
                        }

                        break;
                   case nameof(PhotoModel.CreatedAtString):
                        currentPhotoList = unsortedPhotosList.OrderBy(x=>x.CreatedAt).Where(x => x.CreatedAtString == $"{valueToSortBy}" );
                        break;
                    default :
                        currentPhotoList = unsortedPhotosList;
                        break;
                } 
                      
                if(AllPhotosList!=null)
                { 
                AllPhotosList.Clear();
                    foreach (var individualPhotos in currentPhotoList) // currentPhotoList.ToList())
                    {
                        AllPhotosList.Add(individualPhotos);
                    }
                } 
                else
                { 
                    AllPhotosList = new ObservableCollection<PhotoModel>(currentPhotoList);
                }
                    
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
    }
}