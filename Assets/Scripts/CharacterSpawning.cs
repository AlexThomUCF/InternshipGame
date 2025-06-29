using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawning : MonoBehaviour
{
    [Header("Character Prefabs (One of Each)")]
    public List<GameObject> characterPrefabs;

    [Header("Spawn Points (Randomized)")]
    public List<Transform> spawnPoints;

    private List<GameObject> spawnedCharacters = new List<GameObject>();

    void Start()
    {
        SpawnCharacters();
    }

    void SpawnCharacters()
    {
        if (characterPrefabs.Count == 0 || spawnPoints.Count == 0)
        {
            Debug.LogWarning("Missing character prefabs or spawn points.");
            return;
        }

        if (spawnPoints.Count < characterPrefabs.Count)
        {
            Debug.LogWarning("Not enough spawn points for all character prefabs.");
            return;
        }

        // Shuffle spawn points so spawn order is random
        List<Transform> shuffledSpawns = new List<Transform>(spawnPoints);
        ShuffleList(shuffledSpawns);

        // Spawn one of each character at a random spawn point
        for (int i = 0; i < characterPrefabs.Count; i++)
        {
            GameObject character = Instantiate(
                characterPrefabs[i],
                shuffledSpawns[i].position,
                shuffledSpawns[i].rotation
            );
            spawnedCharacters.Add(character);
        }

        // Pick one random character to be the IMPOSTER
        int imposterIndex = Random.Range(0, spawnedCharacters.Count);
        for (int i = 0; i < spawnedCharacters.Count; i++)
        {
            if (i == imposterIndex)
                spawnedCharacters[i].tag = "IMPOSTER";
            else
                spawnedCharacters[i].tag = "NPC";
        }
    }

    // Fisher-Yates shuffle
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
