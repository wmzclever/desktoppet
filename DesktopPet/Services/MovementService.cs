using System;
using System.Windows;

namespace DesktopPet.Services;

public sealed class MovementService
{
    public double Speed { get; set; } = 1.8;

    public (double x, bool hitLeft, bool hitRight) NextX(double currentX, double width, bool movingLeft)
    {
        var virtualWidth = SystemParameters.VirtualScreenWidth;
        var minX = SystemParameters.VirtualScreenLeft;
        var maxX = minX + virtualWidth - width;

        var x = currentX + (movingLeft ? -Speed : Speed);
        if (x <= minX) return (minX, true, false);
        if (x >= maxX) return (maxX, false, true);
        return (x, false, false);
    }

    public double BottomAlignedTop(double windowHeight)
        => SystemParameters.WorkArea.Bottom - windowHeight - 12;
}
