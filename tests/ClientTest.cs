namespace tests;

using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Ultraocr;
using Ultraocr.Enums;

[TestClass]
public class ClientTest
{
    [TestMethod]
    public void TestNew()
    {
        var client = new Client();
        Assert.IsNotNull(client);
        Assert.IsInstanceOfType<Client>(client);
    }

    [TestMethod]
    public void TestNewHttp()
    {
        var client = new Client(new HttpClient());
        Assert.IsNotNull(client);
        Assert.IsInstanceOfType<Client>(client);
    }

    [TestMethod]
    public void TestNewAuto()
    {
        var client = new Client("123", "123", 60);
        Assert.IsNotNull(client);
        Assert.IsInstanceOfType<Client>(client);
    }

    [TestMethod]
    public void TestNewFull()
    {
        var client = new Client(new HttpClient(), "123", "123", 60);
        Assert.IsNotNull(client);
        Assert.IsInstanceOfType<Client>(client);
    }

    [TestMethod]
    public async Task TestAuthenticate()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"token\":\"123\"}"),
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        Assert.IsNotNull(client);
        await client.Authenticate("123", "123", 60);
    }

    [TestMethod]
    public async Task TestAuthenticateFail()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        await Assert.ThrowsExceptionAsync<HttpRequestException>(async () =>
        {
            await client.Authenticate("123", "123", 60);
        });
    }

    [TestMethod]
    public async Task TestGenerateUrl()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"id\":\"123\",\"exp\":1000,\"status_url\":\"123\",\"urls\":{\"document\":\"123\"}}"),
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        var res = await client.GenerateSignedUrl("cnh", Resource.Job, new Dictionary<string, object>(), []);
        Assert.AreEqual("123", res.Id);
    }

    [TestMethod]
    public async Task TestGenerateUrlFail()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        await Assert.ThrowsExceptionAsync<HttpRequestException>(async () =>
        {
            await client.GenerateSignedUrl("cnh", Resource.Job, new Dictionary<string, object>(), []);
        });
    }

    [TestMethod]
    public async Task TestGetBatchStatus()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"batch_ksuid\":\"123\",\"created_at\":\"1000\",\"service\":\"cnh\",\"status\":\"waiting\"}"),
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        var res = await client.GetBatchStatus("123");
        Assert.AreEqual("123", res.BatchKsuid);
        Assert.AreEqual("cnh", res.Service);
        Assert.AreEqual("waiting", res.Status);
    }

    [TestMethod]
    public async Task TestGetBatchStatusFail()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        await Assert.ThrowsExceptionAsync<HttpRequestException>(async () =>
        {
            await client.GetBatchStatus("123");
        });
    }

    [TestMethod]
    public async Task TestGetJobResult()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"job_ksuid\":\"123\",\"created_at\":\"1000\",\"service\":\"cnh\",\"status\":\"waiting\"}"),
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        var res = await client.GetJobResult("123", "123");
        Assert.AreEqual("123", res.JobKsuid);
        Assert.AreEqual("cnh", res.Service);
        Assert.AreEqual("waiting", res.Status);
    }

    [TestMethod]
    public async Task TestGetJobResultFail()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        await Assert.ThrowsExceptionAsync<HttpRequestException>(async () =>
        {
            await client.GetJobResult("123", "123");
        });
    }

    [TestMethod]
    public async Task TestSendJobSingleStep()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"id\":\"123\",\"status_url\":\"123\"}"),
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        var res = await client.SendJobSingleStep("cnh", "123", [], []);
        Assert.AreEqual("123", res.Id);
    }

    [TestMethod]
    public async Task TestSendJobSingleStepFail()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        await Assert.ThrowsExceptionAsync<HttpRequestException>(async () =>
        {
            await client.SendJobSingleStep("cnh", "123", [], []);
        });
    }

    [TestMethod]
    public async Task TestSendJob()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"id\":\"123\",\"exp\":1000,\"status_url\":\"123\",\"urls\":{\"document\":\"http://locahost:8080\"}}"),
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        var res = await client.SendJob("cnh", "./tests.runtimeconfig.json", [], []);
        Assert.AreEqual("123", res.Id);
    }

    [TestMethod]
    public async Task TestSendJobFail()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        await Assert.ThrowsExceptionAsync<HttpRequestException>(async () =>
        {
            await client.SendJob("cnh", "./tests.runtimeconfig.json", [], []);
        });
    }

    [TestMethod]
    public async Task TestSendJobBase64()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"id\":\"123\",\"exp\":1000,\"status_url\":\"123\",\"urls\":{\"document\":\"http://locahost:8080\"}}"),
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        var res = await client.SendJobBase64("cnh", [], [], []);
        Assert.AreEqual("123", res.Id);
    }

    [TestMethod]
    public async Task TestSendJobBase64Fail()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        await Assert.ThrowsExceptionAsync<HttpRequestException>(async () =>
        {
            await client.SendJobBase64("cnh", [], [], []);
        });
    }

    [TestMethod]
    public async Task TestSendJobFull()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"id\":\"123\",\"exp\":1000,\"status_url\":\"123\",\"urls\":{\"document\":\"http://locahost:8080\",\"selfie\":\"http://locahost:8080\",\"extra_document\":\"http://locahost:8080\"}}"),
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        var res = await client.SendJob("cnh", "./tests.runtimeconfig.json", [], []);
        Assert.AreEqual("123", res.Id);
    }
}
