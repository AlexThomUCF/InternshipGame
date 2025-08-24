using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetProjectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private GameObject wrapVfxPrefab; // Possible net wrap effect to spawn on hit later on

    private void Awake()
    {
        // Setup
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Start() => Destroy(gameObject, lifetime);

    private void OnTriggerEnter(Collider other)
    {
        // Ignore triggers
        if (other.isTrigger) return;

        // Find role/catchable on the hit character
        var role = other.GetComponentInParent<CharacterRole>();
        var catchable = other.GetComponentInParent<CatchableCharacter>();

        if (role && catchable)
        {
            catchable.Catch(role.isImposter);
        }

        if (wrapVfxPrefab)
            Instantiate(wrapVfxPrefab, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
