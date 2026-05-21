using UnityEngine;

public class DeathVFXScript : MonoBehaviour
{
    [SerializeField] private float deathVFXLifetime; // Set it as the longest particle time. In this case its 2 seconds

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, deathVFXLifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
