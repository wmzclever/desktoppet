using DesktopPet.Controllers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DesktopPet;

public partial class MainWindow : Window
{
    private readonly PetController _controller;
    private bool _isDragging;

    public MainWindow()
    {
        InitializeComponent();

        _controller = new PetController(
            PetImage,
            FlipTransform,
            ActionScale,
            ActionTranslate,
            (l, t) => { Left = l; Top = t; },
            () => (Left, Top, Width, Height));

        BuildContextMenu();

        Loaded += (_, _) => _controller.Initialize();
        MouseLeftButtonDown += OnLeftDown;
        MouseLeftButtonUp += OnLeftUp;
        MouseMove += OnMouseMove;
    }

    private void BuildContextMenu()
    {
        var menu = new ContextMenu();

        menu.Items.Add(NewItem("导入宠物图片/动画素材", (_, _) => _controller.ImportPetAsset()));
        menu.Items.Add(NewItem("开始/暂停移动", (_, _) => _controller.ToggleMovement()));
        menu.Items.Add(NewItem("打开设置", (_, _) => MessageBox.Show("当前版本设置项较少：\n1) 右键可导入素材\n2) 可暂停移动\n后续可扩展独立设置窗口。", "设置")));
        menu.Items.Add(NewItem("重置位置", (_, _) => _controller.ResetToBottom()));
        menu.Items.Add(new Separator());
        menu.Items.Add(NewItem("退出", (_, _) => Close()));

        ContextMenu = menu;
    }

    private static MenuItem NewItem(string header, RoutedEventHandler click)
    {
        var item = new MenuItem { Header = header };
        item.Click += click;
        return item;
    }

    private void OnLeftDown(object sender, MouseButtonEventArgs e)
    {
        _isDragging = true;
        CaptureMouse();
        DragMove();
    }

    private void OnLeftUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isDragging) return;
        _isDragging = false;
        ReleaseMouseCapture();
        _controller.SavePosition(Left, Top);
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_isDragging && e.LeftButton == MouseButtonState.Released)
        {
            _isDragging = false;
            ReleaseMouseCapture();
            _controller.SavePosition(Left, Top);
        }
    }

    protected override void OnClosed(System.EventArgs e)
    {
        _controller.SavePosition(Left, Top);
        base.OnClosed(e);
    }
}
