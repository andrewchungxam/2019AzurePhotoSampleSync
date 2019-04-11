using System;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;

namespace AzureBlobStorageSampleApp.Services
{
    public class ComputerVisionService
    {
        private const string subscriptionKey = "";

        public ComputerVisionClient computerVisionClient; 

        public ComputerVisionService()
        {
            computerVisionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey));
            //computerVision.Endpoint = "https://eastus.api.cognitive.microsoft.com/";  //as listed in portal
            computerVisionClient.Endpoint = "https://eastus.api.cognitive.microsoft.com";  //as listed in sample
        }
    }
}
