using CSharpAppiumPOM.Core;
using OpenQA.Selenium.Appium.Android;

namespace CSharpAppiumPOM.Pages;

public sealed class SettingsPage : BasePage
{
    public SettingsPage(AndroidDriver driver, SimpleLogger logger) : base(driver, logger)
    {
    }

    public bool OpenSystemSettings(int timeoutSeconds = 5)
    {
        Logger.Info("尝试打开系统设置界面 (KEYCODE_SETTINGS)");
        PressKey(176, 300);

        for (var i = 0; i < timeoutSeconds; i++)
        {
            var current = GetCurrentActivityRaw();
            if (current.Contains("DashboardHandler", StringComparison.OrdinalIgnoreCase))
            {
                Logger.Info("成功进入系统设置主界面 (DashboardHandler)");
                Thread.Sleep(1000);
                return true;
            }

            if (i == 1)
            {
                Logger.Warn("首次尝试未检测到设置界面，重试一次");
                PressKey(176, 300);
            }

            Thread.Sleep(1000);
        }

        Logger.Error("超时：未能进入系统设置界面 (DashboardHandler)");
        return false;
    }

    public bool EnterSystemMenu()
    {
        if (!GetCurrentActivityRaw().Contains("MainSettings", StringComparison.OrdinalIgnoreCase))
        {
            Logger.Info("当前不在 MainSettings，尝试重新打开设置");
            if (!OpenSystemSettings())
            {
                return false;
            }

            PressEnter();
            Thread.Sleep(1000);
        }

        if (!WaitForActivityContains("MainSettings", 15))
        {
            return false;
        }

        Logger.Info("查找 'System' 选项");
        for (var i = 0; i < 6; i++)
        {
            if (ExistsByText("System"))
            {
                ClickByText("System");
                Logger.Info("成功点击 'System'，进入子菜单");
                Thread.Sleep(2000);
                return true;
            }

            if (i < 5)
            {
                PressDown(1, 500);
            }

            Thread.Sleep(300);
        }

        Logger.Error("滚动6次后仍未找到 'System'");
        return false;
    }

    public void PressEnter(int pressTimes = 1, int delayMs = 1000)
    {
        for (var i = 0; i < pressTimes; i++)
        {
            PressKey(66, delayMs);
        }
    }

    public void PressDown(int times = 1, int delayMs = 100)
    {
        for (var i = 0; i < times; i++)
        {
            PressKey(20, delayMs);
        }
    }

    public void PressBack(int times = 1, int delayMs = 500)
    {
        for (var i = 0; i < times; i++)
        {
            PressKey(4, 0);
            Thread.Sleep(delayMs);
        }
    }

    public void PressHome()
    {
        Logger.Info("返回主屏幕 (KEYCODE_HOME)");
        PressKey(3, 1000);
    }
}
