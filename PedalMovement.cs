using UnityEngine;
using UnityEngine.InputSystem;

public class PedalMovement : MonoBehaviour
{
    [Header("References")]
    public Transform playerBody;

    [Header("Input Actions")]
    public InputAction accelAction;
    public InputAction brakeAction;
    public InputAction leftStick;
    public InputAction rightStick;

    [Header("Move Settings")]
    public float baseSpeed = 5f;      
    public float maxForwardSpeed = 12f;
    public float backDashSpeed = 10f;
    public float sideSpeed = 6f;
    public float acceleration = 6f;

    [Header("Pedal Settings")]
    public float accelDeadZone = 0.5f;
    public float brakeDeadZone = -0.7f;

    [Header("Blend Settings")]
    public float sideBlendStrength = 1.0f;
    public float backDashSideRate = 0.6f;   // 後退時の横ブレンド量

    private float currentForward;
    private float currentBackward;

    void OnEnable()
    {
        accelAction.Enable();
        brakeAction.Enable();
        leftStick.Enable();
        rightStick.Enable();
    }

    void OnDisable()
    {
        accelAction.Disable();
        brakeAction.Disable();
        leftStick.Disable();
        rightStick.Disable();
    }

    void Update()
    {
        float accelValue = accelAction.ReadValue<float>();
        float brakeValue = brakeAction.ReadValue<float>();
        Vector2 ls = leftStick.ReadValue<Vector2>();
        Vector2 rs = rightStick.ReadValue<Vector2>();

        Vector2 blend = (ls + rs * 0.5f) * sideBlendStrength;

        // 前進
        float smoothForward = Mathf.Clamp01((accelValue - accelDeadZone) / (1 - accelDeadZone));
        currentForward = Mathf.MoveTowards(currentForward, smoothForward, Time.deltaTime * acceleration);
        Vector3 forwardMove = playerBody.forward * Mathf.Lerp(baseSpeed, maxForwardSpeed, currentForward);

        // 後退
        float smoothBrake = Mathf.Clamp01((brakeValue - brakeDeadZone) / (1 - brakeDeadZone));
        currentBackward = Mathf.MoveTowards(currentBackward, smoothBrake, Time.deltaTime * acceleration);
        Vector3 backwardMove = (-playerBody.forward * backDashSpeed * currentBackward)
                             + (playerBody.right * blend.x * backDashSideRate);

        // 横移動
        Vector3 horizontal = playerBody.right * blend.x * sideSpeed;

        // 移動合成
        Vector3 moveVec = forwardMove + horizontal + backwardMove;
        playerBody.position += moveVec * Time.deltaTime;
    }
}

