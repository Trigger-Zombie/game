using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Tutorial Hints")]
    public GameObject movementHint;
    public GameObject reloadHint;
    public GameObject reloadNowHint;
    public GameObject slowMoHint;

    [Header("Settings")]
    public float hintDisplayTime = 5f;
    public float fadeTime = 0.5f;

    [Header("State Flags")]
    private bool movementHintActive = false;
    private bool hasShownReloadHint = false;
    private bool hasShownSlowMoHint = false;
    private bool isShowingReloadHint = false;
    private bool isPlayerMoving = false;
    private bool isOutOfAmmo = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    void Start()
    {
        HideAllHints();

        ShowHintPersistent(movementHint);
        movementHintActive = true;
    }

    void Update()
    {
        if (movementHintActive && DetectPlayerMovement())
        {
            HideHintImmediate(movementHint);
            movementHintActive = false;
        }
    }

    // Detect WASD input
    private bool DetectPlayerMovement()
    {
        return Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
    }

    public void OnPlayerMoved()
    {
        if (movementHintActive)
        {
            HideHintImmediate(movementHint);
            movementHintActive = false;
        }
    }

    // public void OnPlayerOutOfAmmo()
    // {
    //     if (!hasShownReloadHint)
    //     {
    //         StartCoroutine(ShowHintForTime(reloadHint, hintDisplayTime));
    //         hasShownReloadHint = true;
    //     }
    //     else
    //     {
    //         ShowHintPersistent(reloadNowHint);
    //     }
    // }
    public void OnPlayerOutOfAmmo()
    {

        if (isOutOfAmmo) return;
        isOutOfAmmo = true;

        if (!hasShownReloadHint && !isShowingReloadHint)
        {
            StartCoroutine(ShowReloadHintOnceThenAlwaysShowReloadText());
        }
        else
        {
            ShowHintPersistent(reloadNowHint);
        }
    }

    public void OnPlayerReloaded()
    {
        HideHintImmediate(reloadNowHint);
    }

    public void OnPowerupAcquired()
    {
        if (!hasShownSlowMoHint)
        {
            StartCoroutine(ShowHintForTime(slowMoHint, hintDisplayTime));
            hasShownSlowMoHint = true;
        }
    }

    IEnumerator ShowReloadHintOnceThenAlwaysShowReloadText()
    {
        isShowingReloadHint = true;
        yield return StartCoroutine(ShowHintForTime(reloadHint, hintDisplayTime));

        hasShownReloadHint = true;
        isShowingReloadHint = false;

        ShowHintPersistent(reloadNowHint);
    }
    IEnumerator ShowHintForTime(GameObject hint, float duration)
    {
        yield return StartCoroutine(FadeHint(hint, 0f, 1f));
        yield return new WaitForSeconds(duration);
        yield return StartCoroutine(FadeHint(hint, 1f, 0f));
        hint.SetActive(false);
    }

    void ShowHintPersistent(GameObject hint)
    {
        hint.SetActive(true);
        CanvasGroup cg = hint.GetComponent<CanvasGroup>();
        if (cg == null) cg = hint.AddComponent<CanvasGroup>();
        cg.alpha = 1f;
    }

    void HideHintImmediate(GameObject hint)
    {
        CanvasGroup cg = hint.GetComponent<CanvasGroup>();
        if (cg == null) return;
        cg.alpha = 0f;
        hint.SetActive(false);
    }

    IEnumerator FadeHint(GameObject hint, float from, float to)
    {
        hint.SetActive(true);
        CanvasGroup cg = hint.GetComponent<CanvasGroup>();
        if (cg == null) cg = hint.AddComponent<CanvasGroup>();

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / fadeTime);
            yield return null;
        }

        cg.alpha = to;
    }

    void HideAllHints()
    {
        movementHint.SetActive(false);
        reloadHint.SetActive(false);
        reloadNowHint.SetActive(false);
        slowMoHint.SetActive(false);
    }
}