using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Hints")]
    public GameObject movementHint;
    public GameObject shootingHint;
    public GameObject reloadHint;
    public GameObject slowMoHint;
    
    [Header("Tutorial Settings")]
    public float hintDisplayTime = 5f;
    public float fadeTime = 0.5f;
    
    [Header("Tutorial Triggers")]
    public bool showMovementOnStart = true;
    public bool hasShownShooting = false;
    public bool hasShownReload = false;
    public bool hasShownSlowMo = false;
    
    private player_controller playerController;
    private riflescript rifleScript; 
    private shotgunScript shotgunScript;
    
    void Start()
    {
        // Hide all hints initially
        HideAllHints();
        
        // Find player components (adjust these to match your script names)
        playerController = FindObjectOfType<player_controller>();
        rifleScript = FindObjectOfType<riflescript>();
        shotgunScript = FindObjectOfType<shotgunScript>();
        
        // Show movement hint at start
        if (showMovementOnStart)
        {
            StartCoroutine(ShowHintForTime(movementHint, hintDisplayTime));
        }
    }
    
    void Update()
    {
        CheckForTutorialTriggers();
    }
    
    void CheckForTutorialTriggers()
    {
        // Show shooting hint when zombies appear or after movement
        if (!hasShownShooting && ShouldShowShootingHint())
        {
            StartCoroutine(ShowHintForTime(shootingHint, hintDisplayTime));
            hasShownShooting = true;
        }
        
        // Show reload hint when ammo is low
        if (!hasShownReload && ShouldShowReloadHint())
        {
            StartCoroutine(ShowHintForTime(reloadHint, hintDisplayTime));
            hasShownReload = true;
        }
        
        // Show slow-mo hint during intense moments
        if (!hasShownSlowMo && ShouldShowSlowMoHint())
        {
            StartCoroutine(ShowHintForTime(slowMoHint, hintDisplayTime));
            hasShownSlowMo = true;
        }
    }
    
    bool ShouldShowShootingHint()
    {
        // Show after 3 seconds or when zombies spawn
        return Time.time > 3f;
    }
    
    bool ShouldShowReloadHint()
    {
        // Check both weapon scripts for low ammo (30% or less)
        
        // For rifle - check if current active weapon is rifle and ammo is low
        if (rifleScript != null && rifleScript.gameObject.activeInHierarchy)
        {
            return rifleScript.clipAmount <= rifleScript.clipSize * 0.3f;
        }
        
        // For shotgun - check if current active weapon is shotgun and ammo is low
        if (shotgunScript != null && shotgunScript.gameObject.activeInHierarchy)
        {
            return shotgunScript.clipAmount <= shotgunScript.clipSize * 0.3f;
        }
        
        return Time.time > 10f; // Fallback: show after 10 seconds
    }
    
    bool ShouldShowSlowMoHint()
    {
        // Show when surrounded by enemies or after some time
        return Time.time > 20f; // Show after 20 seconds
    }
    
    IEnumerator ShowHintForTime(GameObject hint, float duration)
    {
        // Fade in
        yield return StartCoroutine(FadeHint(hint, 0f, 1f));
        
        // Wait
        yield return new WaitForSeconds(duration);
        
        // Fade out
        yield return StartCoroutine(FadeHint(hint, 1f, 0f));
        
        hint.SetActive(false);
    }
    
    IEnumerator FadeHint(GameObject hint, float startAlpha, float endAlpha)
    {
        hint.SetActive(true);
        CanvasGroup canvasGroup = hint.GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = hint.AddComponent<CanvasGroup>();
        }
        
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeTime);
            canvasGroup.alpha = alpha;
            yield return null;
        }
        
        canvasGroup.alpha = endAlpha;
    }
    
    void HideAllHints()
    {
        movementHint.SetActive(false);
        shootingHint.SetActive(false);
        reloadHint.SetActive(false);
        slowMoHint.SetActive(false);
    }
    
    // Public methods to manually trigger hints
    public void ShowMovementHint()
    {
        if (!hasShownShooting)
            StartCoroutine(ShowHintForTime(movementHint, hintDisplayTime));
    }
    
    public void ShowReloadHintNow()
    {
        StartCoroutine(ShowHintForTime(reloadHint, hintDisplayTime));
        hasShownReload = true;
    }
}