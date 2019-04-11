using AzureBlobStorageSampleApp.Pages;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AzureBlobStorageSampleApp
{
    public class App : Application
    {
        public static string LocalAppDirectoryPath {get;set;}
        public static string LocalPhotoFolderName = "LocalPhotosFolder";
        
        public App(){
            this.SetLocalAppDirectory();
            MainPage = new MainPage();
        }  

        void SetLocalAppDirectory()
        { 
            var fileAppDataDirectory = FileSystem.AppDataDirectory;

            var fileAppCacheDirectoryMinusLibrary = fileAppDataDirectory.Replace("Library", "");
            var fileAppCacheDirectoryMinusLibraryPlusDocumentsWithoutFileName = fileAppCacheDirectoryMinusLibrary + "Documents/"; //+ $"{directoryName}/{dateTimeNowStringJpg}";
        
            App.LocalAppDirectoryPath = fileAppCacheDirectoryMinusLibraryPlusDocumentsWithoutFileName;
        }


    }
}
