#pragma warning disable IDE0051
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.ParticleSystem;

public class CarController : MonoBehaviour
{
    private float horizontal; //����
    private float vertical;   //�¿�

    private bool isBraking;  //�극��ũ üũ

    [Header("Car")]
    [SerializeField] private float motorForce;                  //ȸ����
    [SerializeField] private float brakeForce;                  //�극��ũ ��
    [SerializeField] private float downForce;                   //���⿡ ���� �Ʒ��� �޴� ��
    [SerializeField] private float maxSteerAngle;               //�� ���� �ִ� ȸ�� ��
    [SerializeField, Range(0f, 90f)] private float slipAngle;   //�̲����� �Ǻ� ��

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider FrontLeftCol;
    [SerializeField] private WheelCollider FrontRightCol;
    [SerializeField] private WheelCollider RearLeftCol;
    [SerializeField] private WheelCollider RearRightCol;

    [SerializeField] private TrailRenderer leftSkid, rightSkid;
    [SerializeField] private ParticleSystem leftSmoke, rightSmoke;
    private bool isSlipping;

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
    private Vector3 lastFrame;  //���� �ӵ��� �����ϱ� ���� ���� �������� ��ġ

    [Header("Audios")]
    [SerializeField] private AudioSource slipSource;    //�̲����� �Ҹ� �ҽ�
    [SerializeField] private AudioSource engine;        //���� �Ҹ� �ҽ�

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        AudioManager.Instance.PlayAudio(engine, "Idle", 2f, true);
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

        EngineSound();
        if (kilometerPerHour > 15f && (FrontLeftCol.isGrounded || FrontRightCol.isGrounded || RearLeftCol.isGrounded || RearRightCol.isGrounded))
        {
            SlipCheck();
        }
        else
        {
            AudioManager.Instance.StopAudioFadeOut(slipSource, 0.2f);
            SkidMarking(false);
        }
        InstrumentPanel();

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
        needle.localEulerAngles = new Vector3(0f, 0f, 120f - (1.5f * kilometerPerHour));
        speedPanel.text = $"{(int)kilometerPerHour}km/h";
        lastFrame = transform.position;
    }

    private void EngineSound()
    {
        float speed2percentage = kilometerPerHour / 150f;
        engine.pitch = speed2percentage + 1f;
    }

    private void SlipCheck()
    {
        Vector3 forward = transform.forward;
        Vector3 velocity = rigid.velocity;

        float x = forward.x * velocity.x;
        float y = forward.y * velocity.y;
        float z = forward.z * velocity.z;

        float degree = Mathf.Acos((x + y + z) / (forward.magnitude * velocity.magnitude)) * Mathf.Rad2Deg;
        
        if (degree > slipAngle && degree < 180f - slipAngle)
        {
            AudioManager.Instance.PlayAudioFadeIn(slipSource, "Drift", 0.2f, kilometerPerHour / 100f, kilometerPerHour / 150f + 1f, true);
            SkidMarking(true);
        }
        else
        {
            AudioManager.Instance.StopAudioFadeOut(slipSource, 0.2f);
            SkidMarking(false);
        }
    }

    private void SkidMarking(bool isSlipping)
    {
        leftSkid.emitting = RearLeftCol.isGrounded && isSlipping;
        rightSkid.emitting = RearRightCol.isGrounded && isSlipping;

        if (leftSkid.emitting || rightSkid.emitting)
        {
            if (!this.isSlipping)
            {
                if (leftSkid.emitting)
                {
                    leftSmoke.Play();
                }
                else
                {
                    leftSmoke.Stop();
                }

                if (rightSkid.emitting)
                {
                    rightSmoke.Play();
                }
                else
                {
                    rightSmoke.Stop();
                }
                this.isSlipping = true;
            }
        }
        else
        {
            leftSmoke.Stop();
            rightSmoke.Stop();
            this.isSlipping = false;
        }
    }
}
