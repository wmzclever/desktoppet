# DesktopPet (WPF, .NET 8 实现)

> 本项目基于 **.NET 8** 实现，目标框架为 `net8.0-windows`。

一个可以在 **Windows 本地直接开发和运行** 的 WPF 桌宠项目（类似 QQ 宠物风格）。
当前版本已经是“可用骨架”：支持导入宠物照片、简单自动透明化、保存 PNG、播放浮动动画。

> 云端 Linux 容器仅用于代码生成/静态检查；WPF GUI 的运行与完整调试请在 Windows + Visual Studio 完成。

## 功能现状（可用版本）

- 上传宠物照片（PNG/JPG/BMP/GIF）
- 自动透明化（根据左上角背景色 + 阈值做抠图）
- 保存透明 PNG 到项目输出目录
- 浮动动画（上下轻微漂浮）
- 无边框、透明背景、置顶窗口，可拖动

## 项目配置要求（已完成）

`DesktopPet/DesktopPet.csproj` 已包含：

- `TargetFramework = net8.0-windows`
- `UseWPF = true`
- `EnableWindowsTargeting = true`

## Windows 本地开发步骤

### 1) 安装 .NET 8 SDK

1. 打开：<https://dotnet.microsoft.com/download/dotnet/8.0>
2. 安装 **.NET SDK 8.x (Windows x64)**
3. PowerShell 验证：

```powershell
dotnet --info
```

### 2) 安装 Visual Studio 2022

- 建议 VS 2022 17.8+
- 勾选工作负载：**.NET 桌面开发**

### 3) 打开并运行

```powershell
cd <repo-path>
dotnet restore DesktopPet.sln
dotnet build DesktopPet.sln -c Debug
dotnet run --project .\DesktopPet\DesktopPet.csproj
```

或直接双击 `DesktopPet.sln` 用 VS 打开后按 `F5`。

## 素材目录结构

```text
DesktopPet/
  Assets/
    Pets/
      SamplePet/
        Original/      # 原始宠物照片
        Generated/     # 透明图、切帧图、动画序列
    Effects/           # 特效资源（阴影、粒子、光晕）
```

运行时保存透明 PNG 的默认路径（示例）：

```text
DesktopPet/bin/Debug/net8.0-windows/Assets/Pets/MyPet/Generated/
```

## Linux/Codex 环境说明（必须阅读）

在 Linux 容器里通常会遇到以下情况（均正常）：

1. `dotnet restore` 受网络/代理限制失败（无法访问 nuget）
2. `dotnet build` 缺少 WindowsDesktop SDK 目标

这不代表项目不可用。**实际运行请在 Windows 本地完成**。

## 下一步建议（我可以继续帮你补）

- 更好的抠图：边缘羽化、抗锯齿、前景识别
- 多动作动画：idle / walk / click
- 桌宠交互：点击摸头、随机语音气泡
- 托盘菜单：切换宠物、开机启动、透明度/大小调节
- 接入 AI：根据宠物照片自动生成多姿态 PNG 序列
