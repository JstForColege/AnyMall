using UnityEngine;

public class HooliganAI : NPCBase
{
    private Transform targetShelf;
    private enum State { MovingToShelf, Acting, Fleeing }
    private State currentState = State.MovingToShelf;

    private Transform playerTransform;
    private float fleeDistance = 3f;

    private HooliganSpawner spawner;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
    }

    public void SetTargetShelf(Transform shelf)
    {
        targetShelf = shelf;
        if (targetShelf != null)
        {
            MoveTo(targetShelf.position);
            currentState = State.MovingToShelf;
            Debug.Log("Хулиган: иду к полке");
        }
        else
        {
            LeaveStore();
        }
    }

    public void SetSpawner(HooliganSpawner spawnerRef)
    {
        spawner = spawnerRef;
    }

    public override void UpdateState()
    {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh)
            return;
        if (currentState == State.Acting &&
            playerTransform != null &&
            Vector3.Distance(transform.position, playerTransform.position) < fleeDistance)
        {
            currentState = State.Fleeing;
            LeaveStore();
            Debug.Log("Хулиган: игрок прогнал");
            return;
        }

        switch (currentState)
        {
            case State.MovingToShelf:
                if (HasReachedTarget())
                {
                    currentState = State.Acting;
                    Debug.Log("Хулиган: начал шалить");
                }
                break;

            case State.Acting:

                break;

            case State.Fleeing:
                break;
        }
    }

    public bool TryChaseAway()
    {
        if (currentState == State.Acting)
        {
            currentState = State.Fleeing;
            LeaveStore();
            Debug.Log("Хулиган: был прогнана!");
            return true;
        }
        else
        {
            Debug.Log("Хулиган: нельзя прогнать");
            return false;
        }
    }

    private void OnDestroy()
    {
        if (spawner != null)
            spawner.OnHooliganLeft();
    }
}