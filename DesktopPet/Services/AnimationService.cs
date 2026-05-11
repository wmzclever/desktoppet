using System;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace DesktopPet.Services;

public sealed class AnimationService
{
    public void SetFacing(ScaleTransform flip, bool faceLeft) => flip.ScaleX = faceLeft ? -1 : 1;

    public async Task PlayBlinkAsync(ScaleTransform actionScale)
    {
        await AnimateScaleY(actionScale, 1, 0.85, 120);
        await AnimateScaleY(actionScale, 0.85, 1, 120);
    }

    public async Task PlayStretchAsync(ScaleTransform actionScale, TranslateTransform actionTranslate)
    {
        var t1 = AnimateScale(actionScale, 1, 1.08, 220);
        var t2 = AnimateTranslateY(actionTranslate, 0, -8, 220);
        await Task.WhenAll(t1, t2);
        await Task.WhenAll(AnimateScale(actionScale, 1.08, 1, 220), AnimateTranslateY(actionTranslate, -8, 0, 220));
    }

    private static Task AnimateScale(ScaleTransform t, double from, double to, int ms)
    {
        var tx = AnimateDouble(t, ScaleTransform.ScaleXProperty, from, to, ms);
        var ty = AnimateDouble(t, ScaleTransform.ScaleYProperty, from, to, ms);
        return Task.WhenAll(tx, ty);
    }

    private static Task AnimateScaleY(ScaleTransform t, double from, double to, int ms)
        => AnimateDouble(t, ScaleTransform.ScaleYProperty, from, to, ms);

    private static Task AnimateTranslateY(TranslateTransform t, double from, double to, int ms)
        => AnimateDouble(t, TranslateTransform.YProperty, from, to, ms);

    private static Task AnimateDouble(DependencyObject target, DependencyProperty property, double from, double to, int ms)
    {
        var tcs = new TaskCompletionSource<bool>();
        var animation = new DoubleAnimation(from, to, TimeSpan.FromMilliseconds(ms)) { EasingFunction = new SineEase() };
        animation.Completed += (_, _) => tcs.TrySetResult(true);
        target.BeginAnimation(property, animation);
        return tcs.Task;
    }
}
