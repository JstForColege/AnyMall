using UnityEngine;

public class BuyZone : MonoBehaviour
{
    /*[SerializeField]
    private int _price;*/

    [SerializeField]
    private Spawner _spawner;
    private bool _isPlayerIn;
    private bool _isPurchased;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isPurchased)
            return;
        if (other.CompareTag("Player"))
        {
            _isPurchased = true;
            _spawner.Spawn();
        }
    }
}
