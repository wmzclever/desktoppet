namespace DesktopPet.Models;

public sealed class PetSettings
{
    public string? PetImagePath { get; set; }
    public double Left { get; set; } = 200;
    public double Top { get; set; } = 700;
    public bool MovementPaused { get; set; }
}
