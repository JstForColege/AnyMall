using UnityEngine;
using System.Collections;

public class HooliganAI : NPCBase
{
    private Transform targetShelf;
    private enum State { MovingToShelf, Acting, Fleeing }
    private State currentState = State.MovingToShelf;

    private Transform playerTransform;
    private float fleeDistance = 3f;

    private HooliganSpawner spawner;

    private float throwInterval = 5f;
    private float throwTimer = 0f;
    private Storage shelfStorage;

    [SerializeField] private GameObject droppedItemPrefab;
    [SerializeField] private Transform dropPoint;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;

        if (dropPoint == null)
            dropPoint = transform;
    }

    public void SetTargetShelf(Transform shelf)
    {
        targetShelf = shelf;
        if (targetShelf != null)
        {
            shelfStorage = targetShelf.GetComponent<Storage>();
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
                    throwTimer = 0f;
                    Debug.Log("Хулиган: начал шалить");
                }
                break;

            case State.Acting:
                throwTimer += Time.deltaTime;
                if (throwTimer >= throwInterval)
                {
                    throwTimer = 0f;
                    ThrowItemFromShelf();
                }
                break;

            case State.Fleeing:
                break;
        }
    }

    private void ThrowItemFromShelf()
    {
        if (shelfStorage == null) return;

        ItemData item = shelfStorage.RemoveItem();
        if (item == null)
        {
            Debug.Log("Хулиган: полка пуста");
            return;
        }

        if (droppedItemPrefab != null)
        {
            GameObject droppedObj = Instantiate(droppedItemPrefab, dropPoint.position, Quaternion.identity);
            droppedObj.transform.localScale = new Vector3(0.1998988f, 0.1998988f, 1f);

            DroppedItem dropped = droppedObj.GetComponent<DroppedItem>();
            if (dropped != null)
                dropped.Initialize(item);
            else
            {
                SpriteRenderer sr = droppedObj.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sprite = item.Icon;
                dropped = droppedObj.AddComponent<DroppedItem>();
                dropped.Initialize(item);
            }
        }
        else
        {
            GameObject droppedObj = new GameObject("DroppedItem");
            droppedObj.transform.position = dropPoint.position;
            droppedObj.transform.localScale = new Vector3(0.1998988f, 0.1998988f, 1f);

            SpriteRenderer sr = droppedObj.AddComponent<SpriteRenderer>();
            sr.sprite = item.Icon;
            sr.sortingOrder = 1;
            BoxCollider2D collider = droppedObj.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            DroppedItem dropped = droppedObj.AddComponent<DroppedItem>();
            dropped.Initialize(item);
        }

        Debug.Log($"Хулиган: выбросил {item.Type} на пол");
    }

    public bool TryChaseAway()
    {
        if (currentState == State.Acting)
        {
            currentState = State.Fleeing;
            LeaveStore();
            Debug.Log("Хулиган: был прогнан!");
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