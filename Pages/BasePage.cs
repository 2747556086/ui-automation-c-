using CSharpAppiumPOM.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace CSharpAppiumPOM.Pages;

public abstract class BasePage
{
    protected readonly AndroidDriver Driver;
    protected readonly SimpleLogger Logger;

    protected BasePage(AndroidDriver driver, SimpleLogger logger)
    {
        Driver = driver;
        Logger = logger;
    }

    protected bool ExistsByText(string text)
    {
        return Driver.FindElements(By.XPath($"//*[@text={ToXPathLiteral(text)}]")).Count > 0;
    }

    protected bool ExistsByTextMatches(string regex)
    {
        // XPath 不支持正则；这里做一个针对当前用例的兼容实现。
        if (regex.Contains("Power", StringComparison.OrdinalIgnoreCase) &&
            regex.Contains("Energy", StringComparison.OrdinalIgnoreCase))
        {
            return Driver.FindElements(By.XPath("//*[contains(@text,'Power') and contains(@text,'Energy')]")).Count > 0;
        }

        return false;
    }

    protected bool ClickByText(string text)
    {
        var elements = Driver.FindElements(By.XPath($"//*[@text={ToXPathLiteral(text)}]"));
        if (elements.Count == 0)
        {
            return false;
        }

        elements[0].Click();
        return true;
    }

    protected string RunAdbShell(string command, int timeoutSeconds = 5)
    {
        try
        {
            var args = new Dictionary<string, object>
            {
                ["command"] = command,
                ["timeout"] = timeoutSeconds * 1000
            };

            var result = Driver.ExecuteScript("mobile: shell", args);
            return result?.ToString() ?? string.Empty;
        }
        catch (Exception ex)
        {
            Logger.Error($"执行 mobile:shell 失败: {command}, {ex.Message}");
            return string.Empty;
        }
    }

    protected string GetCurrentActivityRaw()
    {
        var output = RunAdbShell("dumpsys window");
        if (string.IsNullOrWhiteSpace(output))
        {
            return string.Empty;
        }

        foreach (var line in output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            if (line.Contains("mCurrentFocus", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("mFocusedApp", StringComparison.OrdinalIgnoreCase))
            {
                return line.Trim();
            }
        }

        return string.Empty;
    }

    protected bool WaitForActivityContains(string expectActivity, int timeoutSeconds = 30, int postWaitMilliseconds = 0)
    {
        Logger.Info($"等待 Activity 包含 '{expectActivity}'，超时 {timeoutSeconds}s");
        for (var i = 0; i < timeoutSeconds; i++)
        {
            var current = GetCurrentActivityRaw();
            if (current.Contains(expectActivity, StringComparison.OrdinalIgnoreCase))
            {
                if (postWaitMilliseconds > 0)
                {
                    Thread.Sleep(postWaitMilliseconds);
                }

                Logger.Info($"检测到目标 Activity: {current}");
                return true;
            }

            Thread.Sleep(1000);
        }

        Logger.Error($"超时：未检测到 Activity {expectActivity}");
        return false;
    }

    protected void PressKey(int keyCode, int delayMs = 300)
    {
        RunAdbShell($"input keyevent {keyCode}");
        if (delayMs > 0)
        {
            Thread.Sleep(delayMs);
        }
    }

    private static string ToXPathLiteral(string value)
    {
        if (!value.Contains('\''))
        {
            return $"'{value}'";
        }

        if (!value.Contains('\"'))
        {
            return $"\"{value}\"";
        }

        var parts = value.Split('\'');
        var quoted = parts.Select(p => $"'{p}'");
        return $"concat({string.Join(", \"'\", ", quoted)})";
    }
}
