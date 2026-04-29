using UnityEngine;
using UnityEngine.InputSystem;

public class PedalMovement : MonoBehaviour
{
    [Header("References")]
    public Transform playerBody;

    [Header("Input Actions")]
    public InputAction accelAction;
    public InputAction brakeAction;
   
    [Header("Move Settings")]
    public float baseSpeed = 0.1f;  //アイドリングスピード（完全停止によるバグ防止）  
    public float maxForwardSpeed = 12f;
    public float backDashSpeed = 10f;
    public float acceleration = 6f;   //最高速までの加速度

    [Header("Pedal Settings")]
    public float accelDeadZone = 0.5f;  //アクセルペダルの遊びを設定
    public float brakeDeadZone = -0.7f;  

    private float currentForward;
    private float currentBackward;

    void OnEnable()
    {
        accelAction.Enable();
        brakeAction.Enable();
    }

    void OnDisable()
    {
        accelAction.Disable();
        brakeAction.Disable();
    }

    void Update()
    {
        float accelValue = accelAction.ReadValue<float>();
        float brakeValue = brakeAction.ReadValue<float>();

        //前進
        //デッドゾーンを超えた分のみで0~1へ設定（踏み加減に応じた加速）
        float smoothForward = Mathf.Clamp01((accelValue - accelDeadZone) / (1 - accelDeadZone));
        //慣性表現（急加速を防止）
        currentForward = Mathf.MoveTowards(currentForward, smoothForward, Time.deltaTime * acceleration);
        //basespeedを維持しつつ最大速度まで加速
        Vector3 forwardMove = playerBody.forward * Mathf.Lerp(baseSpeed, maxForwardSpeed, currentForward);

        //後退
        //ブレーキ入力（後退表現）を前進と同様に設定
        float smoothBrake = Mathf.Clamp01((brakeValue - brakeDeadZone) / (1 - brakeDeadZone));
        //前進と同様に慣性を付与
        currentBackward = Mathf.MoveTowards(currentBackward, smoothBrake, Time.deltaTime * acceleration);
        //後ろ向きに移動（後退動作）
        Vector3 backwardMove = -playerBody.forward * backDashSpeed * currentBackward;

        //合成（前進・後退を合成）
        Vector3 moveVec = forwardMove + backwardMove;
        //フレーム依存を回避
        playerBody.position += moveVec * Time.deltaTime;
    }
}

