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
    [SerializeField] private WheelCollider FrontLeftColIn;
    [SerializeField] private WheelCollider FrontLeftColOut;

    [SerializeField] private WheelCollider FrontRightColIn;
    [SerializeField] private WheelCollider FrontRightColOut;

    [SerializeField] private WheelCollider RearLeftColIn;
    [SerializeField] private WheelCollider RearLeftColOut;

    [SerializeField] private WheelCollider RearRightColIn;
    [SerializeField] private WheelCollider RearRightColOut;


    [Header("Wheel Transforms")]
    [SerializeField] private Transform FrontLeftTr;
    [SerializeField] private Transform FrontRightTr;
    [SerializeField] private Transform RearLeftTr;
    [SerializeField] private Transform RearRightTr;

    private Rigidbody rigid;

    private bool isGround;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.centerOfMass = new Vector3(0f, 0.25f, -0.1f);
    }

    private Vector3 lastFrame;

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();

        UpdateWheel(FrontRightColIn, FrontRightTr);
        UpdateWheel(FrontLeftColIn, FrontLeftTr);
        UpdateWheel(RearRightColIn, RearRightTr);
        UpdateWheel(RearLeftColIn, RearLeftTr);

        rigid.AddForce(downForce * Mathf.Abs(vertical) * -transform.up);
        //rigid.AddForceAtPosition((500f + downForce * Mathf.Abs(vertical)) * -transform.up, horizontal * transform.right + transform.position);
        //Debug.DrawRay(horizontal * transform.right + transform.position, downForce * Mathf.Abs(vertical) * -transform.up, Color.red);
        //Debug.Log(horizontal * transform.right + transform.position);

        Debug.Log($"{(transform.position - lastFrame).magnitude * 168f:0}km/h");
        lastFrame = transform.position;
    }

    private void HandleMotor()
    {
        float force = motorForce * vertical;
        FrontLeftColIn.motorTorque = force;
        FrontLeftColOut.motorTorque = force;

        FrontRightColIn.motorTorque = force;
        FrontRightColOut.motorTorque = force;

        if (isBraking)
        {
            FrontLeftColIn.brakeTorque = brakeForce;
            FrontLeftColOut.brakeTorque = brakeForce;

            FrontRightColIn.brakeTorque = brakeForce;
            FrontRightColOut.brakeTorque = brakeForce;

            RearLeftColIn.brakeTorque = brakeForce;
            RearLeftColOut.brakeTorque = brakeForce;

            RearRightColIn.brakeTorque = brakeForce;
            RearRightColOut.brakeTorque = brakeForce;
        }
        else
        {
            FrontLeftColIn.brakeTorque = 0f;
            FrontLeftColOut.brakeTorque = 0f;

            FrontRightColIn.brakeTorque = 0f;
            FrontRightColOut.brakeTorque = 0f;

            RearLeftColIn.brakeTorque = 0f;
            RearLeftColOut.brakeTorque = 0f;

            RearRightColIn.brakeTorque = 0f;
            RearRightColOut.brakeTorque = 0f;
        }
    }

    private void HandleSteering()
    {
        float steer = maxSteerAngle * horizontal;
        FrontLeftColIn.steerAngle = steer;
        FrontLeftColOut.steerAngle = steer;

        FrontRightColIn.steerAngle = steer;
        FrontRightColOut.steerAngle = steer;
    }

    private void UpdateWheel(WheelCollider wheelCol, Transform wheelTr)
    {
        wheelCol.GetWorldPose(out _, out Quaternion rot);
        wheelTr.rotation = rot;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            isGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            isGround = false;
        }
    }
}
