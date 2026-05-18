using UnityEngine;
using static UnityEngine.UI.Image;

public class ThunderRodScript : MonoBehaviour
{

    [SerializeField] private GameObject explodeThunder;
    [SerializeField] private string groundLayer;

    Vector3 direction;
    Vector3 origin = Vector3.zero;
    float speed;
    bool hasExploded = false;

    bool lightningObjectIsInitialized = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(groundLayer))
        {
            hasExploded = true;
            ActivateExplode(transform.position);
        }
    }

    public void InitializedLightningRod(Vector3 originSpawn, Vector3 dir, float lightningSpeed)
    {
        origin = originSpawn;
        direction = dir.normalized;
        speed = lightningSpeed;
        lightningObjectIsInitialized = true;
        transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90f, 0f, 0f); ;
    }

    private void LightningBeingThrown()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void ActivateExplode(Vector3 explodePos)
    {
        Instantiate(explodeThunder, explodePos, Quaternion.identity);
        Destroy(gameObject, .01f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //transform.rotation = Quaternion.Euler();
    }

    // Update is called once per frame
    void Update()
    {
        if (lightningObjectIsInitialized && !hasExploded)
        {
            LightningBeingThrown();
        }
    }
}
