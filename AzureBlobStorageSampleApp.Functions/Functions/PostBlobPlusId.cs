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
    public static class PostBlobPlusId
    {
        #region Methods
        [FunctionName(nameof(PostBlobPlusId))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "PostBlobPlusId/{title}")]HttpRequestMessage req, string title, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                var imageBlobWithId = await JsonService.DeserializeMessage<PhotoBlobModelPlusId>(req).ConfigureAwait(false);
                var photo = await PhotosBlobStorageService.SavePhoto(imageBlobWithId.Image, title).ConfigureAwait(false);

                //ADDING ID COMPATABILITY
                photo.Id = imageBlobWithId.Id;

                //ALREADY ID COMPATIBLE
                //await PhotoDatabaseService.InsertPhoto(photo).ConfigureAwait(false);

                photo.CreatedAt = imageBlobWithId.CreatedAt;

                //CARRY OVER ADDIIONAL FIELDS
                photo.Tag1 = imageBlobWithId.Tag1;
                photo.Tag2 = imageBlobWithId.Tag2;
                photo.Tag3 = imageBlobWithId.Tag3;
                photo.Tag4 = imageBlobWithId.Tag4;
                photo.Tag5 = imageBlobWithId.Tag5;
                photo.Tag6 = imageBlobWithId.Tag6;
                photo.Tag7 = imageBlobWithId.Tag7;
                photo.Tag8 = imageBlobWithId.Tag8;
                photo.Tag9 = imageBlobWithId.Tag9;
                photo.Tag10 = imageBlobWithId.Tag10;

                photo.TagsSeperatedWithSpaces = imageBlobWithId.TagsSeperatedWithSpaces;

                photo.CustomTag1 = imageBlobWithId.CustomTag1;
                photo.CustomTag2 = imageBlobWithId.CustomTag2;
                photo.CustomTag3 = imageBlobWithId.CustomTag3;
                photo.CustomTag4 = imageBlobWithId.CustomTag4;
                photo.CustomTag5 = imageBlobWithId.CustomTag5;
                photo.CustomTag6 = imageBlobWithId.CustomTag6;
                photo.CustomTag7 = imageBlobWithId.CustomTag7;
                photo.CustomTag8 = imageBlobWithId.CustomTag8;
                photo.CustomTag9 = imageBlobWithId.CustomTag9;
                photo.CustomTag10 = imageBlobWithId.CustomTag10;

                photo.CustomTagsSeperatedWithSpaces = imageBlobWithId.CustomTagsSeperatedWithSpaces;
                photo.CreatedAtString = imageBlobWithId.CreatedAtString;

                photo.City = imageBlobWithId.City;
                photo.LocationState = imageBlobWithId.LocationState;
                photo.Country = imageBlobWithId.Country;
                photo.CityState = imageBlobWithId.CityState;

                photo.Lat = imageBlobWithId.Lat;
                photo.Long = imageBlobWithId.Long;

                photo.BarcodeString = imageBlobWithId.BarcodeString;

                //BELIEVE THIS SHOULD BE TO ADJUST UPDATE TIME
                await PhotoDatabaseService.InsertUpdatedPhoto(photo).ConfigureAwait(false);


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
