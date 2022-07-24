using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] GameObject[] Waypoints;
    [SerializeField] float distance;

    private int currentTargetIndex;

    void Start()
    {
        currentTargetIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, Waypoints[currentTargetIndex].transform.position, Time.deltaTime / 10);

        if (Vector3.Distance(transform.position, Waypoints[currentTargetIndex].transform.position) < distance)
            if (Waypoints.Length - 1 == currentTargetIndex)
                currentTargetIndex = 0;
            else
                currentTargetIndex++;
    }
}
