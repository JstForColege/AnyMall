using UnityEngine;
using UnityEngine.AI;

public abstract class NPCBase : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Transform exitPoint;
    protected bool hasStartedMoving = false;

    [SerializeField] protected Animator animator;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    public abstract void UpdateState();

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            agent = gameObject.AddComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

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

    protected virtual void UpdateAnimation()
    {
        if (animator == null) return;
        bool isMoving = agent.isOnNavMesh && agent.velocity.magnitude > 0.1f;
        animator.SetBool("isMoving", isMoving);
        if (agent.isOnNavMesh)
        {
            float dirX = agent.velocity.x;
            if (Mathf.Abs(dirX) > 0.1f)
            {
                bool newFlip = dirX < 0;
                if (spriteRenderer != null)
                    spriteRenderer.flipX = newFlip;
                UpdateHandFlip();
            }
        }
    }

    protected virtual void UpdateHandFlip()
    {
        // Переопределяется в CustomerAI
    }

    protected virtual void Update()
    {
        UpdateState();
        UpdateAnimation();

        if (exitPoint != null && HasReachedTarget() && Vector3.Distance(transform.position, exitPoint.position) < 0.5f)
        {
            Destroy(gameObject);
        }
    }
}