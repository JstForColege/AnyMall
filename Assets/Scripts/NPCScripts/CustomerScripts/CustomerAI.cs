using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAI : NPCBase
{
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private Transform cashPoint;
    [SerializeField] private int currentWaypointIndex = 0;
    private enum State { MovingToShelf, WaitingAtShelf, MovingToCash, WaitingAtCash }
    private State currentState = State.MovingToShelf;
    private float waitTimer = 0f;
    private float waitDuration = 1.5f;

    public void SetWaypoints(List<Transform> points)
    {
        float threshold = agent.stoppingDistance + 0.2f;
        waypoints = new List<Transform>();
        foreach (Transform p in points)
        {
            if (Vector3.Distance(transform.position, p.position) > threshold)
                waypoints.Add(p);
        }
        if (agent == null)
        {
            Debug.LogWarning("Agent not initialized yet");
            StartCoroutine(DelayedSetWaypoints(points));
            return;
        }
        if (waypoints.Count > 0)
        {
            currentWaypointIndex = 0;
            currentState = State.MovingToShelf;
            MoveToNextWaypoint();
        }
        else
        {
            GoToCash();
        }
    }

    public void SetCashPoint(Transform point)
    {
        cashPoint = point;
    }

    private void MoveToNextWaypoint()
    {
        if (currentWaypointIndex < waypoints.Count)
        {
            MoveTo(waypoints[currentWaypointIndex].position);
            currentState = State.MovingToShelf;
        }
        else
        {
            GoToCash();
        }
    }

    private void GoToCash()
    {
        if (cashPoint != null)
        {
            MoveTo(cashPoint.position);
            currentState = State.MovingToCash;
            Debug.Log("Customer: Going to cash");
        }
        else
        {
            LeaveStore();
        }
    }
    private IEnumerator DelayedSetWaypoints(List<Transform> points)
    {
        yield return new WaitForSeconds(0.1f);
        SetWaypoints(points);
    }
    public override void UpdateState()
    {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh)
            return;

        switch (currentState)
        {
            case State.MovingToShelf:
                if (HasReachedTarget())
                {
                    currentState = State.WaitingAtShelf;
                    waitTimer = waitDuration;
                    Debug.Log($"Customer: Arrived at shelf {currentWaypointIndex}");
                }
                break;

            case State.WaitingAtShelf:
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    currentWaypointIndex++;
                    MoveToNextWaypoint();
                }
                break;

            case State.MovingToCash:
                if (HasReachedTarget())
                {
                    currentState = State.WaitingAtCash;
                    Debug.Log("Customer: Waiting at cash");
                }
                break;

            case State.WaitingAtCash:
                break;
        }
    }

    public void OnPaymentDone()
    {
        LeaveStore();
        Debug.Log("Customer: Payment done, leaving");
    }
}