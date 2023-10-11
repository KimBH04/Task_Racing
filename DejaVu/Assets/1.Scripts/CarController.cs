#pragma warning disable IDE0051
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    private bool isBraking;

    [Header("Car")]
    [SerializeField] private float motorForce;
    [SerializeField] private float brakeForce;
    [SerializeField] private float downForce;
    [SerializeField] private float maxSteerAngle;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider FrontLeftCol;
    [SerializeField] private WheelCollider FrontRightCol;
    [SerializeField] private WheelCollider RearLeftCol;
    [SerializeField] private WheelCollider RearRightCol;

    [Header("Wheel Transforms")]
    [SerializeField] private Transform FrontLeftTr;
    [SerializeField] private Transform FrontRightTr;
    [SerializeField] private Transform RearLeftTr;
    [SerializeField] private Transform RearRightTr;

    private Rigidbody rigid;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();

        UpdateWheel(FrontRightCol, FrontRightTr);
        UpdateWheel(FrontLeftCol, FrontLeftTr);
        UpdateWheel(RearRightCol, RearRightTr);
        UpdateWheel(RearLeftCol, RearLeftTr);

        rigid.AddForce(new Vector3(0f, downForce, 0f));
    }

    private void HandleMotor()
    {
        FrontLeftCol.motorTorque = motorForce * vertical;
        FrontRightCol.motorTorque = motorForce * vertical;

        if (isBraking)
        {
            FrontLeftCol.brakeTorque = brakeForce;
            FrontRightCol.brakeTorque = brakeForce;
            RearLeftCol.brakeTorque = brakeForce;
            RearRightCol.brakeTorque = brakeForce;
        }
        else
        {
            FrontLeftCol.brakeTorque = 0f;
            FrontRightCol.brakeTorque = 0f;
            RearLeftCol.brakeTorque = 0f;
            RearRightCol.brakeTorque = 0f;
        }
    }

    private void HandleSteering()
    {
        FrontLeftCol.steerAngle = maxSteerAngle * horizontal;
        FrontRightCol.steerAngle = maxSteerAngle * horizontal;
    }

    private void UpdateWheel(WheelCollider wheelCol, Transform wheelTr)
    {
        wheelCol.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTr.SetPositionAndRotation(pos, rot);
    }

    #region INPUTS
    private void OnDirection(InputValue value)
    {
        Vector2 dir = value.Get<Vector2>();
        horizontal = dir.x;
        vertical = dir.y;
    }

    private void GetInput()
    {
        isBraking = Input.GetKey(KeyCode.Space);
    }
    #endregion
}
