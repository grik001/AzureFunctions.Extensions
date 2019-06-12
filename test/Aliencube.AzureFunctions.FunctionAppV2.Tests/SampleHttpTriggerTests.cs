using System.Threading.Tasks;

using Aliencube.AzureFunctions.Extensions.DependencyInjection.Abstractions;
using Aliencube.AzureFunctions.FunctionAppCommon.Functions;
using Aliencube.AzureFunctions.Tests.Fakes;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace Aliencube.AzureFunctions.FunctionAppV2.Tests
{
    /// <summary>
    /// This represents the test entity for the <see cref="SampleHttpTrigger"/> class.
    /// </summary>
    [TestClass]
    public class SampleHttpTriggerTests
    {
        [TestMethod]
        public async Task Given_Request_Run_Should_Return_Result()
        {
            var message = "hello world";
            var result = new OkObjectResult(message);

            var function = new Mock<ISampleHttpFunction>();
            function.Setup(p => p.InvokeAsync<HttpRequest, IActionResult>(It.IsAny<HttpRequest>(), It.IsAny<FunctionOptionsBase>())).ReturnsAsync(result);

            var factory = new Mock<IFunctionFactory>();
            factory.Setup(p => p.Create<ISampleHttpFunction, ILogger>(It.IsAny<ILogger>())).Returns(function.Object);

            SampleHttpTrigger.Factory = factory.Object;

            var id = 1;
            var category = "heros";

            var query = new FakeQueryCollection();
            query["name"] = "ipsum";

            var req = new Mock<HttpRequest>();
            req.SetupGet(p => p.Query).Returns(query);

            var log = new Mock<ILogger>();
            var response = await SampleHttpTrigger.GetSample(req.Object, id, category, log.Object).ConfigureAwait(false);

            response.Should().BeOfType<OkObjectResult>();
            (response as OkObjectResult).Value.Should().Be(message);
        }
    }
}