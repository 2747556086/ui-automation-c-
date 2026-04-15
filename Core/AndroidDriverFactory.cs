using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;

namespace CSharpAppiumPOM.Core;

public sealed class AndroidDriverFactory
{
    private readonly SimpleLogger _logger;

    public AndroidDriverFactory(SimpleLogger logger)
    {
        _logger = logger;
    }

    public AndroidDriver CreateDriver()
    {
        var serverUrl = Environment.GetEnvironmentVariable("APPIUM_SERVER_URL") ?? "http://127.0.0.1:4723";
        var deviceName = Environment.GetEnvironmentVariable("ANDROID_DEVICE_NAME") ?? "AndroidDevice";
        var udid = Environment.GetEnvironmentVariable("ANDROID_UDID");

        var options = new AppiumOptions
        {
            PlatformName = "Android",
            AutomationName = "UiAutomator2",
            DeviceName = deviceName
        };

        if (!string.IsNullOrWhiteSpace(udid))
        {
            options.AddAdditionalAppiumOption("udid", udid);
        }

        options.AddAdditionalAppiumOption("newCommandTimeout", 120);
        options.AddAdditionalAppiumOption("autoGrantPermissions", true);
        options.AddAdditionalAppiumOption("noReset", true);

        _logger.Info($"连接 Appium Server: {serverUrl}");
        var driver = new AndroidDriver(new Uri(serverUrl), options, TimeSpan.FromSeconds(60));
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(300);
        return driver;
    }
}
