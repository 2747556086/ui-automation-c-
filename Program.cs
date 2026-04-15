using CSharpAppiumPOM.Core;
using CSharpAppiumPOM.Pages;

var logger = new SimpleLogger("GTV_setting");
var start = DateTime.UtcNow;

AndroidDriverFactory? driverFactory = null;

try
{
    driverFactory = new AndroidDriverFactory(logger);
    using var driver = driverFactory.CreateDriver();

    var settingsPage = new SettingsPage(driver, logger);
    var systemPage = new SystemPage(driver, logger);

    // Step 1: 打开设置
    if (!settingsPage.OpenSystemSettings())
    {
        logger.Error("步骤1失败：无法打开系统设置界面");
        return;
    }

    settingsPage.PressEnter();
    Thread.Sleep(1000);

    // Step 2: 进入 System
    if (!settingsPage.EnterSystemMenu())
    {
        logger.Error("步骤2失败：未能进入 'System' 菜单");
        return;
    }

    // Step 3: 进入 Power & Energy 并设置 Shut-Off Timer 为 Never
    if (!systemPage.EnterPowerEnergyAndSetNever())
    {
        logger.Error("步骤3失败：未能完成 'Power & Energy' 导航");
        return;
    }

    // Step 4: 设置时区
    if (!systemPage.SetTimeZone("Hong Kong"))
    {
        logger.Error("步骤4失败：未能设置时区为 'Hong Kong'");
        return;
    }

    // Step 5: 开启蓝牙 HCI 日志
    if (!systemPage.EnableBluetoothHciLogging())
    {
        logger.Error("步骤5失败：蓝牙 HCI 日志配置未成功");
        return;
    }

    // Step 6: 返回主页
    logger.Info("步骤6：返回主屏幕...");
    settingsPage.PressHome();

    var elapsed = DateTime.UtcNow - start;
    logger.Info($"全流程执行成功！总耗时：{elapsed.TotalSeconds:F2} 秒");
}
catch (Exception ex)
{
    logger.Error($"执行失败：{ex.Message}");
}
finally
{
    logger.Info("执行结束。");
}
