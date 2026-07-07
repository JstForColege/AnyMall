using UnityEngine;
using UnityEngine.AI;

public abstract class NPCBase : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Transform exitPoint;

    public abstract void UpdateState();

    protected virtual void Start()
    {

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            agent = gameObject.AddComponent<NavMeshAgent>();

        GameObject exit = GameObject.FindGameObjectWithTag("ExitPoint");
        if (exit != null) exitPoint = exit.transform;

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public void MoveTo(Vector3 target)
    {
        if (agent != null && agent.isActiveAndEnabled)
            agent.SetDestination(target);
    }

    public bool HasReachedTarget()
    {
        return agent.remainingDistance <= agent.stoppingDistance;
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