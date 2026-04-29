using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    [Header("MaxHP)]
    public float maxHP = 100f;

    [Header("DeadEffect")]
    //オブジェクトが破壊された時のエフェクト
    public GameObject explosionEffect;

    private float currentHP;

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {   //MaxHP - 弾丸のダメージ
        currentHP -= amount;

        if (currentHP <= 0f)
        {   //HPが0になったら破壊
            ExplodeAndDestroy();
        }
    }

    private void ExplodeAndDestroy()
    {
        if (explosionEffect != null)
        {   //破壊されたときにエフェクトが発生
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
