using DesktopPet.Models;
using DesktopPet.Services;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DesktopPet.Controllers;

public sealed class PetController
{
    private readonly Image _petImage;
    private readonly ScaleTransform _flip;
    private readonly ScaleTransform _actionScale;
    private readonly TranslateTransform _actionTranslate;
    private readonly Action<double, double> _setPosition;
    private readonly Func<(double left, double top, double width, double height)> _getWindow;

    private readonly SettingsService _settingsService = new();
    private readonly AnimationService _animationService = new();
    private readonly MovementService _movementService = new();
    private readonly PetStateMachine _stateMachine = new();
    private readonly DispatcherTimer _timer;

    private PetSettings _settings;
    private bool _busyAction;

    public bool MovementPaused { get; private set; }

    public PetController(Image petImage, ScaleTransform flip, ScaleTransform actionScale, TranslateTransform actionTranslate,
        Action<double, double> setPosition, Func<(double left, double top, double width, double height)> getWindow)
    {
        _petImage = petImage;
        _flip = flip;
        _actionScale = actionScale;
        _actionTranslate = actionTranslate;
        _setPosition = setPosition;
        _getWindow = getWindow;

        _settings = _settingsService.Load();
        MovementPaused = _settings.MovementPaused;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        _timer.Tick += (_, _) => OnTick();
    }

    public void Initialize()
    {
        LoadOrCreateDefaultPetImage();

        var window = _getWindow();
        var top = _settings.Top > 0 ? _settings.Top : _movementService.BottomAlignedTop(window.height);
        _setPosition(_settings.Left, top);

        _timer.Start();
    }

    public void SavePosition(double left, double top)
    {
        _settings.Left = left;
        _settings.Top = top;
        _settings.MovementPaused = MovementPaused;
        _settingsService.Save(_settings);
    }

    public void ToggleMovement() => MovementPaused = !MovementPaused;

    public void ResetToBottom()
    {
        var w = _getWindow();
        _setPosition(w.left, _movementService.BottomAlignedTop(w.height));
    }

    public void ImportPetAsset()
    {
        var dialog = new OpenFileDialog { Title = "导入宠物素材", Filter = "图片文件|*.png;*.jpg;*.jpeg;*.bmp;*.gif" };
        if (dialog.ShowDialog() != true) return;

        var outputDir = Path.Combine(AppContext.BaseDirectory, "Assets", "Pets", "DefaultPet");
        Directory.CreateDirectory(outputDir);
        var ext = Path.GetExtension(dialog.FileName);
        var targetPath = Path.Combine(outputDir, $"pet{ext}");
        File.Copy(dialog.FileName, targetPath, true);

        _settings.PetImagePath = targetPath;
        _settingsService.Save(_settings);
        SetImage(targetPath);
    }

    private async void OnTick()
    {
        var state = _stateMachine.Tick();
        if (_busyAction) return;

        if (!MovementPaused)
        {
            var window = _getWindow();
            if (state == PetState.WalkLeft || state == PetState.WalkRight)
            {
                var next = _movementService.NextX(window.left, window.width, state == PetState.WalkLeft);
                _setPosition(next.x, _movementService.BottomAlignedTop(window.height));

                if (next.hitLeft)
                {
                    _stateMachine.Force(PetState.WalkRight);
                    _animationService.SetFacing(_flip, false);
                }
                else if (next.hitRight)
                {
                    _stateMachine.Force(PetState.WalkLeft);
                    _animationService.SetFacing(_flip, true);
                }
                else
                {
                    _animationService.SetFacing(_flip, state == PetState.WalkLeft);
                }
            }
        }

        if (state == PetState.SimpleAction)
        {
            _busyAction = true;
            if (DateTime.Now.Second % 2 == 0)
                await _animationService.PlayBlinkAsync(_actionScale);
            else
                await _animationService.PlayStretchAsync(_actionScale, _actionTranslate);
            _busyAction = false;
            _stateMachine.Force(PetState.Idle);
        }
    }

    private void LoadOrCreateDefaultPetImage()
    {
        if (!string.IsNullOrWhiteSpace(_settings.PetImagePath) && File.Exists(_settings.PetImagePath))
        {
            SetImage(_settings.PetImagePath);
            return;
        }

        var defaultPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Pets", "DefaultPet", "pet.png");
        if (File.Exists(defaultPath))
        {
            _settings.PetImagePath = defaultPath;
            SetImage(defaultPath);
            return;
        }
    }

    private void SetImage(string path)
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.UriSource = new Uri(path);
        bitmap.EndInit();
        bitmap.Freeze();
        _petImage.Source = bitmap;
    }
}
