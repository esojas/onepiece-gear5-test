using UnityEngine;

public class ThunderExplode : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float radius = 5.0f;
    [SerializeField] private float power = 10.0f;
    [SerializeField] private float upwardsModifier = 3.0f;
    [SerializeField] private ForceMode forceMode = ForceMode.Force;
    [SerializeField] private GameObject thunderExplodeVFXPrefab;
    [SerializeField] private string enemyLayer;
    [SerializeField] private float colliderLifetime = .5f;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(enemyLayer))
        {
            other.attachedRigidbody.AddExplosionForce(power,transform.position, radius, upwardsModifier, forceMode);
        }
    }

    private void OnThunderExplode(Vector3 explodePos)
    {
        Instantiate(thunderExplodeVFXPrefab, explodePos, Quaternion.identity);
    
        Destroy(gameObject, colliderLifetime); // destroy the collider afterward
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnThunderExplode(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
