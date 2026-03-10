using UnityEngine;

public static class SharedActionCooldownUtility
{
    public static bool IsReady(float lastActionTime, float cooldownDuration)
    {
        float cooldown = Mathf.Max(0f, cooldownDuration);
        return Time.time >= lastActionTime + cooldown;
    }

    public static float GetRemaining(float lastActionTime, float cooldownDuration)
    {
        float cooldown = Mathf.Max(0f, cooldownDuration);
        float remaining = (lastActionTime + cooldown) - Time.time;
        return Mathf.Max(0f, remaining);
    }

    public static float ConsumeNow()
    {
        return Time.time;
    }
}
