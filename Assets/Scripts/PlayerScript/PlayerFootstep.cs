// On your Character controller script
using UnityEngine;
using SmallHedge.SoundManager;

public class PlayerFootstep : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer, wallLayer;
    [SerializeField] private float minMoveSpeed = 0.1f;
    [SerializeField] private Rigidbody rb;

    private void OnTriggerEnter(Collider other)
    {
        if (rb.linearVelocity.magnitude < minMoveSpeed) return;

        int otherMask = 1 << other.gameObject.layer;
        if ((otherMask & groundLayer) != 0 || (otherMask & wallLayer) != 0)
        {

            BounceReceiver receiver = other.GetComponent<BounceReceiver>();
            if (receiver == null) return;
            receiver.Bounce(transform.position, Vector3.up);
        }
    }

}