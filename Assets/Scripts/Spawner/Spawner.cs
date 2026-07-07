using System;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private ResourceNode _prefab;

    public void Spawn()
    {
        Instantiate(_prefab, transform.position, Quaternion.identity);
    }

    //Instantiate(bush2Prefab, new Vector3(-2, -1, 0), Quaternion.identity);
}
 