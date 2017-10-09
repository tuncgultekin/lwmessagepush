
using LWMessagePush.DTOs;
using LWMessagePush.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class ChatAppIntegrationTests
    {
        private string ClientAddress = "http://localhost:7471";
        private HttpClient PrepareClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ClientAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        [Fact]
        public async void LoginTest()
        {
            var client = PrepareClient();

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("userName", "test")
            });
            
            HttpResponseMessage postResponse = await client.PostAsync("api/login", formContent);
            postResponse.EnsureSuccessStatusCode();

            HttpResponseMessage response = await client.GetAsync("/api/GetUserList");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Assert.Contains("\"username\":\"test\"", result);
            }
                        
        }

        [Fact]
        public async void LogoutTest()
        {
            var client = PrepareClient();
            var expectedKey = "\"username\":\"logouttest\"";

            PrepareForLogout(client, expectedKey);

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("userName", "test")
            });

            var postResponse = await client.PostAsync("api/logout", formContent);
            postResponse.EnsureSuccessStatusCode();

            var response = await client.GetAsync("/api/GetUserList");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Assert.DoesNotContain(expectedKey, result);
            }

        }

        private async void PrepareForLogout(HttpClient client, string ecpectedKey)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("userName", "test")
            });                       

            HttpResponseMessage postResponse = await client.PostAsync("api/login", formContent);
            postResponse.EnsureSuccessStatusCode();

            HttpResponseMessage response = await client.GetAsync("/api/GetUserList");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();                
            }
        }

        [Fact]
        public async void SendMessageTest()
        {
            var client = PrepareClient();

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("to", "receiver"),
                new KeyValuePair<string, string>("from", "sender"),
                new KeyValuePair<string, string>("content", "test"),

            });
            
            HttpResponseMessage postResponse = await client.PostAsync("api/SendMessageToUser", formContent);
            Assert.True(postResponse.EnsureSuccessStatusCode().IsSuccessStatusCode);
        }

        [Fact]
        public async void PingTest()
        {
            var client = PrepareClient();

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("userName", "test")
            });

            HttpResponseMessage postResponse = await client.PostAsync("api/ping", formContent);
            Assert.True(postResponse.EnsureSuccessStatusCode().IsSuccessStatusCode);
        }
    }
}
