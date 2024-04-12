using static Gun;

public class CooldownTimer
{
    private float timeElapsed;
    private float cooldownTime;
    private bool isReady;

    public bool IsReady => isReady;

    public CooldownTimer()
    {
        cooldownTime = 0f;
        timeElapsed = 0f;
        isReady = true;
    }
    
    public void UpdateCooldown(float deltaTime)
    {
        if (!isReady)
        {
            timeElapsed += deltaTime;
            if (timeElapsed >= cooldownTime)
            {
                isReady = true;
            }
        }
    }

    public void StartCooldown(float cooldown)
    {
        if (isReady)
        {
            cooldownTime = cooldown;
            timeElapsed = 0f;
            isReady = false;
        }
    }
}