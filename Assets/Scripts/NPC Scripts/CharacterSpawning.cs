using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawning : MonoBehaviour
{
    [Header("Character Prefabs (One of Each)")]
    public List<GameObject> characterPrefabs;

    [Header("Spawn Points (Randomized)")]
    public List<Transform> spawnPoints;

    [Header("Layers")]
    public LayerMask charactersLayer; // Assign Characters layer

    private readonly List<GameObject> spawnedCharacters = new();
    public GameObject Imposter { get; private set; }

    void Start() => SpawnCharacters();

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

        // Randomize spawn points
        var shuffledSpawns = new List<Transform>(spawnPoints);
        ShuffleList(shuffledSpawns);

        // Spawn one of each
        for (int i = 0; i < characterPrefabs.Count; i++)
        {
            var prefab = characterPrefabs[i];
            var character = Instantiate(prefab, shuffledSpawns[i].position, shuffledSpawns[i].rotation);
            spawnedCharacters.Add(character);

            // Ensure CharacterRole exists
            var role = character.GetComponent<CharacterRole>();
            if (!role) role = character.AddComponent<CharacterRole>();
            role.isImposter = false;

            // Put on Characters layer
            SetLayerRecursive(character, Mathf.RoundToInt(Mathf.Log(charactersLayer.value, 2)));
        }

        // Choose imposter
        int imposterIndex = Random.Range(0, spawnedCharacters.Count);
        var imposterRole = spawnedCharacters[imposterIndex].GetComponent<CharacterRole>();
        imposterRole.isImposter = true;
        Imposter = spawnedCharacters[imposterIndex];

        Imposter.tag = "IMPOSTER"; //tags the imposter
    }


    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }
    void SetLayerRecursive(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform t in go.transform) SetLayerRecursive(t.gameObject, layer);
    }
}
