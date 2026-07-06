using System.Collections.Generic;
using UnityEngine;

public class CustomerAI : NPCBase
{
    [SerializeField] private List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private float waitDuration = 1.5f; // время покупки (пока заглушка)

    public void SetWaypoints(List<Transform> points)
    {
        waypoints = points;
        currentWaypointIndex = 0;
        if (waypoints != null && waypoints.Count > 0)
        {
            MoveToNextWaypoint();
        }
        else
        {
            LeaveStore();
        }
    }

    private void MoveToNextWaypoint()
    {
        if (currentWaypointIndex < waypoints.Count)
        {
            MoveTo(waypoints[currentWaypointIndex].position);
        }
        else
        {
            LeaveStore();
        }
    }

    public override void UpdateState()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                currentWaypointIndex++;
                MoveToNextWaypoint();
            }
            return;
        }

        // Если агент достиг цели
        if (HasReachedTarget())
        {
            // пока имитируем покупку
            if (currentWaypointIndex < waypoints.Count)
            {
                isWaiting = true;
                waitTimer = waitDuration;
                // Здесь позже будет логика взятия товара с полки, пока отладка
                Debug.Log($"Покуп: пришел на точку {currentWaypointIndex}");
            }
            else
            {
                // Если достиг выхода пока ничего не делаем, базовый класс сам уничтожит
            }
        }
    }
}