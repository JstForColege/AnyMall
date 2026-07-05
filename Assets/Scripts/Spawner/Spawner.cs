using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject bushPrefab;

    private void Start()
    {
        Instantiate(bushPrefab, new Vector3(-5, -5, 0), Quaternion.identity);
        Instantiate(bushPrefab, new Vector3(-2, 2, 0), Quaternion.identity);
    }
}
