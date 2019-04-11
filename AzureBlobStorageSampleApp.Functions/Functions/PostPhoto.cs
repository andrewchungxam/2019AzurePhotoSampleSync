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
    public static class PostPhoto
    {
        #region Methods
        [FunctionName(nameof(PostPhoto))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "PostPhoto/{title}")]HttpRequestMessage req, string title, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                var photoModel = await JsonService.DeserializeMessage<PhotoModel>(req).ConfigureAwait(false);
                //var photo = await PhotosBlobStorageService.SavePhoto(imageBlobWithId.Image, title).ConfigureAwait(false);

                ////ADDING ID COMPATABILITY
                //photo.Id = imageBlobWithId.Id;

                //ALREADY ID COMPATIBLE
                //await PhotoDatabaseService.InsertPhoto(photo).ConfigureAwait(false);
                await PhotoDatabaseService.InsertPhoto(photoModel).ConfigureAwait(false);

                return new CreatedResult(photoModel.Url, photoModel);
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
