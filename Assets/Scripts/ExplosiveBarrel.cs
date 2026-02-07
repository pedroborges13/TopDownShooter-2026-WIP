using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    private bool hasExploded = false;

    public void TriggerExplosion()
    {
        if (hasExploded) return;
        hasExploded = true;

        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.GetComponent<Explosion>().Setup(8f, 200f, 40f);

        Destroy(gameObject);
    }
}
