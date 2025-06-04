using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class TankHitDetect : MonoBehaviour
{
    public Transform player;
    private player_controller playerController;
    public bool playerCurrentlyInTrigger = false;
    public int damageAmount = 25;
    void Start()
    {
        playerCurrentlyInTrigger = false;
        playerController = player.GetComponent<player_controller>();
    }

    // Update is called once per frame
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCurrentlyInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCurrentlyInTrigger = false;
        }
    }

}
