using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject bush1Prefab;
    public GameObject bush2Prefab;

    private void Start()
    {
        Instantiate(bush1Prefab, new Vector3(-4, 2, 0), Quaternion.identity);
        Instantiate(bush1Prefab, new Vector3(-2, 2, 0), Quaternion.identity);

        Instantiate(bush2Prefab, new Vector3(-4, -1, 0), Quaternion.identity);
        Instantiate(bush2Prefab, new Vector3(-2, -1, 0), Quaternion.identity);
    }
}
