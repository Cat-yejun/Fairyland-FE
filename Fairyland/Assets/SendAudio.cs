using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace UploadWavFile
{
    class Program
    {
        static async Task Main(string[] args)
        {

            //////////////////////////// 감정 인식 하는 부분 
            var filePath = "output.mp3"; // Replace with your WAV file path
            var serverUrl = "http://a249-125-132-126-243.ngrok-free.app/predict-emotion/"; // Replace with your FastAPI server URL

            using (var client = new HttpClient())
            {
                using (var form = new MultipartFormDataContent())
                {
                    // Read the file content
                    byte[] fileContent = File.ReadAllBytes(filePath);
                    var fileContentByteArrayContent = new ByteArrayContent(fileContent);
                    fileContentByteArrayContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/wav");

                    // Add the file content to the form
                    form.Add(fileContentByteArrayContent, "audiofile", Path.GetFileName(filePath));

                    // Send POST request
                    HttpResponseMessage response = await client.PostAsync(serverUrl, form);

                    // Check the response
                    if (response.IsSuccessStatusCode)
                    {
                        UnityEngine.Debug.Log("response get!");
                        string responseData = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Response: {responseData}");
                        UnityEngine.Debug.Log(responseData);
                    }
                    else
                    {
                        UnityEngine.Debug.Log("response didn't get!");
                        Console.WriteLine($"Error: {response.StatusCode}");
                    }
                }
            }

            //            var filePath = "output.mp3"; // Replace with your WAV file path

            ///////////////////////////////////////////////// 유사도 비교하는 부분 
            var groundTruth = "who are you ";
            serverUrl = "http://a249-125-132-126-243.ngrok-free.app/asr-similarity/"; // Replace with your FastAPI server URL

            using (var client = new HttpClient())
            {
                using (var form = new MultipartFormDataContent())
                {
                    // Read the file content
                    byte[] fileContent = File.ReadAllBytes(filePath);
                    var fileContentByteArrayContent = new ByteArrayContent(fileContent);
                    fileContentByteArrayContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/wav");

                    // Add the file content to the form
                    form.Add(fileContentByteArrayContent, "audio_file", Path.GetFileName(filePath));

                    // Add the ground truth to the form
                    var stringContent = new StringContent(groundTruth);
                    form.Add(stringContent, "groundtruth");

                    // Send POST request
                    HttpResponseMessage response = await client.PostAsync(serverUrl, form);

                    // Check the response
                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Response: {responseData}");
                    }
                    else
                    {
                        string errorData = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error: {response.StatusCode}, Details: {errorData}");
                    }
                }
            }

        }
    }
}