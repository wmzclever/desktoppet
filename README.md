# DesktopPet (WPF, .NET 8)

一个可在 **Windows 本地开发与运行** 的 WPF 桌宠项目骨架，定位为类似 QQ 宠物风格的桌面宠物。

> 说明：当前仓库重点完成可编译项目结构与跨环境检查配置。云端 Linux 容器仅用于代码生成/静态检查；GUI 实际运行请在 Windows + Visual Studio 中进行。

## 1. 技术栈

- C# 12
- WPF
- .NET 8 (`net8.0-windows`)

## 2. 云端与本地的关系

- 云端（如 Codex Linux 容器）：主要做
  - 代码生成
  - `dotnet restore`
  - `dotnet build`（尽量）
- 本地 Windows：负责
  - GUI 实际运行
  - 调试窗口透明、置顶、拖拽、动画效果

> 如在 Linux 上构建 WPF 遇到平台限制，这是预期行为；请以 Windows 本地构建结果为准。

## 3. 安装 .NET 8 SDK（Windows）

1. 打开官方页面：
   - https://dotnet.microsoft.com/download/dotnet/8.0
2. 下载并安装 **.NET SDK 8.x (Windows x64)**。
3. 安装完成后在 PowerShell 验证：

```powershell
dotnet --info
```

## 4. 如何在 Windows 本地打开项目

### 方案 A：Visual Studio 2022（推荐）

1. 安装 Visual Studio 2022（17.8+）。
2. 勾选工作负载：
   - **.NET 桌面开发**
3. 打开 `DesktopPet.sln`。
4. 选择 `DesktopPet` 为启动项目。
5. 按 `F5` 运行。

### 方案 B：命令行

```powershell
cd <repo-path>
dotnet restore DesktopPet.sln
dotnet build DesktopPet.sln -c Debug
dotnet run --project .\DesktopPet\DesktopPet.csproj
```

## 5. 素材目录结构（支持自定义宠物照片）

```text
DesktopPet/
  Assets/
    Pets/
      SamplePet/
        Original/      # 你上传的原始宠物照片
        Generated/     # 处理后的透明图、切帧图、动画序列
    Effects/           # 特效资源（光晕、阴影、粒子贴图等）
```

建议流程：
1. 将宠物照片放入 `Original/`。
2. 使用你常用的抠图/背景移除工具生成透明 PNG，放入 `Generated/`。
3. 后续可加入逐帧动作图（如 `idle_0001.png` ...）用于动画。

## 6. 项目结构

```text
DesktopPet.sln
DesktopPet/
  DesktopPet.csproj
  App.xaml
  App.xaml.cs
  MainWindow.xaml
  MainWindow.xaml.cs
  Assets/
```

## 7. 关键项目配置

`DesktopPet.csproj` 已包含：

- `<TargetFramework>net8.0-windows</TargetFramework>`
- `<UseWPF>true</UseWPF>`
- `<EnableWindowsTargeting>true</EnableWindowsTargeting>`

这使得在非 Windows 环境也可尽量执行 restore/build 检查。

## 8. 下一步可扩展方向

- 拖拽移动（鼠标按下拖动窗口）
- 透明穿透与点击策略
- Idle/Walk/Interact 动画状态机
- 自动抠图与风格化（接入图像处理/生成模型）
- 托盘图标、右键菜单、开机自启

---

如果你希望，我下一步可以继续补上：
- 宠物图片自动加载与回退逻辑
- 简单逐帧动画播放器
- Windows 本地可用的一键导入照片脚本
