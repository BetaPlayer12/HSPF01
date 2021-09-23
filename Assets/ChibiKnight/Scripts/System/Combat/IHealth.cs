using System;

namespace ChibiKnight.Systems.Combat
{
    public interface IHealth
    {
        int maxHealth { get; }
        int currentHealth { get; }
        event Action<int> HealthChange;
    }
}