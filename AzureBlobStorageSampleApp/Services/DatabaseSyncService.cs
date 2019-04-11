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

                        var bytes = File.ReadAllBytes(fullPathForFile);
                        var photoBlobModPlusId = new PhotoBlobModelPlusId
                        {
                            CreatedAt = photo.CreatedAt,
                            Image = bytes,
                            Id = photo.Id,
                            CreatedAtString = photo.CreatedAtString,
                        };

                        try
                        {
                            var returnedPhoto = await APIService.PostPhotoBlobPlusId(photoBlobModPlusId, photo.Title);
                            PhotoDatabase.SavePhoto(returnedPhoto);

                        }
                        catch (Exception e)
                        {
                        }
           
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
