using DSharpPlus.Entities;
using MutinyBot.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MutinyBot.Services
{
    /// <summary>
    /// Service responsible for interacting with the Imgur API.
    /// </summary>
    /// <remarks>We can consider Imgur to host images indefinitely.</remarks>
    public class ImgurService
    {
        private string Authorization { get; set; }
        private static RestClient RestClient { get; set; }
        public ImgurService(MutinyBotConfig config)
        {
            Authorization = (config.Imgur.ClientId != null) ? $"Client-ID {config.Imgur.ClientId}" : String.Empty;

            var options = new RestClientOptions("https://api.imgur.com/3/")
            {
                ThrowOnAnyError = true,
                Timeout = 4000,
            };
            RestClient = new RestClient(options);
        }

        /// <summary>
        /// Uploads an image from the given url to Imgurs' servers.
        /// </summary>
        /// <param name="imageUrl">The url that the image is currently hosted on.</param>
        /// <returns>A triple tuple containing the <see cref="RestResponse"/> of the request, along with the url and the delete hash of the uploaded image if successful.</returns>
        public async Task<(RestResponse response, string? imgurUrl, string? deleteHash)> UploadImage(string imageUrl)
        {
            var request = new RestRequest("image", Method.Post)
                    .AddHeader("Authorization", Authorization)
                    .AddParameter("image", imageUrl);

            RestResponse response = new();
            try
            {
                response = await RestClient.ExecuteAsync(request);
            }
            catch (TimeoutException ex)
            {
                response.ResponseStatus = ResponseStatus.TimedOut;
                response.ErrorMessage = ex.Message;
                response.ErrorException = ex;

                return (response, null, null);
            }
            catch
            {
                throw;
            }

            if (response.IsSuccessful && response.StatusCode == HttpStatusCode.OK)
            {
                JObject jResponse = JObject.Parse(response.Content);
                JObject jData = (JObject)jResponse["data"];

                var url = jData["link"].ToString();
                var hash = jData["deletehash"].ToString();

                return (response, url, hash);
            }
            else
            {
                return (response, null, null);
            }
        }

        /// <summary>
        /// Sends a request to Imgur to delete an image from their server using its' delete hash.
        /// </summary>
        /// <param name="deleteHash">The delete hash of the particular image to remove.</param>
        /// <returns>A tuple containing the <see cref="RestResponse"/> of the request and a bool indicating if the request was successful or not.</returns>
        public async Task<(RestResponse response, bool success)> DeleteImage(string deleteHash)
        {
            var request = new RestRequest($"image/{deleteHash}", Method.Delete);
            request.AddHeader("Authorization", Authorization);

            RestResponse response = new();
            try
            {
                response = await RestClient.ExecuteAsync(request);
                return (response, response.IsSuccessful);
            }
            catch (TimeoutException ex)
            {
                response.ResponseStatus = ResponseStatus.TimedOut;
                response.ErrorMessage = ex.Message;
                response.ErrorException = ex;

                return (response, false);
            }
            catch
            {
                throw;
            }
        }
    }
}
