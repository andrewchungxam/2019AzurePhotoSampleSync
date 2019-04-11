using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using AzureBlobStorageSampleApp.Mobile.Shared;
using AzureBlobStorageSampleApp.Shared;

using Polly;
using Refit;

namespace AzureBlobStorageSampleApp
{
    static class APIService
    {
        #region Constant Fields
        readonly static Lazy<IPhotosAPI> _photosApiClientHolder = new Lazy<IPhotosAPI>(() => RestService.For<IPhotosAPI>(new HttpClient { BaseAddress = new Uri(BackendConstants.FunctionsAPIBaseUrl) }));
        #endregion

        #region Properties
        static IPhotosAPI PhotosApiClient => _photosApiClientHolder.Value;
        #endregion

        #region Methods
        public static Task<List<PhotoModel>> GetAllPhotoModels() => ExecutePollyFunction(PhotosApiClient.GetAllPhotoModels);

        public static Task<PhotoModel> PostPhotoBlob(PhotoBlobModel photoBlob, string photoTitle) => ExecutePollyFunction(() => PhotosApiClient.PostPhotoBlob(photoBlob, photoTitle, BackendConstants.PostPhotoBlobFunctionKey));

        public static Task<PhotoModel> PostPhotoBlobPlusId(PhotoBlobModelPlusId photoBlob, string photoTitle) => ExecutePollyFunction(() => PhotosApiClient.PostPhotoBlobPlusId(photoBlob, photoTitle, BackendConstants.PostPhotoBlobFunctionKey));

        public static Task<PhotoModel> PostPhoto(PhotoModel photoModel, string photoTitle) => ExecutePollyFunction(() => PhotosApiClient.PostPhoto(photoModel, photoTitle, BackendConstants.PostPhotoBlobFunctionKey));

        public static Task<PhotoModel> PatchPhoto(PhotoModel photoModel, string photoTitle) => ExecutePollyFunction(() => PhotosApiClient.PatchPhoto(photoModel, photoTitle, BackendConstants.PostPhotoBlobFunctionKey));


        //#TODO - if not using Function with AuthorizationLevel.Anonymous, add something similar to the following (Also be user to make changes in IPhotosAPI.cs)

        //public static Task<PhotoModel> PostPhotoBlob(PhotoBlobModel photoBlob, string photoTitle) => ExecutePollyFunction(() => PhotosApiClient.PostPhotoBlob(photoBlob, photoTitle));

        static Task<T> ExecutePollyFunction<T>(Func<Task<T>> action, int numRetries = 3)
        {
            return Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync
                    (
                        numRetries,
                        pollyRetryAttempt
                    ).ExecuteAsync(action);

            TimeSpan pollyRetryAttempt(int attemptNumber) => TimeSpan.FromSeconds(Math.Pow(2, attemptNumber));
        }
        #endregion
    }
}
