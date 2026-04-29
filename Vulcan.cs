using UnityEngine;
using UnityEngine.InputSystem;

public class Vulcan : MonoBehaviour
{
    [Header("InputAction")]
     //射撃ボタンの設定
    public InputAction fireAction;

    [Header("Fire")]
    //弾丸のモデル
    public GameObject bulletPrefab;
    //弾丸が発射されるポイント（空オブジェクトを頭部に設置）
    public Transform firePoint;
    public float bulletSpeed = 50f;
    public float fireRate = 0.1f;

    [Header("移動方向補正")]
    [Tooltip("移動方向への補正強度（0〜1程度）")]
    //高速移動時の命中精度を上げるための補正
    public float movementInfluence = 0.4f;

    [Header("Flash")]
    public GameObject muzzleFlashPrefab;
    public Transform muzzleFlashPoint;
    public float muzzleFlashDuration = 0.1f;

    [Tooltip("FlashCT")]
    public float flashCooldown = 0.1f;

    private float fireTimer = 0f;
    private float flashTimer = 0f;

    private Rigidbody parentRb;

    private void Awake()
    {
        // 本体のRigidbody取得（移動補正のため）
        parentRb = GetComponentInParent<Rigidbody>();
    }

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
        // ===== 弾の発射 =====
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(
                bulletPrefab,
                firePoint.position,
                firePoint.rotation
            );

            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 finalDirection = firePoint.forward;

                // 本体の移動方向を少し加える
                if (parentRb != null)
                {
                    Vector3 moveVelocity = parentRb.linearVelocity;

                    if (moveVelocity.magnitude > 0.1f)
                    {
                        Vector3 moveInfluence =
                            moveVelocity.normalized * movementInfluence;

                        finalDirection =
                            (firePoint.forward + moveInfluence).normalized;
                    }
                }

                rb.linearVelocity = finalDirection * bulletSpeed;
            }
        }

        // ===== マズルフラッシュ =====
        if (muzzleFlashPrefab != null &&
            muzzleFlashPoint != null &&
            flashTimer >= flashCooldown)
        {
            GameObject flash = Instantiate(
                muzzleFlashPrefab,
                muzzleFlashPoint.position,
                muzzleFlashPoint.rotation,
                muzzleFlashPoint
            );

            Destroy(flash, muzzleFlashDuration);
            flashTimer = 0f;
        }
    }
}
