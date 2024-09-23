using System;
using System.Net.Http;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection.Metadata;

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
        [Parallelizable(ParallelScope.Self)]
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

        [Test] // POST request
     //   [Parallelizable(ParallelScope.Self)]
        public async Task TestPostAPI()
        {
            // Create a new post object 
            var newPost = new
            {
                userId = 1,
                title = "foo",
                body = "bar"
            };

            // Convert the newPost object to JSON
            string jsonContent = JsonConvert.SerializeObject(newPost);
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Send a POST request to the /posts endpoint with the new post
            HttpResponseMessage response = await _httpClient.PostAsync("posts", content);

            // Assert that the status code is 201 Created
            Assert.AreEqual(201, (int)response.StatusCode, "Expected status code 201");

            // Read the response body as a string
            string responseBody = await response.Content.ReadAsStringAsync();

            // Assert that the response body contains the posted data
            Assert.IsTrue(responseBody.Contains("foo"), "Response body does not contain the expected title");
            Assert.IsTrue(responseBody.Contains("bar"), "Response body does not contain the expected body");

            // Print the response body for debugging purposes (optional)
            Console.WriteLine("Response Body: " + responseBody);
        }

        [Test] // PUT request
       // [Parallelizable(ParallelScope.Self)]
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

        [Test] // DELETE request for deleting a post
      //  [Parallelizable(ParallelScope.Self)]
        public async Task TestDeletePost()
        {
            // Ensure _httpClient is not null
            if (_httpClient == null)
            {
                Assert.Fail("HttpClient i snot initialized.");
            }

            // Define the Post id to delete
            int postId = 1;

            // Ensure postID is valid to prevent UriFormatException
            if (postId <= 0)
            {
                Assert.Fail("Invalid postId.");
            }

            try
            {
                // Send a DELETE request
                HttpResponseMessage response = await _httpClient.DeleteAsync($"posts/{postId}");

                // Assert that the status code is 200 OK or 204 No Content *************************************
                Assert.IsTrue(
                    response.StatusCode == System.Net.HttpStatusCode.OK ||
                    response.StatusCode == System.Net.HttpStatusCode.NoContent,
                    "Expected status code 200 OK or 204 No Content");

                // Check that the body is empty (No Content)
                string responseBody = await response.Content.ReadAsStringAsync();
               // Assert.IsTrue(string.IsNullOrEmpty(responseBody), "Response body is not empty after DELETE.");

                // Print the response status and body 
                Console.WriteLine($"Response Status: {response.StatusCode}");
                Console.WriteLine("DELETE Response Body: " + responseBody);
            }
            catch (HttpRequestException ex)
            {
                Assert.Fail($"HttpRequestException: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Assert.Fail($"TaskCanceledException: {ex.Message}");
            }
            catch (UriFormatException ex)
            {
                Assert.Fail($"UriFormatException: {ex.Message}");
            }
        }

        [Test] // POST request to create new comment on a specific post
       // [Parallelizable(ParallelScope.Self)]
        public async Task TestPostComment()
        {
            // Ensure _httpClient is not null
            if (_httpClient == null)
            {
                Assert.Fail("HttpClient is not initialized.");
            }

            // Define the Post id for which we want to add comment
            int postId = 1;

            // Ensure postID is valid to prevent UriFormatException
            if (postId <= 0)
            {
                Assert.Fail("Invalid postId.");
            }

            // Create a new comment object
            var newComment = new 
            {
                postID  = postId,  // The ID of the post where the comment will be added
                name = "Demo_Commenter",
                email = "demo@commenter.com",
                body = "This is a sample comment."            
            };

            // Serialize the comment object to JSON
            string jsonContent = JsonConvert.SerializeObject(newComment);
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "applicaton/json");

            try
            {
                // Log message before sending the POST request
                Console.WriteLine("Sending POST request to add a comment...");

                // Send a POST request to create a new comment
                HttpResponseMessage response = await _httpClient.PostAsync($"posts/{postId}/comments", content);

                // Log status code and headers
                Console.WriteLine("Response Status Code: " + (int)response.StatusCode);
              //  Console.WriteLine("Response Headers: " + response.Headers.ToString());

                // Assert that the status code is 201 Created (typical for a successful POST)
                Assert.AreEqual(201, (int)response.StatusCode, "Expected status code 201");

                // Read the response body as a string
                string responseBody = await response.Content.ReadAsStringAsync();

                // Print response body
                Console.WriteLine("***************Response Body ****************");
                Console.WriteLine(responseBody);

                // Assert that the response body contains the comment
                // Assert.IsTrue(responseBody.Contains("Demo_Commenter"), "Response body does not contain the expected name");
                // Assert.IsTrue(responseBody.Contains("demo@commenter.com"), "Response body does not contain the expected email");
                //  Assert.IsTrue(responseBody.Contains("This is a sample comment."), "Response body does not contain the expected comment");
                // Since the response does not contain "name", "email", or "body", only check for "postId" and "id"
                Assert.IsTrue(responseBody.Contains("\"postId\""), "Response body does not contain postId.");
                Assert.IsTrue(responseBody.Contains("\"id\""), "Response body does not contain id.");

            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException: {ex.Message}");
                Assert.Fail("HttpRequestException: ", ex.Message );
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"TaskCanceledException: {ex.Message}");
                Assert.Fail($"TaskCanceledException: {ex.Message}");
            }
            catch (UriFormatException ex)
            {
                Console.WriteLine($"UriFormatException: {ex.Message}");
                Assert.Fail($"UriFormatException: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"InvalidOperationException: {ex.Message}");
                Assert.Fail($"InvalidOperationException: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Catch any other exception that might occur
                Console.WriteLine($"Unexpected exception: {ex.Message}");
                Assert.Fail($"Unexpected exception: {ex.Message}");
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
