namespace tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ultraocr;

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
}
