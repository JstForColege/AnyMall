using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAI : NPCBase
{
    private List<Transform> waypoints;
    private List<ItemType> shoppingList;
    private int currentItemIndex = 0;

    private Transform cashPoint;
    private enum State { MovingToShelf, WaitingAtShelf, MovingToCash, WaitingAtCash }
    private State currentState = State.MovingToShelf;
    private float waitTimer = 0f;
    private float waitDuration = 0.5f;

    private CustomerSpawner spawner;
    private CashRegister cashRegister;
    private bool isRegistered = false;

    [SerializeField] private Transform handPosition;
    private List<GameObject> handItems = new List<GameObject>();

    private float stackOffsetY = 1f; 
    private float dragMultiplier = 0.2f;
    private float maxDrag = 0.5f;        
    private GameObject handItemObject;


    public void SetShoppingList(List<ItemType> items)
    {
        shoppingList = items;
        currentItemIndex = 0;
    }

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
        if (agent == null) yield break;
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
            currentItemIndex = 0;
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
        if (currentItemIndex < waypoints.Count)
        {
            MoveTo(waypoints[currentItemIndex].position);
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
                    TryTakeItem();
                }
                break;

            case State.WaitingAtShelf:
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    waitTimer = waitDuration;
                    TryTakeItem();
                }
                break;

            case State.MovingToCash:
                if (HasReachedTarget())
                {
                    if (!isRegistered && cashRegister != null)
                    {
                        cashRegister.RegisterCustomer(this, shoppingList);
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

    private void TryTakeItem()
    {
        if (currentItemIndex >= waypoints.Count)
        {
            GoToCash();
            return;
        }

        Transform shelfTransform = waypoints[currentItemIndex];
        Storage shelf = shelfTransform.GetComponent<Storage>();
        if (shelf == null)
        {
            Debug.LogWarning($"Shelf at {shelfTransform.name} has no Storage component!");
            currentItemIndex++;
            MoveToNextWaypoint();
            return;
        }

        ItemData item = shelf.RemoveItem();
        if (item != null)
        {
            Debug.Log($"Customer: took {item.Type} from shelf {currentItemIndex}");
            UpdateHand(item);
            currentItemIndex++;
            MoveToNextWaypoint();
        }
        else
        {
        }
    }

    private void UpdateHand(ItemData item)
    {
        if (item == null || item.Icon == null || handPosition == null)
            return;

        // Создаём новый предмет
        GameObject newItem = new GameObject("HandItem_" + handItems.Count);
        newItem.transform.SetParent(handPosition);
        newItem.transform.localScale = Vector3.one;

        SpriteRenderer sr = newItem.AddComponent<SpriteRenderer>();
        sr.sprite = item.Icon;
        sr.sortingLayerName = spriteRenderer != null ? spriteRenderer.sortingLayerName : "Default";
        sr.sortingOrder = spriteRenderer != null ? spriteRenderer.sortingOrder + 1 : 1;
        sr.flipX = spriteRenderer != null && spriteRenderer.flipX;

        handItems.Add(newItem);
        UpdateHandPositions();
    }

    private void UpdateHandPositions()
    {
        if (handItems.Count == 0) return;

        float velocityX = 0f;
        if (agent != null && agent.isOnNavMesh)
            velocityX = agent.velocity.x;

        bool isMoving = Mathf.Abs(velocityX) > 0.1f;

        for (int i = 0; i < handItems.Count; i++)
        {
            GameObject obj = handItems[i];
            if (obj == null) continue;

            float offsetY = i * stackOffsetY;

            float dragX = 0f;
            if (isMoving)
            {
                float drag = -velocityX * dragMultiplier * (i + 1);
                dragX = Mathf.Clamp(drag, -maxDrag, maxDrag);
            }

            obj.transform.localPosition = new Vector3(dragX, offsetY, 0f);
        }
    }

    private void ClearHand()
    {
        foreach (GameObject obj in handItems)
            if (obj != null) Destroy(obj);
        handItems.Clear();
    }

    protected override void UpdateHandFlip()
    {
        if (spriteRenderer == null) return;
        bool flip = spriteRenderer.flipX;
        foreach (GameObject obj in handItems)
        {
            if (obj == null) continue;
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null) sr.flipX = flip;
        }
    }

    protected override void Update()
    {
        base.Update();
        UpdateHandPositions();
    }

    public void OnPaymentDone()
    {
        ClearHand();
        if (spawner != null)
            spawner.OnCustomerLeft(this);
        LeaveStore();
    }

    private void OnDestroy()
    {
        ClearHand();
        if (spawner != null)
            spawner.OnCustomerLeft(this);
    }
}