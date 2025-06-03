using UnityEngine;

public class ghostHitBox : MonoBehaviour, IDamageCapable
{
    public float ghostHealth = 50;
    public WaveManager waveManager;
    public bool alive = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDMG(float Damage){

        ghostHealth -= Damage;
        Debug.Log("Ghost Health: " + ghostHealth);
        
        if(ghostHealth <= 0){
            Debug.Log("Ghost would have died");
            alive = false;
        }
    }
}
