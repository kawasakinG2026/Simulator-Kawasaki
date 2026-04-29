using UnityEngine;

public class EnemyesHP : MonoBehaviour
{
    [Header("MaxHP)]
    public float maxHP = 100f;

    [Header("DeadEffect")]
    public GameObject explosionEffect;

    private float currentHP;

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;

        if (currentHP <= 0f)
        {
            ExplodeAndDestroy();
        }
    }

    private void ExplodeAndDestroy()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
