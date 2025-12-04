using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float lifeTime = 1.5f; // Time before deletion

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}