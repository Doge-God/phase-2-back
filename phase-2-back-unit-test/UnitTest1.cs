using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using phase_2_back.Controllers;
using System.Web.Http;

namespace phase_2_back_unit_test
{
    public class Tests
    {
        private personInfoController controller;
        //private HttpClient client;

        public class FakeHttpMessageHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            }
        }

        [SetUp]
        public void Setup()
        {
            var config = Substitute.For<IConfiguration>();
            var clientFactory = Substitute.For<IHttpClientFactory>();
            controller = new personInfoController(clientFactory, config);
        }

        [Test]
        public void TestInvalidDeleteIndex()
        {
            IActionResult result = controller.deletePersonInfo(0);
            var badresult = result as BadRequestObjectResult;
            Assert.That(400 == badresult.StatusCode, Is.True);
        }

        [Test]
        public void TestReturnEmptyList()
        {
            IActionResult result = controller.getPersonInfoList();
            var goodResult = result as OkObjectResult;
            Assert.That(200 == goodResult.StatusCode, Is.True);
        }

        
    }
}