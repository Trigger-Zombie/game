using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public float slowDownFactor = 0.05f;
    public float slowDownLength = 3.5f;
    private float startTime = -1f;
    public bool slowActivated = false;

    public float cooldownDuration = 10f;
    private float cooldownTimer = 0f;

    public bool perkUnlocked = false; // <- this is the only new variable you need

    public AudioSource slowMoSound;
    public TextMeshProUGUI cooldownText;

    void Start()
    {
        if (cooldownText != null)
        {
            cooldownText.text = "Slow-mo: Locked"; // <- Start locked
        }
    }

    void Update()
    {
        if (!perkUnlocked)
        {
            if (cooldownText != null)
                cooldownText.text = "Slow-mo: Locked";
            return;
        }

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.unscaledDeltaTime;
            if (cooldownText != null)
            {
                cooldownText.text = $"Slow-mo: {Mathf.CeilToInt(cooldownTimer)}s";
            }
        }
        else if (cooldownText != null && !slowActivated)
        {
            cooldownText.text = "Slow-mo: Ready";
        }

        if (startTime >= 0f)
        {
            float elapsedTime = Time.realtimeSinceStartup - startTime;

            if (elapsedTime >= 2f)
            {
                float targetValue = Mathf.Min(1.0f, Time.timeScale + (1f / slowDownLength) * Time.unscaledDeltaTime);
                Time.timeScale = targetValue;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;

                if (Time.timeScale >= 0.99f)
                {
                    Time.timeScale = 1f;
                    slowActivated = false;
                    startTime = -1f;
                }
            }
        }

        // Listen for activation
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DoSlowMotion();
        }
    }

    public void DoSlowMotion()
    {
        if (!perkUnlocked) return;

        if (cooldownTimer > 0f)
        {
            Debug.Log("Slow-mo is on cooldown!");
            return;
        }

        if (slowMoSound != null)
        {
            slowMoSound.Play();
        }

        slowActivated = true;
        Time.timeScale = slowDownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        startTime = Time.realtimeSinceStartup;
        cooldownTimer = cooldownDuration;

        if (cooldownText != null)
        {
            cooldownText.text = $"Slow-mo: {Mathf.CeilToInt(cooldownTimer)}s";
        }
    }
}
