using System;
using System.IO;
using System.Windows.Input;
using System.Threading.Tasks;

using Xamarin.Forms;

using Plugin.Media;
using Plugin.Media.Abstractions;

using AzureBlobStorageSampleApp.Shared;
using AzureBlobStorageSampleApp.Mobile.Shared;
using AzureBlobStorageSampleApp.Services;



using AsyncAwaitBestPractices.MVVM;
using AsyncAwaitBestPractices;
using Xamarin.Essentials;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using ZXing;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
//using ScannerHelperLibrary;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Cognitive.CustomVision.Prediction.Models;
using Microsoft.Cognitive.CustomVision.Prediction;
using System.Globalization;
using FFImageLoading.Helpers.Exif;

namespace AzureBlobStorageSampleApp
{


    public class AddPhotoViewModel : BaseViewModel
    {
        #region Constant Fields
        readonly WeakEventManager _noCameraFoundEventManager = new WeakEventManager();
        readonly WeakEventManager _savePhotoCompletedEventManager = new WeakEventManager();
        readonly WeakEventManager<string> _savePhotoFailedEventManager = new WeakEventManager<string>();
        readonly WeakEventManager _noCameraPickerFoundEventManager = new WeakEventManager();

        #endregion

        #region Fields
        ICommand _savePhotoCommand, _takePhotoCommand, _takeScanCommand;
        ICommand _getGeoLocationCommand;
        ICommand _getPhotoCommand;

        string _photoTitle, _pageTitle = PageTitles.AddPhotoPage;
        bool _isPhotoSaving;
        ImageSource _photoImageSource;
        PhotoBlobModel _photoBlob;
        ImageAnalysis analysis;
        #endregion

        #region Events
        public event EventHandler NoCameraFound
        {
            add => _noCameraFoundEventManager.AddEventHandler(value);
            remove => _noCameraFoundEventManager.RemoveEventHandler(value);
        }

        public event EventHandler NoCameraPickerFound
        {
            add => _noCameraPickerFoundEventManager.AddEventHandler(value);
            remove => _noCameraPickerFoundEventManager.RemoveEventHandler(value);
        }

        public event EventHandler SavePhotoCompleted
        {
            add => _savePhotoCompletedEventManager.AddEventHandler(value);
            remove => _savePhotoCompletedEventManager.RemoveEventHandler(value);
        }

        public event EventHandler<string> SavePhotoFailed
        {
            add => _savePhotoFailedEventManager.AddEventHandler(value);
            remove => _savePhotoFailedEventManager.RemoveEventHandler(value);
        }
        #endregion

        #region Properties
        public ICommand TakePhotoCommand => _takePhotoCommand ??
            (_takePhotoCommand = new AsyncCommand(ExecuteTakePhotoCommand, continueOnCapturedContext: false));

        public ICommand GetPhotoCommand => _getPhotoCommand ??
            (_getPhotoCommand = new AsyncCommand(ExecuteGetPhotoCommand, continueOnCapturedContext: false));

        public ICommand TakeScanCommand => _takeScanCommand ??
            (_takeScanCommand = new AsyncCommand(ExecuteTakeScanCommand, continueOnCapturedContext: false));


        public ICommand SavePhotoCommand => _savePhotoCommand ??
            (_savePhotoCommand = new AsyncCommand(() => ExecuteSavePhotoCommand(PhotoBlob, PhotoTitle), continueOnCapturedContext: false));


        public ICommand GetGeoLocationCommand => _getGeoLocationCommand ??
            (_getGeoLocationCommand = new AsyncCommand(ExecuteGetGeoLocationCommand, continueOnCapturedContext: false));




        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
        }

        public bool IsPhotoSaving
        {
            get => _isPhotoSaving;
            set => SetProperty(ref _isPhotoSaving, value);
        }

        public string PhotoTitle
        {
            get => _photoTitle;
            set => SetProperty(ref _photoTitle, value, UpdatePageTilte);
        }

        public ImageSource PhotoImageSource
        {
            get => _photoImageSource;
            set => SetProperty(ref _photoImageSource, value);
        }

        PhotoBlobModel PhotoBlob
        {
            get => _photoBlob;
            set => SetProperty(ref _photoBlob, value, UpdatePhotoImageSource);
        }

        string _geoString;
        string _generalCognitiveServices;
        string _entitiesCognitiveServices;

        public string GeoString
        {
            get => _geoString;
            set => SetProperty(ref _geoString, value);
        }

        public string GeneralCognitiveServices
        {
            get => _generalCognitiveServices;
            set => SetProperty(ref _generalCognitiveServices, value);
        }

        public string EntitiesCognitiveServices
        {
            get => _entitiesCognitiveServices;
            set => SetProperty(ref _entitiesCognitiveServices, value);
        }


        string _barcodeString;
        public string BarcodeString
        {
            get => _barcodeString;
            set => SetProperty(ref _barcodeString, value, UpdatePageTilte);
        }

        string _descriptionCaptionOfImage;
        public string DescriptionCaptionOfImage
        {
            get => _descriptionCaptionOfImage;
            set => SetProperty(ref _descriptionCaptionOfImage, value, UpdatePageTilte);
        }

        string _tagsCombinedString;
        public string TagsCombinedString
        {
            get => _tagsCombinedString;
            set => SetProperty(ref _tagsCombinedString, value, UpdatePageTilte);
        }

        string _foregroundColor;
        public string ForegroundColor
        {
            get => _foregroundColor;
            set => SetProperty(ref _foregroundColor, value);
        }

       string _colorsCombinedString;
        public string ColorsCombinedString
        {
            get => _colorsCombinedString;
            set => SetProperty(ref _colorsCombinedString, value, UpdatePageTilte);
        }

        string _objectDescription;
        public string ObjectDescription
        {
            get => _objectDescription;
            set => SetProperty(ref _objectDescription, value, UpdatePageTilte);
        }

        List<string> _tagsListOfStrings;
        public List<string> TagsListOfStrings
        {
            get => _tagsListOfStrings;
            set => SetProperty(ref _tagsListOfStrings, value, UpdatePageTilte);
        }

        List<string> _colorsListOfStrings;
        public List<string> ColorsListOfStrings
        {
            get => _colorsListOfStrings;
            set => SetProperty(ref _colorsListOfStrings, value);
        }

        string _customVisionTagsCombinedString;
        public string CustomVisionTagsCombinedString
        {
            get => _customVisionTagsCombinedString;
            set => SetProperty(ref _customVisionTagsCombinedString, value, UpdatePageTilte);
        }

        Xamarin.Forms.Color _blueColor;
        public Xamarin.Forms.Color BlueColor
        {
            get => _blueColor;
            set => SetProperty(ref _blueColor, value);
        }

        Xamarin.Forms.Color _switchTrueColor;
        public Xamarin.Forms.Color SwitchTrueColor
        {
            get => _switchTrueColor;
            set => SetProperty(ref _switchTrueColor, value);
        }

        bool _isBarcode;
        public bool IsBarcode
        {
            get => _isBarcode;
            set => SetProperty(ref _isBarcode, value);
        }

        bool _isComputerVision;
        public bool IsComputerVision
        {
            get => _isComputerVision;
            set => SetProperty(ref _isComputerVision, value);
        }

        bool _isCustomVision;
        public bool IsCustomVision
        {
            get => _isCustomVision;
            set => SetProperty(ref _isCustomVision, value);
        } 

        bool _isPhotoGallery;
        public bool IsPhotoGallery
        {
            get => _isPhotoGallery;
            set => SetProperty(ref _isPhotoGallery, value);
        }  

        DateTimeOffset _photoCreatedDateTime;
        public DateTimeOffset PhotoCreatedDateTime
        {
            get => _photoCreatedDateTime;
            set => SetProperty(ref _photoCreatedDateTime, value);
        }  

        //#TODO
        string _localPhotoPath;
        public string LocalPhotoPath
        {
            get => _localPhotoPath;
            set => SetProperty(ref _localPhotoPath, value);
        }

        string _localPhotoPathRelevant;
        public string LocalPhotoPathRelevant
        {
            get => _localPhotoPathRelevant;
            set => SetProperty(ref _localPhotoPathRelevant, value);
        }


        //ADDING DATA ON PHOTO
        float _lat;
        public float Lat
        {
            get => _lat;
            set => SetProperty(ref _lat, value);
        }

        float _long;
        public float Long
        {
            get => _long;
            set => SetProperty(ref _long, value);
        }


        string _city;
        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }
           

        string _locationState;
        public string LocationState
        {
            get => _locationState;
            set => SetProperty(ref _locationState, value);
        }

        string _country;
        public string Country
        {
            get => _country;
            set => SetProperty(ref _country, value);
        }

        string _cityState;
        public string CityState
        {
            get => _cityState;
            set => SetProperty(ref _cityState, value);
        }

        string _tag1;
        public string Tag1
        {
            get => _tag1;
            set => SetProperty(ref _tag1, value);
        }

        string _tag2;
        public string Tag2
        {
            get => _tag2;
            set => SetProperty(ref _tag2, value);
        }

        string _tag3;
        public string Tag3
        {
            get => _tag3;
            set => SetProperty(ref _tag3, value);
        }

        string _tag4;
        public string Tag4
        {
            get => _tag4;
            set => SetProperty(ref _tag4, value);
        }

        string _tag5;
        public string Tag5
        {
            get => _tag5;
            set => SetProperty(ref _tag5, value);
        }

        string _tag6;
        public string Tag6
        {
            get => _tag6;
            set => SetProperty(ref _tag6, value);
        }

        string _tag7;
        public string Tag7
        {
            get => _tag7;
            set => SetProperty(ref _tag7, value);
        }

        string _tag8;
        public string Tag8
        {
            get => _tag8;
            set => SetProperty(ref _tag8, value);
        }

        string _tag9;
        public string Tag9
        {
            get => _tag9;
            set => SetProperty(ref _tag9, value);
        }

        string _tag10;
        public string Tag10
        {
            get => _tag10;
            set => SetProperty(ref _tag10, value);
        }

        string _tagsSeperatedWithSpaces;
        public string TagsSeperatedWithSpaces
        {
            get => _tagsSeperatedWithSpaces;
            set => SetProperty(ref _tagsSeperatedWithSpaces, value);
        }

        string _customTag1;
        public string CustomTag1
        {
            get => _customTag1;
            set => SetProperty(ref _customTag1, value);
        }

        string _customTag2;
        public string CustomTag2
        {
            get => _customTag2;
            set => SetProperty(ref _customTag2, value);
        }

        string _customTag3;
        public string CustomTag3
        {
            get => _customTag3;
            set => SetProperty(ref _customTag3, value);
        }

        string _customTag4;
        public string CustomTag4
        {
            get => _customTag4;
            set => SetProperty(ref _customTag4, value);
        }

        string _customTag5;
        public string CustomTag5
        {
            get => _customTag5;
            set => SetProperty(ref _customTag5, value);
        }

        string _customTag6;
        public string CustomTag6
        {
            get => _customTag6;
            set => SetProperty(ref _customTag6, value);
        }

        string _customTag7;
        public string CustomTag7
        {
            get => _customTag7;
            set => SetProperty(ref _customTag7, value);
        }

        string _customTag8;
        public string CustomTag8
        {
            get => _customTag8;
            set => SetProperty(ref _customTag8, value);
        }

        string _customTag9;
        public string CustomTag9
        {
            get => _customTag9;
            set => SetProperty(ref _customTag9, value);
        }

        string _customTag10;
        public string CustomTag10
        {
            get => _customTag10;
            set => SetProperty(ref _customTag10, value);
        }

        string _customTagsSeperatedWithSpaces;
        public string CustomTagsSeperatedWithSpaces
        {
            get => _customTagsSeperatedWithSpaces;
            set => SetProperty(ref _customTagsSeperatedWithSpaces, value);
        }

        string _createdAtString;
        public string CreatedAtString
        {
            get => _createdAtString;
            set => SetProperty(ref _createdAtString, value);
        }

        Xamarin.Essentials.Location _location;

        #endregion

        #region Methods

        //async Task ExecuteGetGeoLocationCommand()
        //{

        //    try
        //    {

        //            try
        //            {

        //                //Device.BeginInvokeOnMainThread(async () =>
        //                //{
        //                //    _location = await Geolocation.GetLastKnownLocationAsync();
        //                //});


        //                Device.BeginInvokeOnMainThread(() =>
        //                {
        //                    _location = Geolocation.GetLastKnownLocationAsync();
        //                });
        //                //var location1 = await Geolocation.GetLastKnownLocationAsync();
        //                //LastLocation = FormatLocation(location);'

        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"error: {ex}");
        //            }

        //       //var location = await Geolocation.GetLastKnownLocationAsync();
        //        //var location = await Geolocation.GetLocationAsync();   

        //        if (_location != null)
        //        {
        //            Console.WriteLine($"Latitude: {_location.Latitude}, Longitude: {_location.Longitude}, Altitude: {_location.Altitude}");
        //        } else
        //        {
        //            Console.WriteLine($"Exiting geolocation");
        //            //return;
        //        }

        //        //this.GeoString = "Paramus, NJ";

        //        //var lat = 47.673988;
        //        //var lon = -122.121513;

        //        //var placemarks = Task.Run(async () => await Geocoding.GetPlacemarksAsync(lat, lon)).Result;

        //        var lat = 47.673988;
        //        var lon = -122.121513;
            
        //        var placemarks = Task.Run(async () => await Geocoding.GetPlacemarksAsync(_location.Latitude, _location.Longitude)).Result;


        //        var placemark = placemarks?.FirstOrDefault();

        //        if (placemark != null)
        //        {
        //            //var geocodeAddress =
        //            //$"AdminArea:       {placemark.AdminArea}\n" +
        //            //$"CountryCode:     {placemark.CountryCode}\n" +
        //            //$"CountryName:     {placemark.CountryName}\n" +
        //            //$"FeatureName:     {placemark.FeatureName}\n" +
        //            //$"Locality:        {placemark.Locality}\n" +
        //            //$"PostalCode:      {placemark.PostalCode}\n" +
        //            //$"SubAdminArea:    {placemark.SubAdminArea}\n" +
        //            //$"SubLocality:     {placemark.SubLocality}\n" +
        //            //$"SubThoroughfare: {placemark.SubThoroughfare}\n" +
        //            //$"Thoroughfare:    {placemark.Thoroughfare}\n";

        //            var geocodeAddress = $"Location: {placemark.Locality}, {placemark.AdminArea}";

        //            //Console.WriteLine(geocodeAddress);
        //            this.GeoString = geocodeAddress;
        //        }
        //    }
        //    catch (FeatureNotSupportedException fnsEx)
        //    {
        //        // Feature not supported on device
        //        //return $"Feature not supported: {fnsEx}";
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exception that may have occurred in geocoding
        //        //return $"Error: {ex}";
        //    }

        //}

        async Task StampDateTime()
        {

            try
            {
                this.PhotoCreatedDateTime = DateTimeOffset.UtcNow;
                //this.CreatedAtString = $"{PhotoCreatedDateTime.ToString("MMMdhmmtt", new CultureInfo("en-US"))}.FilteredDate";
                //this.CreatedAtString = $"{PhotoCreatedDateTime.ToString("yyyyMMddHHmmss", new CultureInfo("en-US"))}.FilteredDateTime";
                this.CreatedAtString = $"{PhotoCreatedDateTime.ToString("yyyyMMdd", new CultureInfo("en-US"))}.FilteredDateTime";

            }
            catch (Exception ex)
            {

            }
        }


            async Task ExecuteGetGeoLocationCommand()
        {
            //#TODO DATABASEENTRIES
            try
            {
                ////MOVE TO SEPERATE COMMAND
                //await this.StampDateTime();

                var locationFromPhone = await GetLocationFromPhone().ConfigureAwait(false);

                if (locationFromPhone is null)
                    return;

                _location = locationFromPhone;

                if (_location != null)
                {
                    Console.WriteLine($"Latitude: {_location.Latitude}, Longitude: {_location.Longitude}, Altitude: {_location.Altitude}");

                    this.Lat =  (float)_location.Latitude;
                    this.Long =  (float)_location.Longitude;

                } else
                {
                    Console.WriteLine($"Exiting geolocation");
                    this.Lat = (float)47.673988;
                    this.Long =  (float)122.121513;

                }

                var localLat = 47.673988;
                var longLong = -122.121513;
            
                var placemarks = Task.Run(async () => await Geocoding.GetPlacemarksAsync(_location.Latitude, _location.Longitude)).Result;


                var placemark = placemarks?.FirstOrDefault();

                if (placemark != null)
                {
                    var geocodeAddress = $"Location: {placemark.Locality}, {placemark.AdminArea}";
                    this.GeoString = geocodeAddress;

                    this.City = placemark.Locality;
                    this.LocationState = placemark.AdminArea;
                    this.Country = placemark.CountryName;
                    this.CityState = placemark.Locality + ", " + placemark.AdminArea;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                //return await $"Error: {fnsEx}";
            }
            catch (Exception ex)
            {
                //return await $"Error: {ex}";
            }
        }


        async Task<Xamarin.Essentials.Location> GetLocationFromPhone()
        {

            var locationTaskCompletionSource = new TaskCompletionSource<Xamarin.Essentials.Location>();
            Device.BeginInvokeOnMainThread( async () =>
                {
                    locationTaskCompletionSource.SetResult(await Geolocation.GetLastKnownLocationAsync());
                }   
            );

            return await locationTaskCompletionSource.Task;
        }


        //async Task ExecuteSavePhotoCommand(PhotoBlobModel photoBlob, string photoTitle)
        //{
        //    if (IsPhotoSaving)
        //        return;

        //    //#TODO - Uncomment - when blob storage requires authorization
        //    //if (string.IsNullOrWhiteSpace(BackendConstants.PostPhotoBlobFunctionKey))
        //    //{
        //    //    OnSavePhotoFailed("Invalid Azure Function Key");
        //    //    return;
        //    //}

        //    if (string.IsNullOrWhiteSpace(photoTitle))
        //    {
        //        OnSavePhotoFailed("Title Cannot Be Empty");
        //        return;
        //    }

        //    IsPhotoSaving = true;

        //    try
        //    {
        //        var photo = await APIService.PostPhotoBlob(photoBlob, photoTitle).ConfigureAwait(false);

        //        if (photo is null)
        //        {
        //            OnSavePhotoFailed("Error Uploading Photo");
        //        }
        //        else
        //        {
        //            await PhotoDatabase.SavePhoto(photo).ConfigureAwait(false);
        //            OnSavePhotoCompleted();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        OnSavePhotoFailed(e.Message);
        //    }
        //    finally
        //    {
        //        IsPhotoSaving = false;
        //    }
        //}

        ////WITHOUT INTERNET
        //async Task ExecuteSavePhotoCommand(PhotoBlobModel photoBlob, string photoTitle)
        //{
        //    if (IsPhotoSaving)
        //        return;

        //    //#TODO - Uncomment - when blob storage requires authorization
        //    if (string.IsNullOrWhiteSpace(BackendConstants.PostPhotoBlobFunctionKey))
        //    {
        //        OnSavePhotoFailed("Invalid Azure Function Key");
        //        return;
        //    }

        //    if (string.IsNullOrWhiteSpace(photoTitle))
        //    {
        //        OnSavePhotoFailed("Title Cannot Be Empty");
        //        return;
        //    }

        //    IsPhotoSaving = true;

        //    try
        //    {
        //        //var photo = await APIService.PostPhotoBlob(photoBlob, photoTitle).ConfigureAwait(false);

        //        //if (photo is null)
        //        //{
        //        //    OnSavePhotoFailed("Error Uploading Photo");
        //        //}
        //        //else
        //        //{

        //        var currentTime = DateTimeOffset.UtcNow;

        //        var photo = new PhotoModel() { 
                
        //            Title = photoTitle,
        //            //Url = LocalPhotoPath,
        //            Url = LocalPhotoPathRelevant,
        //            //CreatedAt = currentTime,
        //            //UpdatedAt = currentTime,
        //        };
        //            await PhotoDatabase.SavePhoto(photo).ConfigureAwait(false);
        //            OnSavePhotoCompleted();
        //        //}
        //    }
        //    catch (Exception e)
        //    {
        //        OnSavePhotoFailed(e.Message);
        //    }
        //    finally
        //    {
        //        IsPhotoSaving = false;
        //    }
        //}

        //WITHOUT INTERNET
        async Task ExecuteSavePhotoCommand(PhotoBlobModel photoBlob, string photoTitle)
        {

            if (IsPhotoSaving)
                return;

            //#TODO - Uncomment - when blob storage requires authorization
            if (string.IsNullOrWhiteSpace(BackendConstants.PostPhotoBlobFunctionKey))
            {
                OnSavePhotoFailed("Invalid Azure Function Key");
                return;
            }

            if (string.IsNullOrWhiteSpace(photoTitle))
            {
                OnSavePhotoFailed("Title Cannot Be Empty");
                return;
            }

            IsPhotoSaving = true;

            try
            {

                if  (this.IsInternetConnectionActive == true)
                {
                    //PHOTOBLOB - SAVE THE VARIOUS ATTRIBUTES
                    photoBlob.Tag1 = Tag1;
                    photoBlob.Tag2 = Tag2;
                    photoBlob.Tag3 = Tag3;
                    photoBlob.Tag4 = Tag4;
                    photoBlob.Tag5 = Tag5;
                    photoBlob.Tag6 = Tag6;
                    photoBlob.Tag7 = Tag7;
                    photoBlob.Tag8 = Tag8;
                    photoBlob.Tag9 = Tag9;
                    photoBlob.Tag10 = Tag10;

                    photoBlob.TagsSeperatedWithSpaces = TagsSeperatedWithSpaces;

                    photoBlob.CustomTag1 = CustomTag1;
                    photoBlob.CustomTag2 = CustomTag2;
                    photoBlob.CustomTag3 = CustomTag3;
                    photoBlob.CustomTag4 = CustomTag4;
                    photoBlob.CustomTag5 = CustomTag5;
                    photoBlob.CustomTag6 = CustomTag6;
                    photoBlob.CustomTag7 = CustomTag7;
                    photoBlob.CustomTag8 = CustomTag8;
                    photoBlob.CustomTag9 = CustomTag9;
                    photoBlob.CustomTag10 = CustomTag10;

                    photoBlob.CustomTagsSeperatedWithSpaces = CustomTagsSeperatedWithSpaces;
                    photoBlob.CreatedAtString = CreatedAtString;

                    photoBlob.City = City;
                    photoBlob.LocationState = LocationState;
                    photoBlob.Country = Country;
                    photoBlob.CityState = CityState;

                    photoBlob.Lat = Lat;
                    photoBlob.Long = Long;

                    photoBlob.BarcodeString = BarcodeString;

                    var photo = await APIService.PostPhotoBlob(photoBlob, photoTitle).ConfigureAwait(false);

                    if (photo is null)
                    {
                        OnSavePhotoFailed("Error Uploading Photo");
                    }
                    else
                    {
                        await PhotoDatabase.SavePhoto(photo).ConfigureAwait(false);
                        OnSavePhotoCompleted();
                    } 
                }
                //INTERNET-OFF
                else if (this.IsInternetConnectionActive == false)
                { 
                    //var photo = await APIService.PostPhotoBlob(photoBlob, photoTitle).ConfigureAwait(false);

                    //if (photo is null)
                    //{
                    //    OnSavePhotoFailed("Error Uploading Photo");
                    //}
                    //else
                    //{

                    var currentTime = DateTimeOffset.UtcNow;


                    //PHOTO - SAVE THE VARIOUS ATTRIBUTES


                    var photo = new PhotoModel() { 
                    
                        Title = photoTitle,
                        //Url = LocalPhotoPath,
                        Url = LocalPhotoPathRelevant,
                        CreatedAt = currentTime,
                        //UpdatedAt = currentTime,
                        Tag1 = Tag1,
                        Tag2 = Tag2,
                        Tag3 = Tag3,
                        Tag4 = Tag4,
                        Tag5 = Tag5,
                        Tag6 = Tag6,
                        Tag7 = Tag7,
                        Tag8 = Tag8,
                        Tag9 = Tag9,
                        Tag10 = Tag10,

                        TagsSeperatedWithSpaces = TagsSeperatedWithSpaces,

                        CustomTag1 = CustomTag1,
                        CustomTag2 = CustomTag2,
                        CustomTag3 = CustomTag3,
                        CustomTag4 = CustomTag4,
                        CustomTag5 = CustomTag5,
                        CustomTag6 = CustomTag6,
                        CustomTag7 = CustomTag7,
                        CustomTag8 = CustomTag8,
                        CustomTag9 = CustomTag9,
                        CustomTag10 = CustomTag10,

                        CustomTagsSeperatedWithSpaces = CustomTagsSeperatedWithSpaces,
                        CreatedAtString = CreatedAtString,

                        City = City,
                        LocationState = LocationState,
                        Country = Country,
                        CityState = CityState,

                        Lat = Lat,
                        Long = Long,

                        BarcodeString = BarcodeString, 

                    };
                        //photo.photoTag1 = Tag1,
                        //photo.Tag2 = Tag2,
                        //photo.Tag3 = Tag3,
                        //photo.Tag4 = Tag4,
                        //photo.Tag5 = Tag5,
                        //photo.Tag6 = Tag6,
                        //photo.Tag7 = Tag7;
                        //photo.Tag8 = Tag8,
                        //photo.Tag9 = Tag9,
                        //photo.Tag10 = Tag10,

                        //photo.TagsSeperatedWithSpaces = TagsSeperatedWithSpaces,

                        //photo.Tag1 = Tag1,
                        //photo.Tag2 = Tag2,
                        //photo.Tag3 = Tag3,
                        //photo.Tag4 = Tag4,
                        //photo.Tag5 = Tag5,
                        //photo.Tag6 = Tag6,
                        //photo.Tag7 = Tag7,
                        //photo.Tag8 = Tag8,
                        //photo.Tag9 = Tag9,
                        //photo.Tag10 = Tag10,

                        //photo.CustomTagsSeperatedWithSpaces = CustomTagsSeperatedWithSpaces,
                        //photo.CreatedAtString = CustomTagsSeperatedWithSpaces,


                        await PhotoDatabase.SavePhoto(photo).ConfigureAwait(false);
                        OnSavePhotoCompleted();
                }
            }
            catch (Exception e)
            {
                OnSavePhotoFailed(e.Message);
            }
            finally
            {
                IsPhotoSaving = false;
            }
        }

        //PICK PHOTO
        //BarcodeDecoding barcode;
        async Task ExecuteGetPhotoCommand()
        {
            await this.StampDateTime();
             
            var mediaFile = await GetStoredMediaFileFromCamera().ConfigureAwait(false);

            if (mediaFile is null)
                return;

            var tempByteArray = ConvertStreamToByteArrary(mediaFile.GetStream());

            this.PhotoBlob = new PhotoBlobModel
            {
                Image = ConvertStreamToByteArrary(mediaFile.GetStream())
            };


            //////

            //File.WR   (SAVE WITH PROPER NAME // THEN SET THE URL PROPERTY OF PAGE TO EQUAL THE "RELEVANT PATH PROPERTY")
            
            var pathNameFile = mediaFile.Path;

//"/var/mobile/Containers/Data/Application/1655FB04-8633-43AF-90AB-E514F28556C6/Documents/temp/IMG_20190225_132848.jpg"

            var tempFolderPlusFileName = pathNameFile.Replace(App.LocalAppDirectoryPath, "");
            var newFileName = tempFolderPlusFileName.Replace("temp/" , "");

            string destFolderIncludingLocalPhotoFolder = System.IO.Path.Combine(App.LocalAppDirectoryPath, App.LocalPhotoFolderName);
            string destFilePath = System.IO.Path.Combine(destFolderIncludingLocalPhotoFolder, newFileName);

            var finalFolderPath = App.LocalAppDirectoryPath + App.LocalPhotoFolderName;

    //Should be the following
    //"/var/mobile/Containers/Data/Application/1655FB04-8633-43AF-90AB-E514F28556C6/Documents/LocalPhotosFolder
            if (!System.IO.Directory.Exists(finalFolderPath))
            {
                System.IO.Directory.CreateDirectory(finalFolderPath);
            }

            System.IO.File.Copy(pathNameFile, destFilePath, true);

            var directoryName = App.LocalPhotoFolderName;

            this.LocalPhotoPathRelevant = $"{App.LocalPhotoFolderName}/{newFileName}";
            

//////////////////////////////////////////////////////////////////////////


            //DON'T NEED THIS
            //this.LocalPhotoPath = pathNameFile;

            //var tempByteArray = ConvertStreamToByteArrary(mediaFile.GetStream());

            //PhotoBlob = new PhotoBlobModel
            //{
            //    Image = ConvertStreamToByteArrary(mediaFile.GetStream())
            //};

            ////////


            if (IsComputerVision || IsCustomVision)
            {
                //TODO
                this.GetGeoLocationCommand.Execute(null);
            }

            if (IsComputerVision)
            {
                //COMPUTER VISION
                IList<VisualFeatureTypes> visFeatures = new List<VisualFeatureTypes>() {
                    VisualFeatureTypes.Tags, VisualFeatureTypes.Color, VisualFeatureTypes.Categories, VisualFeatureTypes.Color, VisualFeatureTypes.Faces, VisualFeatureTypes.Objects, VisualFeatureTypes.ImageType, VisualFeatureTypes.Description
                };
     
                //TODO
                var client = new ComputerVisionService();
                using (var photoStream = mediaFile.GetStream())
                {
                    //ImageAnalysis analysis = client.AnalyzeImageAsync(photoStream);
                    //ImageAnalysis analysis = await client.computerVisionClient.AnalyzeImageInStreamAsync(photoStream);    //AnalyzeImageInStreamAsync(photoStream);

                    analysis = await client.computerVisionClient.AnalyzeImageInStreamAsync(photoStream, visFeatures);                                                                                                //DisplayResults (analysis, photoStream);
                    DisplayResults(analysis);
                }
            }
        
            if (IsCustomVision)
            {
               //CUSTOM VISION
                var tagList = this.GetBestTagList(mediaFile);
                DisplayCustomVisionResults(tagList);
            }
        }


        //TAKE PHOTO
        async Task ExecuteTakePhotoCommand()
        {
            await this.StampDateTime();

            //DELETE
            //var mediaFile = await GetStoredMediaFileFromCamera().ConfigureAwait(false);

            var mediaFile = await GetMediaFileFromCamera().ConfigureAwait(false);

            if (mediaFile is null)
                return;

            var pathNameFile = mediaFile.Path;
            //var pathNameAlbum = mediaFile.AlbumPath;

            //DON'T NEED THIS
            this.LocalPhotoPath = pathNameFile;

            var tempByteArray = ConvertStreamToByteArrary(mediaFile.GetStream());

            PhotoBlob = new PhotoBlobModel
            {
                Image = ConvertStreamToByteArrary(mediaFile.GetStream())
            };


            if (IsComputerVision || IsCustomVision)
            {
                //TODO
                this.GetGeoLocationCommand.Execute(null);
            }

            if (IsComputerVision)
            {
                //COMPUTER VISION
                IList<VisualFeatureTypes> visFeatures = new List<VisualFeatureTypes>() {
                    VisualFeatureTypes.Tags, VisualFeatureTypes.Color, VisualFeatureTypes.Categories, VisualFeatureTypes.Color, VisualFeatureTypes.Faces, VisualFeatureTypes.Objects, VisualFeatureTypes.ImageType, VisualFeatureTypes.Description
                };
     
                //TODO
                var client = new ComputerVisionService();
                using (var photoStream = mediaFile.GetStream())
                {
                    //ImageAnalysis analysis = client.AnalyzeImageAsync(photoStream);
                    //ImageAnalysis analysis = await client.computerVisionClient.AnalyzeImageInStreamAsync(photoStream);    //AnalyzeImageInStreamAsync(photoStream);

                    analysis = await client.computerVisionClient.AnalyzeImageInStreamAsync(photoStream, visFeatures);                                                                                                //DisplayResults (analysis, photoStream);
                    DisplayResults(analysis);
                }
            }
        
            if (IsCustomVision)
            {
               //CUSTOM VISION
                var tagList = this.GetBestTagList(mediaFile);
                DisplayCustomVisionResults(tagList);
            }

            //TODO
            //var barcodeScannerService = new BarcodeScannerServiceLib();
            //var stringBarcode = barcodeScannerService.JustDecodeBarcode(PhotoBlob.Image);
            //var stringBarcode = barcodeScannerService.DecodeBarcode(PhotoBlob.Image);

            //var barcodeScannerService = new BarcodeScannerService();
            //var byteArray = this.DoConvertMediaFileToByteArray(mediaFile);
            //var stringBarcode = barcodeScannerService.DecodeBarcode(byteArray);


            //System.Drawing.Bitmap(filename);

            //int hi = 5;

            //barcode = new BarcodeDecoding();

            //var aditionalHints = new KeyValuePair<DecodeHintType, object>(key: DecodeHintType.PURE_BARCODE, value: "TRUE");

            //var result = barcode.Decode(file: "image_to_read", format: BarcodeFormat.QR_CODE, aditionalHints: new[] { aditionalHints });

            //Label to show the text decoded
            //QrResult.Text = result.Text;

            //var qrRest = result.Text;

        }

        // Display the most relevant caption for the image
        private void DisplayResults(ImageAnalysis analysis)
        {
            Console.WriteLine("Test image 1");
            //Console.WriteLine(analysis.Description.Captions[0].Text + "\n");

            this.DescriptionCaptionOfImage = analysis.Description.Captions.FirstOrDefault()?.Text ?? "";

            this.ForegroundColor = analysis.Color?.DominantColorForeground ?? ""; //.FirstOrDefault()?.Text ?? "";

            //this.ColorsListOfStrings = analysis.Color.Select(t => t.DominantColors).ToList();
            this.ColorsListOfStrings = analysis.Color.DominantColors.ToList();


            var newStringBuilder1 = new StringBuilder();

            //foreach (var metaData in result.ResultMetadata)

            //foreach (var colorName in analysis.Tags.Select(t => t.Name))
            foreach (var colorName in analysis.Color.DominantColors.Select(x=>x.ToLower()).ToList())
            {
                newStringBuilder1.Append($"#{colorName} ");
            }

            var combinedTagString1 = newStringBuilder1.ToString();
            var trimCombinedString1 = combinedTagString1.Trim();

            this.ColorsCombinedString = trimCombinedString1;




            this.ObjectDescription = analysis.Objects.FirstOrDefault()?.ObjectProperty ?? "";  //.Text ?? "";

            this.TagsListOfStrings = analysis.Tags.Select(t => t.Name).ToList();
            //this.TagsCombinedString = analysis.Tags.Select(t => t.Name).ToString();

            //TagsCombinedString
            var newStringBuilder = new StringBuilder();

            //foreach (var metaData in result.ResultMetadata)

            foreach (var tagName in analysis.Tags.Select(t => t.Name))
            {
                newStringBuilder.Append($"#{tagName} ");

            }

            var combinedTagString = newStringBuilder.ToString();
            var trimCombinedString = combinedTagString.Trim();

            this.TagsCombinedString = trimCombinedString;

            //TagsCombinedStringWithoutTag
            var newStringBuilderWithoutTag = new StringBuilder();

            //foreach (var metaData in result.ResultMetadata)

            //foreach (var tagName in analysis.Tags.Select(t => t.Name))
            //{
            //    newStringBuilderWithoutTag.Append($"{tagName} ");

            //}


            //for (int i = 0; i < 10; i++)
            //for ((var tagName in analysis.Tags.Select(t => t.Name))
            //{
            //    newStringBuilderWithoutTag.Append($"{tagName} ");

            //}

            for (int i = 0; i < analysis.Tags.Count ; i++)
            {
                var itemTagName = analysis.Tags[i].Name;
                newStringBuilderWithoutTag.Append($"{itemTagName} ");

                switch (i+1)
                      {
                          case 1:
                              this.Tag1 = itemTagName;
                              break;
                          case 2:
                              this.Tag2 = itemTagName;
                              break;
                          case 3:
                              this.Tag3 = itemTagName;
                              break;
                          case 4:
                              this.Tag4 = itemTagName;
                              break;
                          case 5:
                              this.Tag5 = itemTagName;
                              break;
                          case 6:
                              this.Tag6 = itemTagName;
                              break;
                          case 7:
                              this.Tag7 = itemTagName;
                              break;
                          case 8:
                              this.Tag8 = itemTagName;
                              break;
                          case 9:
                              this.Tag9 = itemTagName;
                              break;
                          case 10:
                              this.Tag10 = itemTagName;
                              break;
                          default:
                              Console.WriteLine("Default case");
                              break;
                }
            }

            var combinedTagStringWithoutTag = newStringBuilderWithoutTag.ToString();
            var trimCombinedStringWithoutTag = combinedTagStringWithoutTag.Trim();

                       //this.TagsCombinedString = trimCombinedString;

            this.TagsSeperatedWithSpaces  = trimCombinedStringWithoutTag;


            //TagsListOfStrings
        }

        // Display the most relevant caption for the image
        //private void DisplayCustomVisionResults(IEnumerable<ImageTagPredictionModel> tagList)
        private void DisplayCustomVisionResults(IEnumerable<ImageTagPredictionModel> tagList)
        {
            StringBuilder stringOfTags = new StringBuilder();


            if (tagList == null)
                stringOfTags.Append($"No tags found");
            else
            { 
                //TAGS WITH #
                foreach (var tagItem in tagList)
                {
                    stringOfTags.Append($"#{tagItem.Tag} ");
                    //Console.WriteLine($"\t{c.TagName}: {c.Probability:P1}");
                }

                var combinedTagString = stringOfTags.ToString();
                var trimCombinedString = combinedTagString.Trim();

                this.CustomVisionTagsCombinedString = trimCombinedString;

                //TAGS WITHOUT # TO BE SAVED
                StringBuilder stringOfTagsWithoutHashes = new StringBuilder();

                //foreach (var tagItem in tagList)
                //{
                //    stringOfTagsWithoutHashes.Append($"#{tagItem.Tag} ");
                //    //Console.WriteLine($"\t{c.TagName}: {c.Probability:P1}");
                //}

                //var combinedTagStringWithoutHashes = stringOfTagsWithoutHashes.ToString();
                //var trimCombinedStringWithoutHashes = combinedTagStringWithoutHashes.Trim();

                //this.CustomTagsSeperatedWithSpaces = trimCombinedStringWithoutHashes;

                var ListTagList = tagList.ToList();

                //for (int i = 0; i < tagList.Count(); i++)
                for (int i = 0; i < ListTagList.Count(); i++)
                {
                    //var itemTagName = analysis.Tags[i].Name;

                    var itemTagName = ListTagList[i].Tag;    ; //analysis.Tags[i].Name;

                    stringOfTagsWithoutHashes.Append($"{itemTagName} ");

                    switch (i+1)
                    {
                          case 1:
                              this.CustomTag1 = itemTagName;
                              break;
                          case 2:
                              this.CustomTag2 = itemTagName;
                              break;
                          case 3:
                              this.CustomTag3 = itemTagName;
                              break;
                          case 4:
                              this.CustomTag4 = itemTagName;
                              break;
                          case 5:
                              this.CustomTag5 = itemTagName;
                              break;
                          case 6:
                              this.CustomTag6 = itemTagName;
                              break;
                          case 7:
                              this.CustomTag7 = itemTagName;
                              break;
                          case 8:
                              this.CustomTag8 = itemTagName;
                              break;
                          case 9:
                              this.CustomTag9 = itemTagName;
                              break;
                          case 10:
                              this.CustomTag10 = itemTagName;
                              break;
                          default:
                              Console.WriteLine("Default case");
                              break;
                    }
                }

                var combinedTagStringWithoutHashes = stringOfTagsWithoutHashes.ToString();
                var trimCombinedStringWithoutHashes = combinedTagStringWithoutHashes.Trim();

                this.CustomTagsSeperatedWithSpaces = trimCombinedStringWithoutHashes;

            }
        }

        private IEnumerable<ImageTagPredictionModel> GetBestTagList(MediaFile file)
        {
            using (var stream = file.GetStream())
            {
                //var predictImagePredictions = _endpoint.PredictImage(CustomVisionService.ProjectId, stream).Predictions;

                var _endpoint = new CustomVisionService()._endpoint;
                var predictImagePredictions = _endpoint.PredictImage(CustomVisionService.ProjectId, stream).Predictions;
                var orderedPredictions = predictImagePredictions.OrderByDescending(p => p.Probability).Where(p => p.Probability > CustomVisionService.ProbabilityThreshold);

                return orderedPredictions;
            }
        }


        private Task ExecuteTakeScanCommand()
        {
            //    var customScanPage = new CustomScanPage();
            //    //    await Navigation.PushAsync(customScanPage);
            throw new NotImplementedException();
        }


        byte[] ConvertStreamToByteArrary(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        //https://stackoverflow.com/questions/33947138/convert-image-into-byte-array-in-xamarin-forms
        byte[] DoConvertMediaFileToByteArray(MediaFile mediaFile)
        {

            byte[] imageByte;
            Stream imageStream = null;

            //var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            //{ Name = "pic.jpg" });
            //if (file == null) return;

            imageStream = mediaFile.GetStream();
            BinaryReader br = new BinaryReader(imageStream);
            return imageByte = br.ReadBytes((int)imageStream.Length);

        }

        //TAKE PHOTO
        async Task<MediaFile> GetMediaFileFromCamera()
        {
            await CrossMedia.Current.Initialize().ConfigureAwait(false);

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                OnNoCameraFound();
                return null;
            }

            var dateTimeNowStringJpg = $"{DateTime.Now.ToString("MMMdhmmtt", new CultureInfo("en-US"))}.jpg";

            //var directoryName = "LocalPhotosFolder";
            //can switch to 
            var directoryName = App.LocalPhotoFolderName;


            var mediaFileTCS = new TaskCompletionSource<MediaFile>();
            Device.BeginInvokeOnMainThread(async () =>
            {
                mediaFileTCS.SetResult(await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Small,
                    DefaultCamera = CameraDevice.Rear,
                    Directory = directoryName,
                    Name = dateTimeNowStringJpg,
                }));
            });

            this.LocalPhotoPathRelevant = $"{directoryName}/{dateTimeNowStringJpg}";
            



            var fileAppDataDirectory = FileSystem.AppDataDirectory;
            //var fileAppCacheDirectory = FileSystem.CacheDirectory;

            var fileAppCacheDirectoryMinusLibrary = fileAppDataDirectory.Replace("Library", "");
            var fileAppCacheDirectoryMinusLibraryPlusDirectory = fileAppCacheDirectoryMinusLibrary + "Documents/" + $"{directoryName}/{dateTimeNowStringJpg}";
            //this.LocalPhotoPathRelevant  = fileAppCacheDirectoryMinusLibraryPlusDirectory; //NO - we just want Directory + filenamae <--- this line as it stands takes full path

            return await mediaFileTCS.Task;
        }

        //PICK PHOTO
        async Task<MediaFile> GetStoredMediaFileFromCamera()
        {
            await CrossMedia.Current.Initialize().ConfigureAwait(false);

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                //OnNoCameraFound();
                OnNoCameraPickerFound();
                return null;
            }

            var mediaFileTCS = new TaskCompletionSource<MediaFile>();
            Device.BeginInvokeOnMainThread(async () =>
            {
                mediaFileTCS.SetResult(await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Small,
                    //DefaultCamera = CameraDevice.Rear,
                   
                }));
            });


            return await mediaFileTCS.Task;
        }

        void UpdatePageTilte()
        {
            if (string.IsNullOrWhiteSpace(PhotoTitle))
                PageTitle = PageTitles.AddPhotoPage;
            else
                PageTitle = PhotoTitle;
        }

        void UpdatePhotoImageSource() =>
            PhotoImageSource = ImageSource.FromStream(() => new MemoryStream(PhotoBlob.Image));

        void OnSavePhotoFailed(string errorMessage) => _savePhotoFailedEventManager.HandleEvent(this, errorMessage, nameof(SavePhotoFailed));
        void OnNoCameraFound() => _noCameraFoundEventManager.HandleEvent(this, EventArgs.Empty, nameof(NoCameraFound));
        void OnNoCameraPickerFound() => _noCameraPickerFoundEventManager.HandleEvent(this, EventArgs.Empty, nameof(OnNoCameraPickerFound));
        void OnSavePhotoCompleted() => _savePhotoCompletedEventManager.HandleEvent(this, EventArgs.Empty, nameof(SavePhotoCompleted));
        #endregion
    }
}
