using UnityEngine;

public class EnemyMeleeColliderScript : MonoBehaviour
{
    [SerializeField] private string playerLayer;
    [SerializeField] private float meleeLifetime = 2f;
    private float meleeDmg;
    private Rigidbody rb;
    private PlayerHealthScript playerHealthScript;

    public void InitializeEnemyMeleeColliderScript(float meleeDmgAmount)
    {
        meleeDmg = meleeDmgAmount;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer(playerLayer))
        {
            playerHealthScript = other.gameObject.GetComponent<PlayerHealthScript>();

            playerHealthScript.TakeDamage(meleeDmg);
        }
        Destroy(gameObject, 0.1f);
    }

    private void MeleeLifetime()
    {
        Destroy(gameObject, meleeLifetime);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MeleeLifetime();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
