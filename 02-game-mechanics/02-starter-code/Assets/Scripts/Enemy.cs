using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float MoveSpeed = 5;
    public Transform[] waypoints;
    private int currentWaypointIndex;

    private Vector3 lastPosition;
    [SerializeField] private GameObject body;

    private void Awake()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (currentWaypointIndex == waypoints.Length)
        {
            //GameManager.Instance.healthIndicators
            Destroy(gameObject);
            GameManager.Instance.Health -= 1;
            return;
        }

        Transform toWaypoint = waypoints[currentWaypointIndex];
        Vector2 moveVector = Vector2.MoveTowards(transform.position, toWaypoint.position, MoveSpeed * Time.deltaTime);
        transform.position = (Vector3)moveVector;

        if (Vector2.Distance(transform.position, toWaypoint.position) <= float.Epsilon)
        {
            currentWaypointIndex++;
        }

        RotateIntoMoveDirection();
        lastPosition = transform.position;

    }
    private void RotateIntoMoveDirection()
    {
        Vector2 newDirection = (transform.position - lastPosition);
        body.transform.right = newDirection;
    }
}
