using UnityEngine;
using UnityEngine.InputSystem;

public class FirenAndFlash : MonoBehaviour
{
    [Header("InputAction")]
    //射撃ボタンの設定
    public InputAction fireAction;

    [Header("Fire")]
    //弾丸のモデル
    public GameObject bulletPrefab;
    //弾丸が発射されるポイント（空オブジェクトを銃口先端に配置）
    public Transform firePoint;
    //弾丸のスピードと、撃てるクールタイム（調整可能）
    public float bulletSpeed = 50f;
    public float fireRate = 0.1f;

    [Header("Flash")]
    //マズルフラッシュのエフェクト
    public GameObject muzzleFlashPrefab;
    //マズルフラッシュのポイント（適宜選択、無くても問題なし）
    public Transform muzzleFlashPoint;
    public float muzzleFlashDuration = 0.1f;

    [Tooltip("FlashCT")]
    //マズルフラッシュのクールタイム
    public float flashCooldown = 0.1f;

    private float fireTimer = 0f;
    private float flashTimer = 0f;

    private void OnEnable()
    {
        fireAction.Enable();
    }

    private void OnDisable()
    {
        fireAction.Disable();
    }

    private void Update()
    {
        fireTimer += Time.deltaTime;
        flashTimer += Time.deltaTime;

        if (fireAction.IsPressed())
        {
            if (fireTimer >= fireRate)
            {
                Shoot();
                fireTimer = 0f;
            }
        }
        else
        {
            fireTimer = fireRate;
        }
    }

    private void Shoot()
    {
        // 弾の発射（銃口の向きと発射ポイントの向きが同じであれば安定）
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = firePoint.forward * bulletSpeed;
            }
        }

        // フラッシュ表示
        if (muzzleFlashPrefab != null && muzzleFlashPoint != null && flashTimer >= flashCooldown)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, muzzleFlashPoint.position, muzzleFlashPoint.rotation, muzzleFlashPoint);
            Destroy(flash, muzzleFlashDuration);
            flashTimer = 0f;
        }
    }
}
