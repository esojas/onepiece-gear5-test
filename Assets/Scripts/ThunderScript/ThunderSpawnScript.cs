using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class ThunderSpawnScript : MonoBehaviour
{
    Transform cubeSpawn;
    [SerializeField] private float lightningInterval;
    private bool lightningCanSpawn = false;

    [SerializeField] private GameObject lightningPrefab;
    //private Coroutine lightningIntervalCoroutine;

    private void SpawnLightning()
    {
        float randomX = Random.Range(transform.position.x - transform.localScale.x / 2, transform.position.x + transform.localScale.x / 2);
        float randomY = Random.Range(transform.position.y - transform.localScale.y / 2, transform.position.y + transform.localScale.y / 2);
        float randomZ = Random.Range(transform.position.z - transform.localScale.z / 2, transform.position.z + transform.localScale.z / 2);
        Vector3 randomSpawnPos = new Vector3(randomX,randomY,randomZ);
        GameObject lightningGameObject = Instantiate(lightningPrefab,randomSpawnPos,Quaternion.Euler(-90,0,0));

        ParticleSystem lightningParticleSystem = lightningGameObject.GetComponentInChildren<ParticleSystem>();

        float lightningVFXLifetime = lightningParticleSystem ? lightningParticleSystem.main.duration : 5; // the default is 5

        float lightningVFXStartLifetime = lightningParticleSystem.main.startLifetime.constant;

        Destroy(lightningGameObject, lightningVFXLifetime + lightningVFXStartLifetime);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lightningCanSpawn = true;
        StartCoroutine(LightningSpawnIntervalCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LightningSpawnIntervalCoroutine()
    {
        while (lightningCanSpawn)
        {
            yield return new WaitForSeconds(lightningInterval);

            SpawnLightning();
        }
    }

}
