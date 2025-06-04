using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public float slowDownFactor = 0.05f;
    public float slowDownLength = 3.5f;
    private float startTime = -1f;
    public bool slowActivated = false;

    public float cooldownDuration = 15f;
    private float cooldownTimer = 0f;

    public bool perkUnlocked = false;
    private bool hasWarmedUp = false;
    private bool isWarmingUp = false;
    public bool perkReady = false;

    public float readyTextDelay = 0.5f;
    public float readyUISwitchDelay = 14f;

    public TextMeshProUGUI cooldownText;

    private float readyTextTimer = 0f;
    private float readyUIHoldTimer = 0f;
    private bool tutorialPowerupTriggered = false;

    [Header("Audio")]
    public AudioSource slowMoAudioSource;
    public AudioClip slowMoSound;

    [Header("UI Elements")]
    public Image slowMoIcon; // base icon (not used in logic, but useful to assign)
    public Image cooldownOverlay;
    public GameObject lockOverlay;
    public bool showCooldownText = true;

    void Start()
    {
        if (cooldownText != null)
            cooldownText.text = "Locked";

        // Start with lock visible and cooldown overlay full
        if (lockOverlay != null)
            lockOverlay.SetActive(true);

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 1f;
    }

    void Update()
    {
        if (!perkUnlocked)
        {
            if (cooldownText != null)
                cooldownText.text = "Slow-Mo: Locked";

            if (lockOverlay != null)
                lockOverlay.SetActive(true);

            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = 1f;

            return;
        }

        if (!tutorialPowerupTriggered)
        {
            TutorialManager.Instance?.OnPowerupAcquired();
            tutorialPowerupTriggered = true;
        }

        // Initial warm-up phase (only once)
        if (!hasWarmedUp)
        {
            if (!isWarmingUp)
                StartWarmUp();

            if (cooldownText != null && showCooldownText)
                cooldownText.text = "Slow-Mo: Locked";

            if (lockOverlay != null)
                lockOverlay.SetActive(true);

            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = 1f;

            if (readyTextTimer > 0f)
            {
                readyTextTimer -= Time.unscaledDeltaTime;
            }
            else if (readyUIHoldTimer > 0f)
            {
                readyUIHoldTimer -= Time.unscaledDeltaTime;
            }
            else
            {
                isWarmingUp = false;
                hasWarmedUp = true;
                perkReady = true;

                if (cooldownText != null && showCooldownText)
                    cooldownText.text = "Ready";

                if (lockOverlay != null)
                    lockOverlay.SetActive(false);

                if (cooldownOverlay != null)
                    cooldownOverlay.fillAmount = 0f;
            }
        }

        // Handle cooldown countdown (after warm-up has completed)
        if (hasWarmedUp && cooldownTimer > 0f)
        {
            cooldownTimer -= Time.unscaledDeltaTime;

            if (cooldownText != null && showCooldownText)
                cooldownText.text = $"{Mathf.CeilToInt(cooldownTimer)}s";

            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = cooldownTimer / cooldownDuration;

            if (cooldownTimer <= 0f)
            {
                cooldownTimer = 0f;
                perkReady = true;

                if (cooldownText != null && showCooldownText)
                    cooldownText.text = "Ready";

                if (cooldownOverlay != null)
                    cooldownOverlay.fillAmount = 0f;

                if (lockOverlay != null)
                    lockOverlay.SetActive(false);
            }
        }

        // Smooth return to normal time
        if (startTime >= 0f)
        {
            float elapsed = Time.realtimeSinceStartup - startTime;
            if (elapsed >= 2f)
            {
                float t = Mathf.Min(1.0f, Time.timeScale + (1f / slowDownLength) * Time.unscaledDeltaTime);
                Time.timeScale = t;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;

                if (Time.timeScale >= 0.99f)
                {
                    Time.timeScale = 1f;
                    slowActivated = false;
                    startTime = -1f;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
            DoSlowMotion();
    }

    public void DoSlowMotion()
    {

        if (!perkUnlocked || !perkReady || isWarmingUp)
        {
            Debug.Log("Slow-mo not ready yet!");
            return;
        }

        slowActivated = true;
        perkReady = false;
        Time.timeScale = slowDownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        startTime = Time.realtimeSinceStartup;
        cooldownTimer = cooldownDuration;

        if (cooldownText != null && showCooldownText)
            cooldownText.text = $"{Mathf.CeilToInt(cooldownTimer)}s";

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 1f;

        if (lockOverlay != null)
            lockOverlay.SetActive(false);

        if (slowMoAudioSource != null && slowMoSound != null)
            slowMoAudioSource.PlayOneShot(slowMoSound);
    }

    public void StartWarmUp()
    {
        isWarmingUp = true;
        readyTextTimer = readyTextDelay;
        readyUIHoldTimer = readyUISwitchDelay;
    }
}
