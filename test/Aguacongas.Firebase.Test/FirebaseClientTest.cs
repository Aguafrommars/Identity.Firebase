using Aguacongas.Firebase;
using Aguacongas.Firebase.TokenManager;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
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
            }, new FirebaseOptions
            {
                DatabaseUrl = "http://test"
            });

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.PostAsync(null, ""));
        }

        [Fact]
        public void PostAsync_should_throw_argument_null_exception_if_url_is_empty()
        {
            var sut = CreateSut((request, cancellationToken) =>
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }, new FirebaseOptions
            {
                DatabaseUrl = "http://test"
            });

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.PostAsync("", ""));
        }

        [Fact]
        public void PostAsync_should_throw_argument_null_exception_if_data_is_null()
        {
            var sut = CreateSut((request, cancellationToken) =>
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }, new FirebaseOptions
            {
                DatabaseUrl = "http://test"
            });

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.PostAsync<string>("/", null));
        }

        [Fact]
        public async Task PostAsync_should_sanetize_url()
        {
            var options = new FirebaseOptions
            {
                DatabaseUrl = "http://test",                
            };

            var sut = CreateSut((request, cancellationToken) =>
            {
                Assert.Equal("http://test/test.json?auth=[ID_TOKEN]", request.RequestUri.OriginalString);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"name\": \"test\"}")
                });
            }, options);

            var response = await sut.PostAsync("test", "test");
            Assert.Equal("test", response);

            options.DatabaseUrl = "http://test/";
            response = await sut.PostAsync("/test.json", "test");
            Assert.Equal("test", response);
        }

        [Fact]
        public void PutAsync_should_throw_argument_null_exception_if_data_is_null()
        {
            var sut = CreateSut((request, cancellationToken) =>
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }, new FirebaseOptions
            {
                DatabaseUrl = "http://test"
            });

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.PutAsync<string>("/", null));
        }

        [Fact]
        public async Task PutAsync_should_sanetize_url()
        {
            var options = new FirebaseOptions
            {
                DatabaseUrl = "http://test",
            };

            var sut = CreateSut((request, cancellationToken) =>
            {
                Assert.Equal("http://test/test.json?auth=[ID_TOKEN]", request.RequestUri.OriginalString);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject("test"))
                });
            }, options);

            var response = await sut.PutAsync("test", "test");
            Assert.Equal("test", response);

            options.DatabaseUrl = "http://test/";
            response = await sut.PutAsync("/test.json", "test");
            Assert.Equal("test", response);
        }


        [Fact]
        public void PatchAsync_should_throw_argument_null_exception_if_data_is_null()
        {
            var sut = CreateSut((request, cancellationToken) =>
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }, new FirebaseOptions
            {
                DatabaseUrl = "http://test"
            });

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.PatchAsync<string>("/", null));
        }

        [Fact]
        public async Task PatchAsync_should_sanetize_url()
        {
            var options = new FirebaseOptions
            {
                DatabaseUrl = "http://test",
            };

            var sut = CreateSut((request, cancellationToken) =>
            {
                Assert.Equal("http://test/test.json?auth=[ID_TOKEN]", request.RequestUri.OriginalString);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject("test"))
                });
            }, options);

            var response = await sut.PatchAsync("test", "test");
            Assert.Equal("test", response);

            options.DatabaseUrl = "http://test/";
            response = await sut.PatchAsync("/test.json", "test");
            Assert.Equal("test", response);
        }

        [Fact]
        public async Task DeleteAsync_should_sanetize_url()
        {
            var options = new FirebaseOptions
            {
                DatabaseUrl = "http://test",
            };

            var sut = CreateSut((request, cancellationToken) =>
            {
                Assert.Equal("http://test/test.json?auth=[ID_TOKEN]", request.RequestUri.OriginalString);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(null))
                });
            }, options);

            await sut.DeleteAsync("test");
            
            options.DatabaseUrl = "http://test/";
            await sut.DeleteAsync("/test.json");            
        }

        [Fact]
        public async Task GetAsync_should_sanetize_url()
        {
            var options = new FirebaseOptions
            {
                DatabaseUrl = "http://test",
            };

            var sut = CreateSut((request, cancellationToken) =>
            {
                Assert.Equal("http://test/test.json?auth=[ID_TOKEN]", request.RequestUri.OriginalString);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(null))
                });
            }, options);

            var result = await sut.GetAsync<string>("test");
            Assert.Null(result);
        }

        private FirebaseClient CreateSut(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc, FirebaseOptions options)
        {
            var client = new HttpClient(new DelegatingHandlerStub(handlerFunc));

            var optionsMock = new Mock<IOptions<FirebaseOptions>>();
            optionsMock.SetupGet(m => m.Value).Returns(options);

            var tokenOptions = new EmailPasswordOptions
            {
                Email = "test",
                ApiKey = "test",
                Password = "test",
                SignUpUrl = "http://auth"
            };
            var tokenOptionsMock = new Mock<IOptions<EmailPasswordOptions>>();
            tokenOptionsMock.SetupGet(m => m.Value).Returns(tokenOptions);

            options.FirebaseTokenManager = new EmailPasswordTokenManager(new HttpClient(new DelegatingHandlerStub((request, cancellationToken) =>
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

            return new FirebaseClient(client, optionsMock.Object);
        }
    }
}
