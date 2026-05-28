// On your Character controller script
using UnityEngine;

public class PlayerFootstep : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        //TriggerWaveAt(transform.position);
    }

    //private void TriggerWaveAt(Vector3 footPosition)
    //{
    //    // Raycast down to find the exact surface point
    //    if (Physics.Raycast(footPosition + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, 0.5f))
    //    {
    //        var surface = hit.collider.GetComponent<BounceReceiver>();
    //        surface?.TriggerWave(hit.point);
    //    }
    //}
}