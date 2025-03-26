namespace tests;

using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Ultraocr;
using Ultraocr.Enums;
using Ultraocr.Exceptions;

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
    public void TestSets()
    {
        var client = new Client();
        Assert.IsNotNull(client);
        Assert.IsInstanceOfType<Client>(client);

        client.SetAuthBaseUrl("123");
        client.SetBaseUrl("123");
        client.SetInterval(60);
        client.SetTimeout(60);
        client.SetHttpClient(new HttpClient());
        client.SetAutoRefresh("123", "123", 60);

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
                    Content = new StringContent("{\"job_ksuid\":\"123\",\"created_at\":\"1000\",\"service\":\"cnh\",\"status\":\"done\",\"result\":{\"Document\":{},\"Quantity\":1,\"Time\":\"1\"}}"),
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        var res = await client.GetJobResult("123", "123");
        Assert.AreEqual("123", res.JobKsuid);
        Assert.AreEqual("cnh", res.Service);
        Assert.AreEqual("done", res.Status);
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
    public async Task TestGetJobs()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"jobs\":[{\"job_ksuid\":\"123\",\"created_at\":\"1000\",\"service\":\"cnh\",\"status\":\"waiting\"}, {\"job_ksuid\":\"1234\",\"created_at\":\"1000\",\"service\":\"cnh\",\"status\":\"waiting\"}]}"),
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        var res = await client.GetJobs("123", "123");
        Assert.AreEqual(2, res.Count);
    }

    [TestMethod]
    public async Task TestGetJobsWithToken()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        var i = 0;
        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                if (i == 0)
                {
                    i++;
                    HttpResponseMessage res = new()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("{\"jobs\":[{\"job_ksuid\":\"123\",\"created_at\":\"1000\",\"service\":\"cnh\",\"status\":\"waiting\"}, {\"job_ksuid\":\"1234\",\"created_at\":\"1000\",\"service\":\"cnh\",\"status\":\"waiting\"}],\"nextPageToken\":\"1234\"}"),
                    };

                    return res;
                }

                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"jobs\":[{\"job_ksuid\":\"1235\",\"created_at\":\"1000\",\"service\":\"cnh\",\"status\":\"waiting\"}, {\"job_ksuid\":\"1236\",\"created_at\":\"1000\",\"service\":\"cnh\",\"status\":\"waiting\"}]}"),
                };

                return response;

            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        var res = await client.GetJobs("123", "123");
        Assert.AreEqual(4, res.Count);
    }

    [TestMethod]
    public async Task TestGetJobsFail()
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
            await client.GetJobs("123", "123");
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
        var res = await client.SendJob("cnh", "./tests.runtimeconfig.json", "./tests.runtimeconfig.json","./tests.runtimeconfig.json", [], new()
        {
            { "facematch", "true" },
            { "extra-document", "true" },
        });
        Assert.AreEqual("123", res.Id);
    }

    [TestMethod]
    public async Task TestSendJobSingleStepFull()
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
        var res = await client.SendJobSingleStep("cnh", "./tests.runtimeconfig.json", "./tests.runtimeconfig.json","./tests.runtimeconfig.json", [], new()
        {
            { "facematch", "true" },
            { "extra-document", "true" },
        });
        Assert.AreEqual("123", res.Id);
    }

    [TestMethod]
    public async Task TestSendJobBase64Full()
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
        var res = await client.SendJobBase64("cnh", [], [], [], [], new()
        {
            { "facematch", "true" },
            { "extra-document", "true" },
        });
        Assert.AreEqual("123", res.Id);
    }

    [TestMethod]
    public async Task TestSendBatch()
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
        var res = await client.SendBatch("cnh", "./tests.runtimeconfig.json", [], []);
        Assert.AreEqual("123", res.Id);
    }

    [TestMethod]
    public async Task TestSendBatchFail()
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
            await client.SendBatch("cnh", "./tests.runtimeconfig.json", [], []);
        });
    }

    [TestMethod]
    public async Task TestSendBatchBase64()
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
        var res = await client.SendBatchBase64("cnh", [], [], []);
        Assert.AreEqual("123", res.Id);
    }

    [TestMethod]
    public async Task TestSendBatchBase64Fail()
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
            await client.SendBatchBase64("cnh", [], [], []);
        });
    }

    [TestMethod]
    public async Task TestWaitForJobDone()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"job_ksuid\":\"123\",\"created_at\":\"1000\",\"service\":\"cnh\",\"status\":\"done\"}"),
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        var res = await client.WaitForJobDone("123", "123");
        Assert.AreEqual("123", res.JobKsuid);
    }

    [TestMethod]
    public async Task TestWaitForJobDoneFail()
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
            await client.WaitForJobDone("123", "123");
        });
    }

    [TestMethod]
    public async Task TestWaitForJobDoneFailTimeout()
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
        client.SetTimeout(0);
        await Assert.ThrowsExceptionAsync<JobTimeoutException>(async () =>
        {
            await client.WaitForJobDone("123", "123");
        });
    }

    [TestMethod]
    public async Task TestWaitForBatchDone()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"batch_ksuid\":\"123\",\"created_at\":\"1000\",\"service\":\"cnh\",\"status\":\"done\"}"),
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        var res = await client.WaitForBatchDone("123", false);
        Assert.AreEqual("123", res.BatchKsuid);
    }

    [TestMethod]
    public async Task TestWaitForBatchDoneFail()
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
            await client.WaitForBatchDone("123", false);
        });
    }

    [TestMethod]
    public async Task TestWaitForBatchDoneFailTimeout()
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
        client.SetTimeout(0);
        await Assert.ThrowsExceptionAsync<JobTimeoutException>(async () =>
        {
            await client.WaitForBatchDone("123", false);
        });
    }

    [TestMethod]
    public async Task TestWaitForBatchDoneWithJob()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"job_ksuid\":\"1234\",\"batch_ksuid\":\"123\",\"created_at\":\"1000\",\"service\":\"cnh\",\"status\":\"done\",\"jobs\":[{\"job_ksuid\":\"1234\",\"created_at\":\"1000\",\"result_url\":\"123\",\"status\":\"done\"}]}"),
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        var res = await client.WaitForBatchDone("123", true);
        Assert.AreEqual("123", res.BatchKsuid);
    }

    [TestMethod]
    public async Task TestCreateAndWaitBatch()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"id\":\"123\",\"exp\":1000,\"status_url\":\"123\",\"urls\":{\"document\":\"http://localhost:8080\"},\"batch_ksuid\":\"123\",\"created_at\":\"1000\",\"service\":\"cnh\",\"status\":\"done\"}"),
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        var res = await client.CreateAndWaitBatch("cnh", "./tests.runtimeconfig.json", [], [], false);
        Assert.AreEqual("123", res.BatchKsuid);
    }

    [TestMethod]
    public async Task TestCreateAndWaitBatchFail()
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
            await client.CreateAndWaitBatch("cnh", "./tests.runtimeconfig.json", [], [], false);
        });
    }

    [TestMethod]
    public async Task TestCreateAndWaitBatchFailTimeout()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"id\":\"123\",\"exp\":1000,\"status_url\":\"123\",\"urls\":{\"document\":\"http://localhost:8080\"},\"batch_ksuid\":\"123\",\"created_at\":\"1000\",\"service\":\"cnh\",\"status\":\"waiting\"}"),
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        client.SetTimeout(0);
        await Assert.ThrowsExceptionAsync<JobTimeoutException>(async () =>
        {
            await client.CreateAndWaitBatch("cnh", "./tests.runtimeconfig.json", [], [], false);
        });
    }

    [TestMethod]
    public async Task TestCreateAndWaitBatchWithJob()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"id\":\"123\",\"exp\":1000,\"status_url\":\"123\",\"urls\":{\"document\":\"http://localhost:8080\"},\"job_ksuid\":\"1234\",\"batch_ksuid\":\"123\",\"created_at\":\"1000\",\"service\":\"cnh\",\"status\":\"done\",\"jobs\":[{\"job_ksuid\":\"1234\",\"created_at\":\"1000\",\"result_url\":\"123\",\"status\":\"done\"}]}"),
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        var res = await client.CreateAndWaitBatch("cnh", "./tests.runtimeconfig.json", [], [], true);
        Assert.AreEqual("123", res.BatchKsuid);
    }

    [TestMethod]
    public async Task TestCreateAndWaitJob()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"id\":\"123\",\"exp\":1000,\"status_url\":\"123\",\"urls\":{\"document\":\"http://localhost:8080\"},\"job_ksuid\":\"123\",\"created_at\":\"1000\",\"service\":\"cnh\",\"status\":\"done\"}"),
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        var res = await client.CreateAndWaitJob("cnh", "./tests.runtimeconfig.json", [], []);
        Assert.AreEqual("123", res.JobKsuid);
    }

    [TestMethod]
    public async Task TestCreateAndWaitJobFail()
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
            await client.CreateAndWaitJob("cnh", "./tests.runtimeconfig.json", [], []);
        });
    }

    [TestMethod]
    public async Task TestCreateAndWaitJobFailTimeout()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"id\":\"123\",\"exp\":1000,\"status_url\":\"123\",\"urls\":{\"document\":\"http://localhost:8080\"},\"job_ksuid\":\"123\",\"created_at\":\"1000\",\"service\":\"cnh\",\"status\":\"waiting\"}"),
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient);
        client.SetTimeout(0);
        await Assert.ThrowsExceptionAsync<JobTimeoutException>(async () =>
        {
            await client.CreateAndWaitJob("cnh", "./tests.runtimeconfig.json", [], []);
        });
    }

    [TestMethod]
    public async Task TestCreateAndWaitJobFull()
    {
        var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        httpMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"token\":\"123\",\"id\":\"123\",\"exp\":1000,\"status_url\":\"123\",\"urls\":{\"document\":\"http://localhost:8080\",\"selfie\":\"http://locahost:8080\",\"extra_document\":\"http://locahost:8080\"},\"job_ksuid\":\"123\",\"created_at\":\"1000\",\"service\":\"cnh\",\"status\":\"done\"}"),
                };

                return response;
            })
            .Verifiable();
        var httpClient = new HttpClient(httpMock.Object);
        var client = new Client(httpClient, "123", "123", 60);
        var res = await client.CreateAndWaitJob("cnh", "./tests.runtimeconfig.json", "./tests.runtimeconfig.json", "./tests.runtimeconfig.json", [], new()
        {
            { "facematch", "true" },
            { "extra-document", "true" },
        });
        Assert.AreEqual("123", res.JobKsuid);
    }
}
