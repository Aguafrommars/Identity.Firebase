using Aguacongas.Identity.Firebase;
using Microsoft.Extensions.Options;
using Moq;
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
            }, new FirebaseOptions());

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.PostAsync<string>(null, null));
        }

        [Fact]
        public void PostAsync_should_throw_argument_null_exception_if_url_is_empty()
        {
            var sut = CreateSut((request, cancellationToken) =>
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }, new FirebaseOptions());

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.PostAsync<string>("", null));
        }

        [Fact]
        public void PostAsync_should_throw_argument_null_exception_if_data_is_null()
        {
            var sut = CreateSut((request, cancellationToken) =>
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }, new FirebaseOptions());

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.PostAsync<string>("/", null));
        }

        [Fact]
        public async Task PostAsync_should_sanetize_url()
        {
            var options = new FirebaseOptions
            {
                DatabaseUrl = "http://test"
            };

            var sut = CreateSut((request, cancellationToken) =>
            {
                if (request.RequestUri.OriginalString == options.SignUpUrl + "?key=")
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{ \"idToken\": \"[ID_TOKEN]\",\"expiresIn\": \"3600\" }")
                    });
                }
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

        private FirebaseClient CreateSut(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc, FirebaseOptions options)
        {
            var client = new HttpClient(new DelegatingHandlerStub(handlerFunc));
            var optionsMock = new Mock<IOptions<FirebaseOptions>>();
            optionsMock.SetupGet(m => m.Value).Returns(options);
            return new FirebaseClient(client, optionsMock.Object);
        }
    }
}
