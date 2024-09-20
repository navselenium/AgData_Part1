using System;
using System.Net.Http;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AgData_Part1
{
    public class APITestProject
    {
        private HttpClient? _httpClient;

        [SetUp]
        public void Setup()
        {
                // Initiallize HTTPClient
                _httpClient = new HttpClient();
                // Set the BaseAddress property separately after instantiation
                 _httpClient.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");

                // Set a timeout to prevent TaskCanceledException
                _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        [Test]
        public async Task TestGetPostAPI()
        {
            // Check if _httpClient is not null before use
            if (_httpClient == null)
            {
                Assert.Fail("HttpClient is not initialized.");
            }

            // Send a GET request to the /posts endpoint
            HttpResponseMessage response = await _httpClient.GetAsync("posts");

            // Assert that the status code is 200 OK
            Assert.That((int)response.StatusCode, Is.EqualTo(200), "Expected status code 200");

            // Read the response body as a string
            string responseBody = await response.Content.ReadAsStringAsync();

            // Assert that the response body is not null or empty
            Assert.IsNotNull(responseBody, "Response body is null");
            Assert.IsNotEmpty(responseBody, "Response body is empty");

            // Print the response body for debugging purposes
            Console.WriteLine(responseBody);
            Console.WriteLine("Navpreet******************************");
        }

        [Test] // PUT request
        public async Task TestPutAPI() // This method must be marked as async and return Task
        {
            // Check if _httpClient is not null before use
            if (_httpClient == null)
            {
                Assert.Fail("HttpClient is not initialized.");
            }
            // Define postId to update
            int postId = 1;

            // Ensure postId is valid to prevent UriFormatException
            if (postId <= 0)
            {
                Assert.Fail("Invalid postId.");
            }

            // New data to update the post
            var updatedPost = new
            {
                userId = 1,    // userId typically remains the same
                title = "Updated title",
                body = "Updated body"
            };
            // Serialize the updated post data to JSON
            string jsonContent = JsonConvert.SerializeObject(updatedPost);
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                // Send a PUT request to update the post
                HttpResponseMessage response = await _httpClient.PutAsync($"posts/{postId}", content);

                // Assert that the status code is 200 OK
                Assert.AreEqual(200, (int)response.StatusCode, "Expected status code 200");

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.IsTrue(responseBody.Contains("Updated title"), "Response body does not contain the updated title");
                Assert.IsTrue(responseBody.Contains("Updated body"), "Response body does not contain the updated body");

                Console.WriteLine("Updated Post Response Body: " + responseBody);
            }
            catch (HttpRequestException ex)
            {
                Assert.Fail($"HttpRequestException: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Assert.Fail($"TaskCanceledException: Request timed out. {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Assert.Fail($"InvalidOperationException: {ex.Message}");
            }
            catch (UriFormatException ex)
            {
                Assert.Fail($"UriFormatException: {ex.Message}");
            }
        }


        [TearDown]
        public void TearDown()
        {
            // Dispose safely if initialized
            _httpClient?.Dispose();
        }
    }
}
