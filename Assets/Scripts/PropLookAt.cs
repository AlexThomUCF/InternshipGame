using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropLookAt : MonoBehaviour
{
    public Transform lookSpot;
    // Start is called before the first frame update
    void Start()
    {
        lookSpot = GameObject.Find("PondCenter").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = lookSpot.position - transform.position;
        transform.rotation = Quaternion.FromToRotation(-Vector3.right, direction);
    }
}
