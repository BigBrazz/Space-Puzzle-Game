using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    private NPCController npc;

    private Vector3 targetPosition;
    private bool isMoving = false;

    //very bad movement script, use navmesh agent instead after prototype

    private void Awake()
    {
        npc = GetComponent<NPCController>();
    }

    private void Update()
    {
        if (!isMoving) return;
        float speed = npc.Data.MovementSpeed;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) Stop();
    }   
    public void MoveTo(Vector3 position)
    {
        targetPosition = position;
        isMoving = true;
    }

    public void Stop()
    {
        isMoving = false;
    }

}