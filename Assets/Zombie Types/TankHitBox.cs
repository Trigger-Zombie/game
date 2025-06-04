using UnityEngine;

public class TankHitBox : MonoBehaviour, IDamageCapable
{
    public float TankHealth = 200;
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

        TankHealth -= Damage;
        Debug.Log("Tank Health: " + TankHealth);
        
        if(TankHealth <= 0){
            Debug.Log("Tank would have died");
            alive = false;
        }
    }
}
