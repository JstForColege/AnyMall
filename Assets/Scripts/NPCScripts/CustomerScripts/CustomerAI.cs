using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAI : NPCBase
{
    private List<Transform> waypoints;
    private Transform cashPoint;
    private int currentWaypointIndex = 0;
    private enum State { MovingToShelf, WaitingAtShelf, MovingToCash, WaitingAtCash }
    private State currentState = State.MovingToShelf;
    private float waitTimer = 0f;
    private float waitDuration = 1.5f;

    private CustomerSpawner spawner;
    private CashRegister cashRegister;
    private bool isRegistered = false;

    public void SetWaypoints(List<Transform> points)
    {
        if (agent == null)
        {
            StartCoroutine(DelayedSetWaypoints(points));
            return;
        }

        ApplyWaypoints(points);
    }

    private IEnumerator DelayedSetWaypoints(List<Transform> points)
    {
        yield return new WaitForSeconds(0.1f);
        if (agent == null)
        {
            yield break;
        }
        ApplyWaypoints(points);
    }

    private void ApplyWaypoints(List<Transform> points)
    {
        float threshold = agent.stoppingDistance + 0.2f;
        waypoints = new List<Transform>();
        foreach (Transform p in points)
        {
            if (Vector3.Distance(transform.position, p.position) > threshold)
                waypoints.Add(p);
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

        if (cashPoint != null)
        {
            cashRegister = cashPoint.GetComponent<CashRegister>();
            if (cashRegister == null)
                Debug.LogWarning("CashRegister component not found on cash point!");
        }
    }

    public void SetSpawner(CustomerSpawner spawnerRef)
    {
        spawner = spawnerRef;
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
        }
        else
        {
            LeaveStore();
        }
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
                    if (!isRegistered && cashRegister != null)
                    {
                        cashRegister.RegisterCustomer(this);
                        isRegistered = true;
                        currentState = State.WaitingAtCash;
                    }
                    else
                    {
                        currentState = State.WaitingAtCash;
                    }
                }
                break;

            case State.WaitingAtCash:
                break;
        }
    }

    public void OnPaymentDone()
    {
        if (spawner != null)
        {
            spawner.OnCustomerLeft(this);
        }

        LeaveStore();
    }

    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.OnCustomerLeft(this);
        }
    }
}