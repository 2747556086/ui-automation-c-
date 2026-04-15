using CSharpAppiumPOM.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace CSharpAppiumPOM.Pages;

public sealed class SystemPage : BasePage
{
    public SystemPage(AndroidDriver driver, SimpleLogger logger) : base(driver, logger)
    {
    }

    public bool EnterPowerEnergyAndSetNever()
    {
        Logger.Info("尝试进入 'Power & Energy' 菜单");

        if (!ExistsByTextMatches("(?i)Power\\s*[&]?\\s*Energy"))
        {
            Logger.Error("未找到 'Power & Energy' 元素");
            return false;
        }

        var powerElements = Driver.FindElements(By.XPath("//*[contains(@text,'Power') and contains(@text,'Energy')]"));
        if (powerElements.Count == 0)
        {
            Logger.Error("未找到 'Power & Energy' 元素");
            return false;
        }

        powerElements[0].Click();
        Thread.Sleep(2000);

        if (!ExistsByText("Shut-Off Timer"))
        {
            Logger.Warn("'Shut-Off Timer' 不存在，尝试返回 System");
            return BackToSystemMenu();
        }

        ClickByText("Shut-Off Timer");
        Thread.Sleep(1000);

        PressKey(66, 1000); // 进入下级列表

        for (var i = 0; i < 5; i++)
        {
            PressKey(20, 500);

            if (!ExistsByText("Never"))
            {
                for (var j = 0; j < 10; j++)
                {
                    PressKey(20, 100);
                }

                PressKey(66, 1000);
                PressKey(66, 1000);
                Logger.Info("已将 'Shut-Off Timer' 设置为 'Never'");
                break;
            }
        }

        return BackToSystemMenu();
    }

    public bool SetTimeZone(string zone)
    {
        Thread.Sleep(2000);

        if (!ClickByText("Date & Time"))
        {
            Logger.Error("'Date & Time' 不存在，无法设置时区");
            return false;
        }

        Logger.Info("已进入 'Date & Time' 设置");

        if (!ClickByText("Set time zone"))
        {
            Logger.Error("'Set time zone' 选项缺失");
            PressKey(4, 500);
            return false;
        }

        Logger.Info("已点击 'Set time zone'，开始搜索目标时区");

        for (var i = 0; i < 100; i++)
        {
            if (ExistsByText(zone))
            {
                ClickByText(zone);
                Logger.Info($"时区已成功设置为: {zone}");
                Thread.Sleep(1000);
                PressKey(4, 1);
                Logger.Info("已返回 'System' 菜单");
                return true;
            }

            PressKey(20, 1);
        }

        Logger.Error($"滚动100次后仍未找到时区: {zone}");
        PressKey(4, 500);
        return false;
    }

    public bool EnableBluetoothHciLogging()
    {
        Logger.Info("通过 ADB + su 配置蓝牙 HCI 日志");

        // Appium 的 mobile:shell 一次执行一条命令，这里按原脚本顺序逐条执行。
        var commands = new[]
        {
            "su 0 setprop persist.bluetooth.btsnoopenable true",
            "su 0 setprop persist.bluetooth.btsnooplogmode full",
            "su 0 setprop persist.bluetooth.btsnoopsize 0x7ffffff",
            "su 0 setprop persist.bluetooth.btsnooppath /data/misc/bluetooth/logs/btsnoop_hci.log",
            "su 0 setprop persist.log.tag.bluetooth VERBOSE",
            "su 0 svc bluetooth disable",
            "sleep 2",
            "su 0 svc bluetooth enable"
        };

        foreach (var cmd in commands)
        {
            var result = RunAdbShell(cmd, 10);
            Logger.Debug($"执行: {cmd} -> {result}");
        }

        Logger.Info("蓝牙 HCI 日志配置命令已执行完成");
        return true;
    }

    private bool BackToSystemMenu()
    {
        Logger.Info("返回到 'System' 菜单（检测 'Date & Time'）");
        var started = DateTime.UtcNow;
        while (DateTime.UtcNow - started < TimeSpan.FromSeconds(20))
        {
            if (ExistsByText("Date & Time"))
            {
                Logger.Info("已回到 'System' 菜单");
                return true;
            }

            PressKey(4, 500);
            Thread.Sleep(1000);
        }

        Logger.Error("超时：未能返回到 'System' 菜单");
        return false;
    }
}
