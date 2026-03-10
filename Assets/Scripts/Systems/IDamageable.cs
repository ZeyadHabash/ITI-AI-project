

public interface IDamageable
{
    DamagableType Type { get; }
    void TakeDamage(int damage);
}

public enum DamagableType
{
    Player,
    Enemy
}