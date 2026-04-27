using UnityEngine;
using UnityEngine.UI;

public class AimAssist : MonoBehaviour
{
    [Header("参照")]
    public Transform rightArm;        // 上腕（肩関節）
    public Transform forearm;         // 肘関節
    public Transform head;            // 頭部
    public Transform gunMuzzle;       // 銃口
    public Camera cam;

    [Header]//エイムアシスト設定
    public float assistRange = 750f; //エイムアシストをさせる距離（Unity基準で、単位1あたり1mを想定）
    public float assistFovAngle = 15f; //カメラの中心からエイムアシストの判定を与える範囲の角度
    public LayerMask enemyLayer; //ターゲット判定を与えるレイヤー

    [Header]//追従速度設定
    public float armRotateSpeed = 10f;
    public float headRotateSpeed = 6f;

    [Header]//腕オフセット
    public Vector3 localRotationOffset = new Vector3(0f, 0f, 0f); //操作機体ごとに微調整可能
    public Vector3 elbowRotationOffset = Vector3.zero;

    [Header]//頭オフセット
    public Vector3 headRotationOffset = Vector3.zero;

    [Header]//腕の回転制限
    public float minPitch = -60f;
    public float maxPitch = 60f;

    [Header]//頭部の回転制限（ローカル角度で）
    public Vector2 headPitchLimit = new Vector2(-45f, 45f);
    public Vector2 headYawLimit   = new Vector2(-70f, 70f);
    public Vector2 headRollLimit  = new Vector2(-20f, 20f);

    [Header]//UI
    public Image assistReticle; //ファイル形式はPNGを想定
    public float reticleVisibleAlpha = 1f;

    private Transform targetEnemy;
    private Quaternion initialArmRotation;
    private Quaternion initialElbowRotation;
    private Quaternion initialHeadRotation;

    void Start()
    {
        if (rightArm != null)
        {
            // 初期回転（Z-180 + X/Y調整）
            initialArmRotation = rightArm.localRotation * Quaternion.Euler(180f, 180f, 180f);
        }
        if (forearm != null)
            initialElbowRotation = forearm.localRotation;
        if (head != null)
            initialHeadRotation = head.localRotation;
    }

    void Update()
    {
       //範囲内＆視野内のターゲットを検索
        FindTarget();

        if (targetEnemy != null)
        {
            //腕のエイムアシスト
            AssistAim();
            //頭部もターゲット方向へ追従
            AssistHead();
            //レティクル表示
            ShowReticle(true);
        }
        else
        {
            //範囲にターゲットがいない場合は姿勢を元に戻す
            ResetArmOffset();
            ResetHeadOffset();
            //レティクル表示（非表示）
            ShowReticle(false);
        }
    }

    void FindTarget()
    {
        //一定範囲内の敵を検知
        Collider[] hits = Physics.OverlapSphere(transform.position, assistRange, enemyLayer);
        Transform best = null;
        float bestAngle = Mathf.Infinity;

        foreach (var hit in hits)
        {
　　　　　　 //カメラから見た方向へのベクトル
            Vector3 dir = hit.transform.position - cam.transform.position;
            //カメラ正面との角度差を計算
            float angle = Vector3.Angle(cam.transform.forward, dir);
            //視野角内かつ最も近い敵を選択
            if (angle < assistFovAngle && angle < bestAngle)
            {
                best = hit.transform;
                bestAngle = angle;
            }
        }

        targetEnemy = best;
    }

    void AssistAim()
    {
        //ターゲット方向を取得
        Vector3 dir = targetEnemy.position - rightArm.position;
        //操作機体のローカル方向に変換
        Vector3 localDir = rightArm.parent.InverseTransformDirection(dir.normalized);
        //ローカル方向から上下左右（角度）を計算
        float pitch = Mathf.Clamp(Mathf.Asin(-localDir.x) * Mathf.Rad2Deg, minPitch, maxPitch);
        float yaw   = Mathf.Atan2(-localDir.y,localDir.z) * Mathf.Rad2Deg;
        float roll  = 0f;

        // X/Y/Z 微調整用で個別にオフセットを設定
        pitch += localRotationOffset.x;
        yaw   += localRotationOffset.y;
        roll  += localRotationOffset.z;
        //初期の姿勢を基準に回転を合成
        Quaternion targetArmRot =
            initialArmRotation *
            Quaternion.Euler(pitch, yaw, roll);
        //自然な追従を表現するための、追従スピード
        rightArm.localRotation = Quaternion.RotateTowards(
            rightArm.localRotation,
            targetArmRot,
            armRotateSpeed * Time.deltaTime * 100f
        );

        //肘関節も同様に補正
        if (forearm != null)
        {
            Quaternion targetElbowRot = initialElbowRotation * Quaternion.Euler(elbowRotationOffset);
            forearm.localRotation = Quaternion.RotateTowards(
                forearm.localRotation,
                targetElbowRot,
                armRotateSpeed * Time.deltaTime * 100f
            );
        }
    }

    void AssistHead()
    {
        if (head == null) return;
        //ターゲット方向
        Vector3 dir = targetEnemy.position - head.position;
        //操作機体のローカル空間へ変換
        Vector3 localDir = head.parent.InverseTransformDirection(dir.normalized);
        //各軸の回転量を算出
        float pitch = Mathf.Asin(-localDir.y) * Mathf.Rad2Deg;
        float yaw   = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;
        float roll  = 0f;
        roll = -roll;
        //回転制限（首の回転へ限界を持たせる）
        pitch = Mathf.Clamp(pitch, headPitchLimit.x, headPitchLimit.y);
        yaw   = Mathf.Clamp(yaw,   headYawLimit.x,  headYawLimit.y);
        roll  = Mathf.Clamp(roll,  headRollLimit.x, headRollLimit.y);
        //初期姿勢とオフセットを合成
        Quaternion targetRot =
            initialHeadRotation *
            Quaternion.Euler(pitch, yaw, roll) *
            Quaternion.Euler(headRotationOffset);
        //追従をなめらかにさせる
        head.localRotation = Quaternion.RotateTowards(
            head.localRotation,
            targetRot,
            headRotateSpeed * Time.deltaTime * 100f
        );
    }

    //ターゲットがいない時、腕を初期の位置と角度に戻す
    void ResetArmOffset()
    {
        Quaternion targetArm = initialArmRotation * Quaternion.Euler(localRotationOffset);
        rightArm.localRotation = Quaternion.RotateTowards(
            rightArm.localRotation,
            targetArm,
            armRotateSpeed * Time.deltaTime * 100f
        );

        if (forearm != null)
        {
            Quaternion targetElbow = initialElbowRotation * Quaternion.Euler(elbowRotationOffset);
            forearm.localRotation = Quaternion.RotateTowards(
                forearm.localRotation,
                targetElbow,
                armRotateSpeed * Time.deltaTime * 100f
            );
        }
    }

    //頭も同様に戻す
    void ResetHeadOffset()
    {
        if (head == null) return;

        Quaternion targetHead =
            initialHeadRotation *
            Quaternion.Euler(headRotationOffset);

        head.localRotation = Quaternion.RotateTowards(
            head.localRotation,
            targetHead,
            headRotateSpeed * Time.deltaTime * 100f
        );
    }

    void ShowReticle(bool on)
    {
        if (assistReticle == null) return;
        //アルファ値を徐々に変化させて表示と非表示をさせる
        Color c = assistReticle.color;
        float targetA = on ? reticleVisibleAlpha : 0f;

        c.a = Mathf.MoveTowards(c.a, targetA, Time.deltaTime * 5f);
        assistReticle.color = c;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, assistRange);
    }
}
