using UnityEngine;

public class ThunderExplodeVFXScript : MonoBehaviour
{
    [SerializeField] private float thunderVFXLifetime; // Set it as the longest particle time. In this case its 2 seconds

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, thunderVFXLifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
