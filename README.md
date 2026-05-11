# DesktopPet (WPF, .NET 8)

一个真正“常驻桌面宠物”取向的 WPF 程序：启动后默认只显示宠物本体，不显示常驻控制面板。

## 当前实现能力

- 透明背景、无边框、置顶桌宠窗口
- 屏幕底部附近左右行走，触碰边缘自动转向
- 基础状态机：`Idle` / `WalkLeft` / `WalkRight` / `SimpleAction`
- 自然切换：待机后随机走动、走动后随机停下、偶发简单动作
- 左键拖拽宠物，松开后继续活动
- 右键菜单：导入素材、开始/暂停移动、打开设置、重置位置、退出
- 设置持久化（图片路径/位置/是否暂停）

## 技术栈与项目要求

- C# + WPF
- .NET 8
- `TargetFramework=net8.0-windows`
- `UseWPF=true`
- `EnableWindowsTargeting=true`

## 项目结构

```text
DesktopPet/
  Controllers/
    PetController.cs
    PetStateMachine.cs
  Models/
    PetSettings.cs
    PetState.cs
  Services/
    AnimationService.cs
    MovementService.cs
    SettingsService.cs
  MainWindow.xaml
  MainWindow.xaml.cs
  App.xaml
  App.xaml.cs
```

## 运行方式（Windows 本地）

1. 安装 .NET 8 SDK 与 Visual Studio 2022（.NET 桌面开发工作负载）
2. 打开 `DesktopPet.sln`
3. 启动项目（F5）

## 素材目录

默认查找：

```text
DesktopPet/bin/Debug/net8.0-windows/Assets/Pets/DefaultPet/pet.png
```

也可以运行后右键宠物 -> 导入宠物图片/动画素材。

## 交互方式

- 左键拖拽：移动宠物
- 右键菜单：功能操作
- 无常驻调试文字/大黑框面板

## Linux / 云端说明

云端 Linux 容器仅用于代码生成或静态检查；WPF GUI 实际运行请在 Windows + Visual Studio 本地完成。
