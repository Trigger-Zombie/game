using UnityEngine;

public class TimeManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float slowDownFactor = 0.05f;
    public float slowDownLength = 5f;
    private float startTime = -1;

     public AudioSource slowMoSound;
    void Update()
    {
        if(startTime >= 0f)
        {
            float elapsedTime = Time.realtimeSinceStartup - startTime;
            if(elapsedTime >= 2f)
            {
                // More stable calculation to return to normal time
                float targetValue = Mathf.Min(1.0f, Time.timeScale + (1f/slowDownLength) * Time.unscaledDeltaTime);
                Time.timeScale = targetValue;
                
                // Update fixed delta time to match
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                
                // Reset startTime when we return to normal time
                if(Time.timeScale >= 0.99f)
                {
                    Time.timeScale = 1f;
                    startTime = -1f;
                }
            }
        }
    }

    public void DoSlowMotion()
    {
        if(slowMoSound != null){
            slowMoSound.Play();
        }
        Time.timeScale = slowDownFactor; //slows down game time
        Time.fixedDeltaTime = Time.timeScale * .02f; //increases the update time of fixed update so physics are smoother
        startTime = Time.realtimeSinceStartup;
    }
}
