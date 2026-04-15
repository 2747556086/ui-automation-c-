# uiautomator_test

本仓库 Pytho包含一个由 **C# + Appium + POM（Page Object Model）** 编写的自动化项目，目标是保持原有业务步骤一致，同时让代码结构更清晰、可维护、可扩展。

---

## 1. 项目目标

原脚本核心诉求是：在 Android/TV 设备上自动完成系统设置相关操作。  
本 C# 版本保留了同样的执行顺序和业务动作：

1. 打开系统设置
2. 进入 `System`
3. 进入 `Power & Energy`，处理 `Shut-Off Timer`
4. 设置时区为 `Hong Kong`
5. 开启蓝牙 HCI 日志参数
6. 返回主屏幕

---

## 2. C# 工程位置

- `CSharpAppiumPOM`

你可以把它看作一个独立的控制台自动化工具工程。

---

## 3. 技术栈

- .NET 8（控制台应用）
- Appium (`Appium.WebDriver`)
- Android UiAutomator2（由 Appium 驱动）
- POM（页面对象模式）

---

## 4. 目录结构与职责

`CSharpAppiumPOM` 目录下主要文件如下：

- `Program.cs`  
  主流程入口，按步骤串联整个自动化流程（Step1 到 Step6）。

- `Core/AndroidDriverFactory.cs`  
  负责初始化 `AndroidDriver`，读取环境变量并创建 Appium 会话。

- `Core/SimpleLogger.cs`  
  轻量日志工具，统一输出 `INFO/DEBUG/WARN/ERROR`。

- `Pages/BasePage.cs`  
  页面基类，封装共通能力：元素查找、按键事件、Activity 轮询、`mobile: shell` 命令执行。

- `Pages/SettingsPage.cs`  
  对应设置主页面相关操作（打开设置、进入 System、方向键和返回等）。

- `Pages/SystemPage.cs`  
  对应 `System` 子菜单业务动作（Power & Energy、时区设置、蓝牙日志配置）。

---

## 5. POM 设计说明

本项目采用 POM 的原则：

- **流程编排在入口**：`Program.cs` 只关注“做什么步骤”，不关心页面细节。
- **页面行为归页面类**：每个 Page 类只负责本页面可执行动作。
- **通用动作归基类**：按键、等待、Shell 命令等重复逻辑统一放到 `BasePage`。

这样做的好处：

- 新增步骤时，不会让入口文件膨胀；
- 页面变化时，只修改对应 Page 类；
- 更容易写单元测试/集成测试（后续可引入测试框架）。

---

## 6. 运行前准备

请先确认以下环境：

1. 已安装 **.NET 8 SDK**
2. 已安装并启动 **Appium Server**（默认 `http://127.0.0.1:4723`）
3. Android 设备已连接，并可通过 `adb devices` 识别
4. 设备允许自动化（UiAutomator2）
5. 若执行蓝牙日志步骤，设备具备对应权限（部分命令可能需要 root）

---

## 7. 快速开始

在仓库根目录执行：

```bash
cd CSharpAppiumPOM
dotnet restore
dotnet build
dotnet run
```

---

## 8. 可选环境变量

`AndroidDriverFactory` 支持以下环境变量：

- `APPIUM_SERVER_URL`：Appium 服务地址（默认 `http://127.0.0.1:4723`）
- `ANDROID_DEVICE_NAME`：设备名称（默认 `AndroidDevice`）
- `ANDROID_UDID`：指定目标设备序列号（多设备场景建议配置）

Windows PowerShell 示例：

```powershell
$env:APPIUM_SERVER_URL="http://127.0.0.1:4723"
$env:ANDROID_DEVICE_NAME="MyTV"
$env:ANDROID_UDID="emulator-5554"
dotnet run --project .\CSharpAppiumPOM
```

---

## 9. 执行流程细节（对应 Program）

入口执行逻辑为：

- Step 1：调用 `SettingsPage.OpenSystemSettings()`
- Step 2：调用 `SettingsPage.EnterSystemMenu()`
- Step 3：调用 `SystemPage.EnterPowerEnergyAndSetNever()`
- Step 4：调用 `SystemPage.SetTimeZone("Hong Kong")`
- Step 5：调用 `SystemPage.EnableBluetoothHciLogging()`
- Step 6：调用 `SettingsPage.PressHome()`

任一步骤失败都会记录错误并提前退出，避免误操作后续步骤。

---

## 10. 常见问题排查

- **无法连接 Appium**
  - 检查 Appium 是否启动，地址是否与 `APPIUM_SERVER_URL` 一致。

- **找不到设备**
  - 先执行 `adb devices`，确认设备在线且授权成功。

- **元素定位失败**
  - 不同 ROM/机型文案可能有差异，可在对应 Page 类修改文本定位条件。

- **蓝牙日志命令执行不生效**
  - 设备权限或系统限制导致，需确认是否支持 `su` 与相关 `setprop`。

---

## 11. 后续扩展建议

- 增加配置文件（如 `appsettings.json`）统一管理时区、重试次数、超时参数；
- 引入测试框架（`xUnit`/`NUnit`）做可回归测试；
- 增加截图和失败现场采集；
- 按业务页面继续拆分 Page 类，保持单一职责。

---

## 12. 相关文档

- 子项目说明：`CSharpAppiumPOM/README.md`
