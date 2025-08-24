using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        // DontDestroyOnLoad(gameObject);
    }

    public void OnCharacterCaught(GameObject who, bool wasImposter)
    {
        if (wasImposter)
        {
            Debug.Log("WIN: You netted the imposter!");
            // Still needs show win UI, stop timer, progress level, etc.
        }
        else
        {
            Debug.Log("PENALTY: Innocent caught.");
            // Still needs some kind of penalty (time loss, lock ability, etc.)
        }
    }
}
