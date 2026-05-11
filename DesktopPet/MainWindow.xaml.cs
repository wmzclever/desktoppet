using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace DesktopPet;

public partial class MainWindow : Window
{
    private BitmapSource? _originalImage;
    private BitmapSource? _processedImage;
    private bool _isAnimating;
    private Storyboard? _floatStoryboard;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Upload_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "选择宠物照片",
            Filter = "图片文件|*.png;*.jpg;*.jpeg;*.bmp;*.gif"
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.UriSource = new Uri(dialog.FileName);
        bitmap.EndInit();
        bitmap.Freeze();

        _originalImage = bitmap;
        _processedImage = null;
        PetImage.Source = _originalImage;
        PlaceholderText.Visibility = Visibility.Collapsed;
        StatusText.Text = $"已加载：{Path.GetFileName(dialog.FileName)}";
    }

    private void RemoveBackground_Click(object sender, RoutedEventArgs e)
    {
        if (_originalImage is null)
        {
            StatusText.Text = "请先上传宠物照片。";
            return;
        }

        var tolerance = (byte)ToleranceSlider.Value;
        _processedImage = MakeCornerColorTransparent(_originalImage, tolerance);
        PetImage.Source = _processedImage;
        StatusText.Text = $"透明化完成（阈值：{tolerance}）。建议检查边缘并调整阈值。";
    }

    private void SaveProcessed_Click(object sender, RoutedEventArgs e)
    {
        if (_processedImage is null)
        {
            StatusText.Text = "没有可保存的处理结果，请先执行自动透明化。";
            return;
        }

        var outputDir = Path.Combine(AppContext.BaseDirectory, "Assets", "Pets", "MyPet", "Generated");
        Directory.CreateDirectory(outputDir);

        var fileName = $"pet_transparent_{DateTime.Now:yyyyMMdd_HHmmss}.png";
        var outputPath = Path.Combine(outputDir, fileName);

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(_processedImage));
        using var fs = File.Create(outputPath);
        encoder.Save(fs);

        StatusText.Text = $"已保存：{outputPath}";
    }

    private void ToggleAnimation_Click(object sender, RoutedEventArgs e)
    {
        if (_isAnimating)
        {
            _floatStoryboard?.Stop();
            PetTranslate.Y = 0;
            _isAnimating = false;
            StatusText.Text = "浮动动画已停止。";
            return;
        }

        var animation = new DoubleAnimation
        {
            From = 0,
            To = -8,
            Duration = TimeSpan.FromMilliseconds(900),
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever,
            EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
        };

        _floatStoryboard = new Storyboard();
        Storyboard.SetTarget(animation, PetTranslate);
        Storyboard.SetTargetProperty(animation, new PropertyPath("Y"));
        _floatStoryboard.Children.Add(animation);
        _floatStoryboard.Begin();

        _isAnimating = true;
        StatusText.Text = "浮动动画播放中。";
    }

    private static BitmapSource MakeCornerColorTransparent(BitmapSource source, byte tolerance)
    {
        var formatted = new FormatConvertedBitmap(source, System.Windows.Media.PixelFormats.Bgra32, null, 0);
        var width = formatted.PixelWidth;
        var height = formatted.PixelHeight;
        var stride = width * 4;
        var pixels = new byte[height * stride];
        formatted.CopyPixels(pixels, stride, 0);

        var b0 = pixels[0];
        var g0 = pixels[1];
        var r0 = pixels[2];

        for (var i = 0; i < pixels.Length; i += 4)
        {
            var db = Math.Abs(pixels[i] - b0);
            var dg = Math.Abs(pixels[i + 1] - g0);
            var dr = Math.Abs(pixels[i + 2] - r0);

            if (db <= tolerance && dg <= tolerance && dr <= tolerance)
            {
                pixels[i + 3] = 0;
            }
        }

        var result = BitmapSource.Create(width, height, formatted.DpiX, formatted.DpiY,
            System.Windows.Media.PixelFormats.Bgra32, null, pixels, stride);
        result.Freeze();
        return result;
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
