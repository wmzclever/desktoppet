using DesktopPet.Models;
using System;

namespace DesktopPet.Controllers;

public sealed class PetStateMachine
{
    private readonly Random _random = new();
    private DateTime _nextStateChange = DateTime.UtcNow.AddSeconds(2);

    public PetState Current { get; private set; } = PetState.Idle;

    public PetState Tick()
    {
        if (DateTime.UtcNow < _nextStateChange) return Current;

        Current = Current switch
        {
            PetState.Idle => RollFromIdle(),
            PetState.WalkLeft or PetState.WalkRight => RollFromWalk(),
            PetState.SimpleAction => PetState.Idle,
            _ => PetState.Idle
        };

        _nextStateChange = DateTime.UtcNow.AddMilliseconds(Current switch
        {
            PetState.Idle => _random.Next(2200, 5000),
            PetState.WalkLeft or PetState.WalkRight => _random.Next(2400, 5200),
            PetState.SimpleAction => _random.Next(800, 1400),
            _ => 3000
        });

        return Current;
    }

    public void Force(PetState state)
    {
        Current = state;
        _nextStateChange = DateTime.UtcNow.AddMilliseconds(300);
    }

    private PetState RollFromIdle()
    {
        var r = _random.NextDouble();
        if (r < 0.2) return PetState.SimpleAction;
        return _random.Next(0, 2) == 0 ? PetState.WalkLeft : PetState.WalkRight;
    }

    private PetState RollFromWalk()
    {
        var r = _random.NextDouble();
        if (r < 0.22) return PetState.SimpleAction;
        if (r < 0.55) return PetState.Idle;
        return Current;
    }
}
