//using System;
//namespace AzureBlobStorageSampleApp.Pages
//{
//    public class SelectPhotoListViewModel
//    {
//        public SelectPhotoListViewModel()
//        {
//        }
//    }
//}

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

        //public ICommand RefreshCommand => _refreshCommand ??
            //(_refreshCommand = new AsyncCommand(ExecuteRefreshCommand, continueOnCapturedContext: false));



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
//                var oneSecondTaskToShowSpinner = Task.Delay(1000);
                var oneSecondTaskToShowSpinner = Task.Delay(700);


                if (this.IsInternetConnectionActive == true) { 
                //NOT SURE WE NEED THIS ONE IN THE LOCAL ONLY SCNEARIO
                    await DatabaseSyncService.SyncRemoteAndLocalDatabases().ConfigureAwait(false);
                }

                //var unsortedPhotosList = await PhotoDatabase.GetAllPhotos().ConfigureAwait(false);
                unsortedPhotosList = await PhotoDatabase.GetAllPhotos().ConfigureAwait(false);

                    //var filteredBySubjectThenSelectOnlyOneOfEachSubject = filteredBySubject.GroupBy(x => x.LessonNumber).Select(y => y.First()); //ToList();

                    ////foreach (var item in filteredItems)
                    //foreach (var item in filteredBySubjectThenSelectOnlyOneOfEachSubject)
                    //{
                    //    Items.Add(item);
                    //}

      


                //                // Option A, switch
                //switch (whatField) {
                //   case "FieldA": fieldGetter = o => o.FieldA; break;
                //   case "FieldB": fieldGetter = o => o.FieldB; break;
                //   // More options
                //}


               //// Option A, switch
                //switch (variableToSort) {
                //   case "CityState": whichCondition = o.CityState == " => o.FieldA; break;
                //   case "BarcodeString": whichCondition = o => o.FieldB; break;
                //   case "CreatedAtString": whichCondition = o => o.FieldB; break;
                //   // More options
                //}

                //switch (propertyToSort) {
                   //case "CityState":
                   //     Expression<Func<PhotoModel, bool>> whereClause = a => a.CityState == $"{valueToSortBy}";
                   //     break;
                   //case "BarcodeString": 
                   //     Expression<Func<PhotoModel, bool>> whereClause = a => a.CityState == $"{valueToSortBy}";
                   //     break;
                   //case "CreatedAtString":
                   //     Expression<Func<PhotoModel, bool>> whereClause = a => a.CityState == $"{valueToSortBy}";
                   //     break;
                   //// More options
                   /// 
                   /// 
                   /// 
                   /// 


                //var propertyToSort = nameof(PhotoModel.CityState);//"CityState";
                //var valueToSortBy = "NJ";

                //var propertyToSort = this.PropertyToSort;
                //var valueToSortBy = this.ValueToSortBy;

                

                var propertyToSort = filterValue.PropertyToSort;
                var valueToSortBy = filterValue.ValueToSortBy;
                //string valueToSortBy;

                //if (filterValue.ValueToSortBy == null  )
                //    valueToSortBy = "";
                //else
                    //valueToSortBy = filterValue.ValueToSortBy;

                IEnumerable<PhotoModel> currentPhotoList;

                switch (propertyToSort) 
                {
                   case nameof(PhotoModel.CityState):
                        //currentPhotoList = unsortedPhotosList.Where(x => x.CityState == $"{valueToSortBy}");
           

                        if (filterValue.ValueToSortBy == null) { 
                        //valueToSortBy = "";
                        currentPhotoList = unsortedPhotosList.Where(x => string.IsNullOrEmpty(x.CityState));
                        }
                        else { 
                        valueToSortBy = filterValue.ValueToSortBy;
                        currentPhotoList = unsortedPhotosList.Where(x => x.CityState == $"{valueToSortBy}");
                        }


                        //AllPhotosList = new ObservableCollection<PhotoModel>(unsortedPhotosList.Where(x => x.CityState == $"{valueToSortBy}" ));
                        break;
                   case nameof(PhotoModel.BarcodeString):
                        //AllPhotosList = new ObservableCollection<PhotoModel>(unsortedPhotosList.Where(x => x.BarcodeString == $"{valueToSortBy}" ));
                        //currentPhotoList = unsortedPhotosList.Where(x => x.BarcodeString == $"{valueToSortBy}" );
                        if (filterValue.ValueToSortBy == null) { 
                        //valueToSortBy = "";
                        currentPhotoList = unsortedPhotosList.Where(x => string.IsNullOrEmpty(x.BarcodeString));
                        }
                        else { 
                        currentPhotoList = unsortedPhotosList.Where(x => !string.IsNullOrEmpty(x.BarcodeString));
                        }

                        //Expression<Func<PhotoModel, bool>> whereClause = a => a.CityState == $"{valueToSortBy}";
                        break;
                   case nameof(PhotoModel.CreatedAtString):
                        //AllPhotosList = new ObservableCollection<PhotoModel>(unsortedPhotosList.Where(x => x.CreatedAtString == $"{valueToSortBy}" ));
                        currentPhotoList = unsortedPhotosList.Where(x => x.CreatedAtString == $"{valueToSortBy}" );
                        break;
                    default :
                        currentPhotoList = unsortedPhotosList;

                        //Expression<Func<PhotoModel, bool>> whereClause = a => a.CityState == $"{valueToSortBy}";
                        break;
                        // More options
                } 
                      
                if(AllPhotosList!=null)
                { 
                AllPhotosList.Clear();

                   //foreach (var individualPhotos in currentPhotoList.ToList()) // currentPhotoList.ToList())
                    //{
                    //    AllPhotosList.Add(individualPhotos);
                    //}

                    foreach (var individualPhotos in currentPhotoList) // currentPhotoList.ToList())
                    {
                        AllPhotosList.Add(individualPhotos);
                    }
                } 
                else
                { 
                    AllPhotosList = new ObservableCollection<PhotoModel>(currentPhotoList);
                }
                    
                //AllPhotosList = new ObservableCollection<PhotoModel>(unsortedPhotosList.OrderBy(x => x.Title));
                //AllPhotosList = new ObservableCollection<PhotoModel>(unsortedPhotosList.OrderBy(x => x.CreatedAt));
                //AllPhotosList = new ObservableCollection<PhotoModel>(unsortedPhotosList.GroupBy(x => x.CityState).Select(y => y.First()));

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


        #region Methods

//        async Task ExecuteRefreshCommand()
//        {
//            IsRefreshing = true;

//            try
//            {
////                var oneSecondTaskToShowSpinner = Task.Delay(1000);
        //        var oneSecondTaskToShowSpinner = Task.Delay(700);


        //        if (this.IsInternetConnectionActive == true) { 
        //        //NOT SURE WE NEED THIS ONE IN THE LOCAL ONLY SCNEARIO
        //            await DatabaseSyncService.SyncRemoteAndLocalDatabases().ConfigureAwait(false);
        //        }

        //        //var unsortedPhotosList = await PhotoDatabase.GetAllPhotos().ConfigureAwait(false);
        //        unsortedPhotosList = await PhotoDatabase.GetAllPhotos().ConfigureAwait(false);

        //            //var filteredBySubjectThenSelectOnlyOneOfEachSubject = filteredBySubject.GroupBy(x => x.LessonNumber).Select(y => y.First()); //ToList();

        //            ////foreach (var item in filteredItems)
        //            //foreach (var item in filteredBySubjectThenSelectOnlyOneOfEachSubject)
        //            //{
        //            //    Items.Add(item);
        //            //}

      


        //        //                // Option A, switch
        //        //switch (whatField) {
        //        //   case "FieldA": fieldGetter = o => o.FieldA; break;
        //        //   case "FieldB": fieldGetter = o => o.FieldB; break;
        //        //   // More options
        //        //}


        //       //// Option A, switch
        //        //switch (variableToSort) {
        //        //   case "CityState": whichCondition = o.CityState == " => o.FieldA; break;
        //        //   case "BarcodeString": whichCondition = o => o.FieldB; break;
        //        //   case "CreatedAtString": whichCondition = o => o.FieldB; break;
        //        //   // More options
        //        //}

        //        //switch (propertyToSort) {
        //           //case "CityState":
        //           //     Expression<Func<PhotoModel, bool>> whereClause = a => a.CityState == $"{valueToSortBy}";
        //           //     break;
        //           //case "BarcodeString": 
        //           //     Expression<Func<PhotoModel, bool>> whereClause = a => a.CityState == $"{valueToSortBy}";
        //           //     break;
        //           //case "CreatedAtString":
        //           //     Expression<Func<PhotoModel, bool>> whereClause = a => a.CityState == $"{valueToSortBy}";
        //           //     break;
        //           //// More options
        //           /// 
        //           /// 
        //           /// 
        //           /// 


        //        //var propertyToSort = nameof(PhotoModel.CityState);//"CityState";
        //        //var valueToSortBy = "NJ";

        //        var propertyToSort = this.PropertyToSort;
        //        var valueToSortBy = this.ValueToSortBy;

        //        IEnumerable<PhotoModel> currentPhotoList;

        //        switch (propertyToSort) 
        //        {
        //           case nameof(PhotoModel.CityState):
        //                //AllPhotosList.Clear();
        //                currentPhotoList = unsortedPhotosList.Where(x => x.CityState == $"{valueToSortBy}");
           

        //                //AllPhotosList = new ObservableCollection<PhotoModel>(unsortedPhotosList.Where(x => x.CityState == $"{valueToSortBy}" ));
        //                break;
        //           case nameof(PhotoModel.BarcodeString):
        //                //AllPhotosList = new ObservableCollection<PhotoModel>(unsortedPhotosList.Where(x => x.BarcodeString == $"{valueToSortBy}" ));
        //                currentPhotoList = unsortedPhotosList.Where(x => x.BarcodeString == $"{valueToSortBy}" );


        //                //Expression<Func<PhotoModel, bool>> whereClause = a => a.CityState == $"{valueToSortBy}";
        //                break;
        //           case nameof(PhotoModel.CreatedAtString):
        //                //AllPhotosList = new ObservableCollection<PhotoModel>(unsortedPhotosList.Where(x => x.CreatedAtString == $"{valueToSortBy}" ));
        //                currentPhotoList = unsortedPhotosList.Where(x => x.CreatedAtString == $"{valueToSortBy}" );
        //                break;
        //            default :
        //                currentPhotoList = unsortedPhotosList;

        //                //Expression<Func<PhotoModel, bool>> whereClause = a => a.CityState == $"{valueToSortBy}";
        //                break;
        //                // More options
        //        } 
                      
        //        if(AllPhotosList!=null)
        //        { 
        //        AllPhotosList.Clear();

        //           //foreach (var individualPhotos in currentPhotoList.ToList()) // currentPhotoList.ToList())
        //            //{
        //            //    AllPhotosList.Add(individualPhotos);
        //            //}

        //            foreach (var individualPhotos in currentPhotoList) // currentPhotoList.ToList())
        //            {
        //                AllPhotosList.Add(individualPhotos);
        //            }
        //        } 
        //        else
        //        { 
        //            AllPhotosList = new ObservableCollection<PhotoModel>(currentPhotoList);
        //        }
                    
        //        //AllPhotosList = new ObservableCollection<PhotoModel>(unsortedPhotosList.OrderBy(x => x.Title));
        //        //AllPhotosList = new ObservableCollection<PhotoModel>(unsortedPhotosList.OrderBy(x => x.CreatedAt));
        //        //AllPhotosList = new ObservableCollection<PhotoModel>(unsortedPhotosList.GroupBy(x => x.CityState).Select(y => y.First()));

        //        await oneSecondTaskToShowSpinner.ConfigureAwait(false);
        //    }
        //    catch (Exception e)
        //    {
        //        DebugServices.Log(e);
        //    }
        //    finally
        //    {
        //        IsRefreshing = false;
        //    }
        //}
        #endregion
    }
}