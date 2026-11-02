using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetProjectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private GameObject wrapVfxPrefab;

    private void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        var role = other.GetComponentInParent<CharacterRole>();
        var catchable = other.GetComponentInParent<CatchableCharacter>();

        if (role && catchable)
        {
            // Play the character's hit sound.
            AudioSource characterAudio =
                catchable.GetComponent<AudioSource>();

            if (characterAudio != null)
            {
                characterAudio.Play();
            }

            // Catch the character.
            catchable.Catch(role.isImposter);
        }

        if (wrapVfxPrefab)
        {
            Instantiate(
                wrapVfxPrefab,
                transform.position,
                transform.rotation
            );
        }

        Destroy(gameObject);
    }
}