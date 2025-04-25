using UnityEngine;
public class BeamObjective : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Beam reached!");
            Destroy(gameObject); // This removes the beam when the player gets it
        }
    }
}
