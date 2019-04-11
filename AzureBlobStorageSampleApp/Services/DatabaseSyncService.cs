using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using AzureBlobStorageSampleApp.Shared;
using System.IO;
using System;

namespace AzureBlobStorageSampleApp
{
    public static class DatabaseSyncService
    {
        public static async Task SyncRemoteAndLocalDatabases()
        {
            var (photoListFromLocalDatabase, photoListFromRemoteDatabase) = await GetAllSavedPhotos().ConfigureAwait(false);

            var (photosInLocalDatabaseButNotStoredRemotely, photosInRemoteDatabaseButNotStoredLocally, photosInBothDatabases) = GetMatchingModels(photoListFromLocalDatabase, photoListFromRemoteDatabase);

            var (photosToPatchToLocalDatabase, photosToPatchToRemoteDatabase) = GetModelsThatNeedUpdating(photoListFromLocalDatabase, photoListFromRemoteDatabase, photosInBothDatabases);

            //await SavePhotos(photosInRemoteDatabaseButNotStoredLocally.Concat(photosToPatchToLocalDatabase).ToList(), photosInLocalDatabaseButNotStoredRemotely.Concat(photosToPatchToRemoteDatabase).ToList());

            //restructured away from previous parameters
            await SavePhotos(photosToPatchToRemoteDatabase,  photosToPatchToLocalDatabase, photosInRemoteDatabaseButNotStoredLocally, photosInLocalDatabaseButNotStoredRemotely);
        
            }

        static async Task<(List<PhotoModel> photoListFromLocalDatabase,
            List<PhotoModel> photoListFromRemoteDatabase)> GetAllSavedPhotos()
        {
            var photoListFromLocalDatabaseTask = PhotoDatabase.GetAllPhotos();
            var photoListFromRemoteDatabaseTask = APIService.GetAllPhotoModels();

            await Task.WhenAll(photoListFromLocalDatabaseTask, photoListFromRemoteDatabaseTask).ConfigureAwait(false);

            return (await photoListFromLocalDatabaseTask ?? new List<PhotoModel>(),
                    await photoListFromRemoteDatabaseTask ?? new List<PhotoModel>());
        }

        static (List<T> contactsInLocalDatabaseButNotStoredRemotely,
            List<T> contactsInRemoteDatabaseButNotStoredLocally,
            List<T> contactsInBothDatabases) GetMatchingModels<T>(List<T> modelListFromLocalDatabase,
                                                                      List<T> modelListFromRemoteDatabase) where T : IBaseModel
        {
            var modelIdFromRemoteDatabaseList = modelListFromRemoteDatabase?.Select(x => x.Id).ToList() ?? new List<string>();
            var modelIdFromLocalDatabaseList = modelListFromLocalDatabase?.Select(x => x.Id).ToList() ?? new List<string>();

            var modelIdsInRemoteDatabaseButNotStoredLocally = modelIdFromRemoteDatabaseList?.Except(modelIdFromLocalDatabaseList)?.ToList() ?? new List<string>();
            var modelIdsInLocalDatabaseButNotStoredRemotely = modelIdFromLocalDatabaseList?.Except(modelIdFromRemoteDatabaseList)?.ToList() ?? new List<string>();
            var modelIdsInBothDatabases = modelIdFromRemoteDatabaseList?.Where(x => modelIdFromLocalDatabaseList?.Contains(x) ?? false).ToList() ?? new List<string>();

            var modelsInRemoteDatabaseButNotStoredLocally = modelListFromRemoteDatabase?.Where(x => modelIdsInRemoteDatabaseButNotStoredLocally?.Contains(x?.Id) ?? false).ToList() ?? new List<T>();
            var modelsInLocalDatabaseButNotStoredRemotely = modelListFromLocalDatabase?.Where(x => modelIdsInLocalDatabaseButNotStoredRemotely?.Contains(x?.Id) ?? false).ToList() ?? new List<T>();

            var modelsInBothDatabases = modelListFromLocalDatabase?.Where(x => modelIdsInBothDatabases?.Contains(x?.Id) ?? false)
                                            .ToList() ?? new List<T>();

            return (modelsInLocalDatabaseButNotStoredRemotely ?? new List<T>(),
                    modelsInRemoteDatabaseButNotStoredLocally ?? new List<T>(),
                    modelsInBothDatabases ?? new List<T>());

        }

        static (List<T> contactsToPatchToLocalDatabase,
            List<T> contactsToPatchToRemoteDatabase) GetModelsThatNeedUpdating<T>(List<T> modelListFromLocalDatabase,
                                                                              List<T> modelListFromRemoteDatabase,
                                                                              List<T> modelsFoundInBothDatabases) where T : IBaseModel
        {
            var modelsToPatchToRemoteDatabase = new List<T>();
            var modelsToPatchToLocalDatabase = new List<T>();
            foreach (var contact in modelsFoundInBothDatabases)
            {
                var modelFromLocalDatabase = modelListFromLocalDatabase.Where(x => x.Id.Equals(contact.Id)).FirstOrDefault();
                var modelFromRemoteDatabase = modelListFromRemoteDatabase.Where(x => x.Id.Equals(contact.Id)).FirstOrDefault();

                if (modelFromLocalDatabase?.UpdatedAt.CompareTo(modelFromRemoteDatabase?.UpdatedAt ?? default) > 0)
                    modelsToPatchToRemoteDatabase.Add(modelFromLocalDatabase);
                else if (modelFromLocalDatabase?.UpdatedAt.CompareTo(modelFromRemoteDatabase?.UpdatedAt ?? default) < 0)
                    modelsToPatchToLocalDatabase.Add(modelFromRemoteDatabase);
            }

            return (modelsToPatchToLocalDatabase ?? new List<T>(),
                    modelsToPatchToRemoteDatabase ?? new List<T>());
        }

        //static Task SavePhotos(List<PhotoModel> photosToSaveToLocalDatabase,
        //                        List<PhotoModel> photosToSaveToRemoteDatabase)
        //{
        //    var savephotoTaskList = new List<Task>();

        //    foreach (var photo in photosToSaveToLocalDatabase)
        //        savephotoTaskList.Add(PhotoDatabase.SavePhoto(photo));

        //    //ToDo Add Patch API

        //    return Task.WhenAll(savephotoTaskList);
        //}

        async static Task SavePhotos(List<PhotoModel> photosToPatchToRemoteDatabase, 
                                    List<PhotoModel> photosToPatchToLocalDatabase,
                                 List<PhotoModel> photosToAddToLocalDatabase,
                                    List<PhotoModel> photosToPostToRemoteDatabase)
        {
            var savephotoTaskList = new List<Task>();

            //THERE WILL NEED TO BE LOGIC HERE - AS SOME ARE CLOUD READY AND OTHERS ARE NOT
            foreach (var photo in photosToPostToRemoteDatabase)
                {   if (photo.Url.Contains("http"))
                    { 
                        savephotoTaskList.Add(APIService.PostPhoto(photo, photo.Title));
                    }
                    else if (photo.Url.Contains(App.LocalPhotoFolderName))
                    {
                     
                        var fullPathForFile = App.LocalAppDirectoryPath + photo.Url;

                        //https://forums.xamarin.com/discussion/149063/how-to-convert-a-picture-path-to-mediafile
                        //var bytes = File.ReadAllBytes(filePath);

                        var bytes = File.ReadAllBytes(fullPathForFile);

                        //var tempByteArray = ConvertStreamToByteArrary(mediaFile.GetStream());

                        var photoBlobModPlusId = new PhotoBlobModelPlusId
                        {
                           //Image = ConvertStreamToByteArrary(mediaFile.GetStream())
                            CreatedAt = photo.CreatedAt,
                            Image = bytes,
                            Id = photo.Id,
                            CreatedAtString = photo.CreatedAtString,
                        };

                        try
                        {

                            //HitPostApi with PhotoBlob + photo.Title + Id // if ID not retained, then it create a totally new one and not patch the old one
                            var returnedPhoto = await APIService.PostPhotoBlobPlusId(photoBlobModPlusId, photo.Title);

                            //                 Get back a PhotoModel photo
                            //Locally Update (PATCH) this PhotoModelPhoto


                            //var photo = await APIService.PostPhotoBlob(photoBlob, photoTitle).ConfigureAwait(false);

                            //if (returnedPhoto is null)
                                //return Task;

                            //SAVE calls INSERT OR REPLACE
                            PhotoDatabase.SavePhoto(returnedPhoto);

                        }
                        catch (Exception e)
                        {
                            //OnSavePhotoFailed(e.Message);
                        }
           
                        //var photo = new PhotoModel() 
                        //{ 
                        //    Title = photo.Title,
                            

                        //};
                    //MAKE SURE TO UPDATE THESE TOO! IE. THESE LOCAL ONES NEED PATCHING.

                    }
            }
            foreach (var photo in photosToAddToLocalDatabase)
                savephotoTaskList.Add(PhotoDatabase.SavePhoto(photo));

            //NOTE - No "Local only file locations" should ever hit cloud so Patch will only be updates to Cloud compatibile entries
            foreach (var photo in photosToPatchToRemoteDatabase)
                savephotoTaskList.Add(APIService.PatchPhoto(photo, photo.Title));

            foreach (var photo in photosToPatchToLocalDatabase)
                savephotoTaskList.Add(PhotoDatabase.UpdatePhoto(photo));

            await Task.WhenAll(savephotoTaskList);
        }
    }
}
