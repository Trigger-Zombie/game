using UnityEngine;
using TMPro;

public class SpeedBoostManager : MonoBehaviour
{
    public static SpeedBoostManager Instance;

    public bool perkUnlocked = false;
    public bool perkActive = false;
    public bool perkReady = false;

    public float speedMultiplier = 2f;
    public float duration = 10f;
    public float cooldownDuration = 20f;

    private float activeTimer = 0f;
    private float cooldownTimer = 0f;

    public float readyTextDelay = 0.5f;
    public float readyUISwitchDelay = 14f;

    private bool hasWarmedUp = false;
    private bool isWarmingUp = false;
    private float readyTextTimer = 0f;
    private float readyUIHoldTimer = 0f;

    public TextMeshProUGUI perkText;

    private player_controller player;

    void Awake()
    {
        Instance = this;
        player = FindFirstObjectByType<player_controller>();
    }

    void Start()
    {
        if (perkText != null)
            perkText.text = "Speed Boost: Locked";
    }

    void Update()
    {
        if (!perkUnlocked)
        {
            if (perkText != null)
                perkText.text = "Speed Boost: Locked";
            return;
        }

        if (!hasWarmedUp)
        {
            if (!isWarmingUp)
                StartWarmUp();

            if (perkText != null)
                perkText.text = "Speed Boost: Locked";

            if (readyTextTimer > 0f)
            {
                readyTextTimer -= Time.deltaTime;
            }
            else if (readyUIHoldTimer > 0f)
            {
                readyUIHoldTimer -= Time.deltaTime;
            }
            else
            {
                isWarmingUp = false;
                hasWarmedUp = true;
                perkReady = true;

                if (perkText != null)
                    perkText.text = "Speed Boost: Ready";
            }

            return;
        }

        if (perkActive)
        {
            activeTimer -= Time.deltaTime;
            if (perkText != null)
                perkText.text = $"Speed Boost: {Mathf.CeilToInt(activeTimer)}s";

            if (activeTimer <= 0f)
            {
                perkActive = false;
                cooldownTimer = cooldownDuration;
                player.speed /= speedMultiplier;

                if (perkText != null)
                    perkText.text = $"Speed Boost: {Mathf.CeilToInt(cooldownTimer)}s";
            }

            return;
        }

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            if (perkText != null)
                perkText.text = $"Speed Boost: {Mathf.CeilToInt(cooldownTimer)}s";

            if (cooldownTimer <= 0f)
            {
                perkReady = true;
                if (perkText != null)
                    perkText.text = "Speed Boost: Ready";
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.Z) && perkReady)
        {
            ActivatePerk();
        }
    }

    public void ActivatePerk()
    {
        perkActive = true;
        perkReady = false;
        activeTimer = duration;

        player.speed *= speedMultiplier;

        if (perkText != null)
            perkText.text = $"Speed Boost: {duration}s";
    }

    public void StartWarmUp()
    {
        isWarmingUp = true;
        readyTextTimer = readyTextDelay;
        readyUIHoldTimer = readyUISwitchDelay;
    }
}