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

    public bool perkUnlocked = false;
    private bool perkReady = false;

    public AudioSource slowMoSound;
    public AudioClip readySound;

    public float readyTextDelay = 0.5f;
    public float readyUISwitchDelay = 14f;

    public TextMeshProUGUI cooldownText;

    private bool wasReadyLastFrame = false;
    private float readyTextTimer = 0f;
    private float readyUIHoldTimer = 0f;

    // 🎥 Camera shake
    public Transform cameraTransform;
    public float shakeIntensity = 0.1f;
    public float shakeFrequency = 20f;
    private Vector3 originalCamPos;

    void Start()
    {
        if (cooldownText != null)
        {
            cooldownText.text = "Slow-mo: Locked";
        }

        // Assign camera if not set
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        originalCamPos = cameraTransform.localPosition;
    }

    void Update()
    {
        if (!perkUnlocked)
        {
            if (cooldownText != null)
                cooldownText.text = "Slow-mo: Locked";

            wasReadyLastFrame = false;
            perkReady = false;
            return;
        }

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.unscaledDeltaTime;
            if (cooldownText != null)
            {
                cooldownText.text = $"Slow-mo: {Mathf.CeilToInt(cooldownTimer)}s";
            }
            wasReadyLastFrame = false;
        }
        else if (!slowActivated)
        {
            // 🔊 Play ready sound once
            if (!wasReadyLastFrame)
            {
                if (readySound != null && slowMoSound != null)
                {
                    slowMoSound.PlayOneShot(readySound);
                    Debug.Log("🔊 Played slow-mo ready sound.");
                }

                readyTextTimer = readyTextDelay;
                readyUIHoldTimer = readyUISwitchDelay;
            }

            // ⏱ Wait before showing "Ready"
            if (readyTextTimer > 0f)
            {
                readyTextTimer -= Time.unscaledDeltaTime;
            }
            else if (readyUIHoldTimer > 0f)
            {
                readyUIHoldTimer -= Time.unscaledDeltaTime;

                // 🎥 Shake camera
                if (cameraTransform != null)
                {
                    float shakeAmount = Mathf.Sin(Time.time * shakeFrequency) * shakeIntensity;
                    Vector3 shakeOffset = new Vector3(shakeAmount, shakeAmount, 0f);
                    cameraTransform.localPosition = originalCamPos + shakeOffset;
                }
            }
            else
            {
                // ✅ Warmup complete
                if (cooldownText != null)
                {
                    cooldownText.text = "Slow-mo: Ready";
                }

                if (cameraTransform != null)
                {
                    cameraTransform.localPosition = originalCamPos;
                }

                perkReady = true;
            }

            wasReadyLastFrame = true;
        }

        // Restore time scale if slow-mo is active
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

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DoSlowMotion();
        }
    }

    public void DoSlowMotion()
    {
        if (!perkUnlocked || !perkReady) return;

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
