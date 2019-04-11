using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using AzureBlobStorageSampleApp.Shared;

namespace AzureBlobStorageSampleApp.Functions
{
    public static class PostBlob
    {
        #region Methods
        [FunctionName(nameof(PostBlob))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "PostBlob/{title}")]HttpRequestMessage req, string title, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                var imageBlob = await JsonService.DeserializeMessage<PhotoBlobModel>(req).ConfigureAwait(false);
                var photo = await PhotosBlobStorageService.SavePhoto(imageBlob.Image, title).ConfigureAwait(false);

                photo.Tag1 = imageBlob.Tag1;
                photo.Tag2 = imageBlob.Tag2;
                photo.Tag3 = imageBlob.Tag3;
                photo.Tag4 = imageBlob.Tag4;
                photo.Tag5 = imageBlob.Tag5;
                photo.Tag6 = imageBlob.Tag6;
                photo.Tag7 = imageBlob.Tag7;
                photo.Tag8 = imageBlob.Tag8;
                photo.Tag9 = imageBlob.Tag9;
                photo.Tag10 = imageBlob.Tag10;

                photo.TagsSeperatedWithSpaces = imageBlob.TagsSeperatedWithSpaces;

                photo.CustomTag1 = imageBlob.CustomTag1;
                photo.CustomTag2 = imageBlob.CustomTag2;
                photo.CustomTag3 = imageBlob.CustomTag3;
                photo.CustomTag4 = imageBlob.CustomTag4;
                photo.CustomTag5 = imageBlob.CustomTag5;
                photo.CustomTag6 = imageBlob.CustomTag6;
                photo.CustomTag7 = imageBlob.CustomTag7;
                photo.CustomTag8 = imageBlob.CustomTag8;
                photo.CustomTag9 = imageBlob.CustomTag9;
                photo.CustomTag10 = imageBlob.CustomTag10;

                photo.CustomTagsSeperatedWithSpaces = imageBlob.CustomTagsSeperatedWithSpaces;
                photo.CreatedAtString = imageBlob.CreatedAtString;

                photo.City = imageBlob.City;
                photo.LocationState = imageBlob.LocationState;
                photo.Country = imageBlob.Country;
                photo.CityState = imageBlob.CityState;

                photo.Lat = imageBlob.Lat;
                photo.Long = imageBlob.Long;

                photo.BarcodeString = imageBlob.BarcodeString;


                await PhotoDatabaseService.InsertPhoto(photo).ConfigureAwait(false);

                return new CreatedResult(photo.Url, photo);
            }
            catch(Exception e)
            {
                log.LogError(e, e.Message);
                return new InternalServerErrorResult();
            }
        }
        #endregion
    }
}



//using System;
//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Web.Http;

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Host;
//using Microsoft.Azure.WebJobs.Extensions.Http;
//using Microsoft.Extensions.Logging;

//using AzureBlobStorageSampleApp.Shared;

//namespace AzureBlobStorageSampleApp.Functions
//{
//    public static class PostBlob
//    {
//        #region Methods
//        [FunctionName(nameof(PostBlob))]
//        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "PostBlob/{title}")]HttpRequestMessage req, string title, ILogger log)
//        {
//            log.LogInformation("C# HTTP trigger function processed a request.");

//            try
//            {
//                var imageBlob = await JsonService.DeserializeMessage<PhotoBlobModel>(req).ConfigureAwait(false);
//                var photo = await PhotosBlobStorageService.SavePhoto(imageBlob.Image, title).ConfigureAwait(false);
                

//                await PhotoDatabaseService.InsertPhoto(photo).ConfigureAwait(false);

//                return new CreatedResult(photo.Url, photo);
//            }
//            catch(Exception e)
//            {
//                log.LogError(e, e.Message);
//                return new InternalServerErrorResult();
//            }
//        }
//        #endregion
//    }
//}
