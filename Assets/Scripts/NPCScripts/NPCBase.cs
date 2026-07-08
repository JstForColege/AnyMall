using UnityEngine;
using UnityEngine.AI;

public abstract class NPCBase : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Transform exitPoint;
    protected bool hasStartedMoving = false;

    public abstract void UpdateState();

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            agent = gameObject.AddComponent<NavMeshAgent>();

        GameObject exit = GameObject.FindGameObjectWithTag("ExitPoint");
        if (exit != null) exitPoint = exit.transform;

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.stoppingDistance = 0.1f;
    }

    public void MoveTo(Vector3 target)
    {
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.SetDestination(target);
            hasStartedMoving = true;
        }
    }

    public bool HasReachedTarget()
    {
        if (!hasStartedMoving)
            return false;

        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh)
            return false;

        return agent.remainingDistance <= agent.stoppingDistance + 0.1f;
    }

    public virtual void LeaveStore()
    {
        if (exitPoint != null)
            MoveTo(exitPoint.position);
    }

    protected virtual void Update()
    {
        UpdateState();
        if (exitPoint != null && HasReachedTarget() && Vector3.Distance(transform.position, exitPoint.position) < 0.5f)
        {
            Destroy(gameObject);
        }
    }
}