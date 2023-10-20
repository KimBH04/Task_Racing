#pragma warning disable IDE0051
using TMPro;
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
    [SerializeField, Range(0f, 100f)] private float slipAngle;

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

    [Header("Instrument Panel")]
    [SerializeField] private Transform needle;
    [SerializeField] private TextMeshProUGUI speedPanel;
    private float kilometerPerHour;

    [SerializeField] private GameObject backLights;

    private Rigidbody rigid;
    private Vector3 lastFrame;

    private AudioSource source;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        source = GetComponent<AudioSource>();
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

        InstrumentPanel();
        SlipCheck();

        rigid.AddForce(downForce * Mathf.Abs(vertical) * -transform.up);

        backLights.SetActive(vertical < 0 || isBraking);
    }

    private void HandleMotor()
    {
        float force = motorForce * vertical;
        FrontLeftCol.motorTorque = force;
        FrontRightCol.motorTorque = force;
        RearLeftCol.motorTorque = force;
        RearRightCol.motorTorque = force;

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
        float steer = maxSteerAngle * horizontal;
        FrontLeftCol.steerAngle = steer;
        FrontRightCol.steerAngle = steer;
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

    private void InstrumentPanel()
    {
        kilometerPerHour = (transform.position - lastFrame).magnitude * 168f;
        needle.localEulerAngles = new Vector3(0f, 0f, 120f - (1.2f * kilometerPerHour));
        speedPanel.text = $"{(int)kilometerPerHour}km/h";
        lastFrame = transform.position;
    }

    private void SlipCheck()
    {
        float x1;
        float y1;

        if (vertical > 0f)
        {
            x1 = transform.forward.x;
            y1 = transform.forward.z;
        }
        else if (vertical < 0f)
        {
            x1 = -transform.forward.x;
            y1 = -transform.forward.z;
        }
        else
        {
            source.Stop();
            return;
        }

        float x2 = rigid.velocity.x;
        float y2 = rigid.velocity.z;

        float degree1 = Mathf.Atan2(y1, x1) * Mathf.Rad2Deg;
        float degree2 = Mathf.Atan2(y2, x2) * Mathf.Rad2Deg;
        float degree = degree1 - degree2;

        if (degree > slipAngle || degree < -slipAngle && vertical != 0f)
        {
            AudioManager.Instance.PlayAudio(source, "Drift");
            Debug.Log("Drift");
        }
        else
        {
            source.Stop();
        }
    }
}
