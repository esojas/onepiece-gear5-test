using UnityEngine;

public class EnemyBulletColliderScript : MonoBehaviour
{
    [SerializeField] private float bulletForce = 4f;
    [SerializeField] private string playerLayer, groundLayer, wallLayer;
    [SerializeField] private float bulletLifetime = 10f;
    private float bulletDmg;
    private Rigidbody rb;
    private PlayerHealthScript playerHealthScript;


    public void InitializedEnemyBulletColliderScript(float bulletDmgAmount)
    {
        bulletDmg = bulletDmgAmount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(playerLayer))
        {
            playerHealthScript = other.gameObject.GetComponent<PlayerHealthScript>();

            playerHealthScript.TakeDamage(bulletDmg);
            Destroy(gameObject, 0.01f);
        }
        Destroy(gameObject, 0.01f);
    }

    private void BulletTrajectory()
    {
        rb.AddForce(transform.forward * bulletForce,ForceMode.Impulse);
    }

    private void BulletLifetime()
    {
        Destroy(gameObject, bulletLifetime);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BulletLifetime();
        BulletTrajectory();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
