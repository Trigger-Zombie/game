using UnityEngine;

public class zombieHitbox : MonoBehaviour, IDamageCapable
{
    public float zombieHealth = 100;
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

        zombieHealth -= Damage;
        Debug.Log("Zombie Health: " + zombieHealth);
        
        if(zombieHealth <= 0){
            Debug.Log("Zombie would have died");
            alive = false;
        }
    }
}
