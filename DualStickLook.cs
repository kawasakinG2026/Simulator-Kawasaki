using UnityEngine;
using UnityEngine.InputSystem;

public class DualStickLook : MonoBehaviour
{
    [Header("Rotation Speeds")]
    public float pitchSpeed = 90f; // 前後傾き（ピッチ）
    public float yawSpeed   = 90f; // 左右旋回（ヨー）
    public float rollSpeed  = 90f; // 時計・反時計回りに回転（ロール）

    [Header("Movement")]
    public float slideSpeed = 3f; // 横スライド速度

    [Header("Input Actions")]
    [SerializeField] private InputAction leftStick;  // Vector2
    [SerializeField] private InputAction rightStick; // Vector2

    private void OnEnable()
    {
        leftStick.Enable();
        rightStick.Enable();
    }

    private void OnDisable()
    {
        leftStick.Disable();
        rightStick.Disable();
    }

    void Update()
    {
        Vector2 left  = leftStick.ReadValue<Vector2>();
        Vector2 right = rightStick.ReadValue<Vector2>();

        // ===== 回転判定 =====
        //誤入力による前傾を防ぐために、片方のスティックのみ入力されているかも判定
        bool onlyLeft  = Mathf.Abs(left.y)  > 0.01f && Mathf.Abs(right.y) < 0.01f;
        bool onlyRight = Mathf.Abs(right.y) > 0.01f && Mathf.Abs(left.y)  < 0.01f;

        // ピッチ（前傾）
        float pitchInput = 0f;
        //両スティックを同時に前後へ倒した場合のみ発生させる
        if (!onlyLeft && !onlyRight) // 両方倒した場合のみ前傾
        {
            pitchInput = (left.y + right.y) * 0.5f;
        }

        // ヨー（左右旋回）
        //左右スティックの前後入力の差分から旋回方向を計算
        float yawInput = (right.y - left.y) * 0.5f;

        // ロール（左右の横入力の差分から旋回方向を計算）
        float rollInput = (right.x - left.x) * 0.5f;

        // ===== 回転適用 (+ - -) =====
        //各入力に速度をかけて回転
        float pitchDelta =  pitchInput * pitchSpeed * Time.deltaTime; 
        float yawDelta   = -yawInput   * yawSpeed   * Time.deltaTime; 
        float rollDelta  = -rollInput  * rollSpeed  * Time.deltaTime;
        //ローカル座標（操作する機体基準）で回転
        transform.Rotate(pitchDelta, yawDelta, rollDelta, Space.Self);

        // ===== 横スライド（ベクトル合成） =====
        //両スティックの入力を合成して横移動へ変換
        Vector2 combined = left + right; 
        //-1~1に制限しつつ挙動を滑らかにする
        float horizontalBlend = Mathf.Clamp(combined.x * 0.5f, -1f, 1f);
        //ローカル単位で左右へストレイフさせる
        transform.Translate(Vector3.right * horizontalBlend * slideSpeed * Time.deltaTime, Space.Self);
    }
}
