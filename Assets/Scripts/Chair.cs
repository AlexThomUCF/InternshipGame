using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Chair : MonoBehaviour
{
    public Transform[] sittingSpots;
    public Transform lookPosition;

    public void NpcChair(AINavigation npc)
    {
        int randomIndex = Random.Range(0, sittingSpots.Length);

        npc.myAgent.enabled = false;

        npc.transform.position = sittingSpots[randomIndex].position;
        npc.transform.rotation = sittingSpots[randomIndex].rotation;

        StartCoroutine(WaitForSitAnimation(npc));
    }

    private IEnumerator WaitForSitAnimation(AINavigation npc)
    {
        yield return new WaitUntil(() =>
            npc.animator.GetCurrentAnimatorStateInfo(0).IsTag("Task")
        );

        yield return new WaitUntil(() =>
            npc.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f &&
            !npc.animator.IsInTransition(0)
        );

        ReturnNpcToNavMesh(npc);
    }

    private void ReturnNpcToNavMesh(AINavigation npc)
    {
        NavMeshHit hit;

        if (NavMesh.SamplePosition(npc.transform.position, out hit, 3f, NavMesh.AllAreas))
        {
            npc.myAgent.enabled = true;
            npc.myAgent.Warp(hit.position);
            npc.myAgent.isStopped = false;
        }
    }
}