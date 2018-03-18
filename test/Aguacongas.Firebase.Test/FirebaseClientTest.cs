using Aguacongas.Firebase;
using Aguacongas.Firebase.Http;
using Aguacongas.Firebase.TokenManager;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongase.Identity.Firebase.Test
{
    public class FirebaseClientTest
    {
        [Fact]
        public void PostAsync_should_throw_argument_null_exception_if_url_is_null()
        {
            var sut = CreateSut((request, cancellationToken) =>
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            });

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.PostAsync(null, ""));
        }

        [Fact]
        public void PostAsync_should_throw_argument_null_exception_if_url_is_empty()
        {
            var sut = CreateSut((request, cancellationToken) =>
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            });

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.PostAsync("", ""));
        }

        [Fact]
        public void PostAsync_should_throw_argument_null_exception_if_data_is_null()
        {
            var sut = CreateSut((request, cancellationToken) =>
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            });

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.PostAsync<string>("/", null));
        }

        [Fact]
        public async Task PostAsync_should_sanetize_url()
        {
            var sut = CreateSut((request, cancellationToken) =>
            {
                Assert.Equal("http://test/test.json?auth=[ID_TOKEN]", request.RequestUri.ToString());
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"name\": \"test\"}")
                });
            });

            var response = await sut.PostAsync("test", "test");
            Assert.Equal("test", response.Data);

            response = await sut.PostAsync("/test.json", "test");
            Assert.Equal("test", response.Data);
        }

        [Fact]
        public void PutAsync_should_throw_argument_null_exception_if_data_is_null()
        {
            var sut = CreateSut((request, cancellationToken) =>
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            });

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.PutAsync<string>("/", null));
        }

        [Fact]
        public async Task PutAsync_should_sanetize_url()
        {
            var sut = CreateSut((request, cancellationToken) =>
            {
                Assert.Equal("http://test/test.json?auth=[ID_TOKEN]", request.RequestUri.ToString());
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject("test"))
                });
            });

            var response = await sut.PutAsync("test", "test");
            Assert.Equal("test", response.Data);

            response = await sut.PutAsync("/test.json", "test");
            Assert.Equal("test", response.Data);
        }


        [Fact]
        public void PatchAsync_should_throw_argument_null_exception_if_data_is_null()
        {
            var sut = CreateSut((request, cancellationToken) =>
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            });

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.PatchAsync<string>("/", null));
        }

        [Fact]
        public async Task PatchAsync_should_sanetize_url()
        {
            var sut = CreateSut((request, cancellationToken) =>
            {
                Assert.Equal("http://test/test.json?auth=[ID_TOKEN]", request.RequestUri.ToString());
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject("test"))
                });
            });

            var response = await sut.PatchAsync("test", "test");
            Assert.Equal("test", response.Data);

            response = await sut.PatchAsync("/test.json", "test");
            Assert.Equal("test", response.Data);
        }

        [Fact]
        public async Task DeleteAsync_should_sanetize_url()
        {
            var sut = CreateSut((request, cancellationToken) =>
            {
                Assert.Equal("http://test/test.json?auth=[ID_TOKEN]", request.RequestUri.ToString());
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(null))
                });
            });

            await sut.DeleteAsync("test");
            await sut.DeleteAsync("/test.json");            
        }

        [Fact]
        public async Task GetAsync_should_sanetize_url()
        {
            var sut = CreateSut((request, cancellationToken) =>
             {
                 Assert.Equal("http://test/test.json?auth=[ID_TOKEN]", request.RequestUri.ToString());
                 return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                 {
                     Content = new StringContent(JsonConvert.SerializeObject(null))
                 });
             });

            var result = await sut.GetAsync<string>("test");
            Assert.Null(result.Data);
        }

        private FirebaseClient CreateSut(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc)
        {
            var tokenOptions = new EmailPasswordOptions
            {
                Email = "test",
                ApiKey = "test",
                Password = "test",
                SignUpUrl = "http://auth"
            };
            var tokenOptionsMock = new Mock<IOptions<EmailPasswordOptions>>();
            tokenOptionsMock.SetupGet(m => m.Value).Returns(tokenOptions);

            var tokenManager = new EmailPasswordTokenManager(new HttpClient(new DelegatingHandlerStub((request, cancellationToken) =>
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new AuthResponse
                    {
                        DisplayName = "test",
                        Email = "test",
                        ExpiresIn = 3600,
                        IdToken = "[ID_TOKEN]",
                        Kind = "test",
                        LocalId = "test",
                        RefreshToken = "test",
                        Registered = false
                    }))
                });
            })), tokenOptionsMock.Object);

            var client = new HttpClient(new FirebaseAuthenticationHandler(tokenManager, new DelegatingHandlerStub(handlerFunc)));

            return new FirebaseClient(client, new FirebaseOptions { DatabaseUrl = "http://test" } );
        }
    }
}
