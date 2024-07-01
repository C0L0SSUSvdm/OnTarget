using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prototypeCar : MonoBehaviour
{

    [Header("----- Camera Settings -----")]
    [Tooltip("This is the Raycast Origin for the Camera")]
    [SerializeField] GameObject RayCastOrigin;

    [Tooltip("Current Euler Angle for Lerping a rotation around the car")]
    [SerializeField] float CameraAngleY = 0;
    [Tooltip("The Speed at which the Camera rates around the car")]
    [SerializeField] float CameraRatationSpeed = 10.0f;

    [Tooltip("Distance Camera maintaines From the Car")]
    [SerializeField] float cameraFollowDistance = 20.0f;
    [Tooltip("Extra Height Added to Camera Position")]
    [SerializeField] float cameraExtraHeight = 10.0f;

    [Tooltip("Current point for Lerping look position")]
    [SerializeField] Vector3 CurrentCameraLookPoint;
    [Tooltip("Distance of the point from the car the Camera locks onto each frame")]
    [SerializeField] float CameraLookOffset = 15.0f;
    [Tooltip("Value to Multiply the Camera Look Offset by when reversing")]
    [Range(1, 2), SerializeField] float CameraReverseLookScalar = 2.0f;

    [Tooltip("Angle Strength to simulate a head tile while turning")]
    [Range(0, 1), SerializeField] float CameraTiltDampener = 0.2f;
    [Tooltip("The Angle of the Current Camera Tilt")]
    [SerializeField] float CameraTiltAngle = 0.0f;

    [Header("----- Car On Start Options -----")]
    [Tooltip("Change Swing Direction, (false = swings to inside), (true = swings to outside)")]
    [SerializeField] bool ToggleCameraSwing = false;
    float SwingDirection = 1;

    [Header("Input Scalars")]
    public float motorPower = 1000;
    public float steerPower = 35; // 50 is a Euler angle
    public float brakePower = 50;
    [Header("RigidBody Fields")]
    [SerializeField] float basemass;
    [SerializeField] float baseDrag;
    [SerializeField] float baseAngularDrag;

    [Header("Wheel Collider Fields")]
    [SerializeField] float mass;
    [SerializeField] float wheelDampingRate;
    [SerializeField] float suspensionDistance;
    [SerializeField] float forceAppPointDistance;
    [Header("Wheel Spring Fields")]
    [SerializeField] float SpringForce;
    [SerializeField] float SpringDamper;
    [SerializeField] float SpringTargetPosition;
    [Header("Foward Friction Curve")]
    [SerializeField] float fwdExtremumSlip;
    [SerializeField] float fwdExtremumValue;
    [SerializeField] float fwdAsymptoteSlip;
    [SerializeField] float fwdAsymptoteValue;
    [SerializeField] float fwdStiffness;
    [Header("Side Friction Curve")]
    [SerializeField] float sideExtremumSlip;
    [SerializeField] float sideExtremumValue;
    [SerializeField] float sideAsymptoteSlip;
    [SerializeField] float sideAsymptoteValue;
    [SerializeField] float sideStiffness;

    [Header("Drag N Drop")]
    [SerializeField] public GameObject centerOfMass;
    [SerializeField] public Rigidbody rigidBody;
    [SerializeField] public GameObject wheelJoint_FrontLeft;
    [SerializeField] public GameObject wheelJoint_Frontright;
    [SerializeField] public GameObject wheelJoint_BackLeft;
    [SerializeField] public GameObject wheelJoint_BackRight;


    private WheelCollider wheel_FL;
    private WheelCollider wheel_FR;
    private WheelCollider wheel_BL;
    private WheelCollider wheel_BR;
    private GameObject wheelMesh_FL;
    private GameObject wheelMesh_FR;
    private GameObject wheelMesh_BL;
    private GameObject wheelMesh_BR;

    void Start()
    {
        gameManager.instance.SetPlayerObejct(this.gameObject);
        SetRigidBodyProperties();
        InitializeWheelReferences();
        SetWheelBasics(wheel_FL);
        SetWheelBasics(wheel_FR);
        SetWheelBasics(wheel_BL);
        SetWheelBasics(wheel_BR);
        SetWheelSpring(wheel_FL);
        SetWheelSpring(wheel_FR);
        SetWheelSpring(wheel_BL);
        SetWheelSpring(wheel_BR);
        SetWheelFriction(wheel_FL);
        SetWheelFriction(wheel_FR);
        SetWheelFriction(wheel_BL);
        SetWheelFriction(wheel_BR);

        rigidBody.centerOfMass = centerOfMass.transform.localPosition;

        if (ToggleCameraSwing)
        {
            SwingDirection = -1;
        }
        else
        {
            SwingDirection = 1;
        }
    }

    void Update()
    {
        SetRigidBodyProperties();
        SetWheelBasics(wheel_FL);
        SetWheelBasics(wheel_FR);
        SetWheelBasics(wheel_BL);
        SetWheelBasics(wheel_BR);
        SetWheelSpring(wheel_FL);
        SetWheelSpring(wheel_FR);
        SetWheelSpring(wheel_BL);
        SetWheelSpring(wheel_BR);
        SetWheelFriction(wheel_FL);
        SetWheelFriction(wheel_FR);
        SetWheelFriction(wheel_BL);
        SetWheelFriction(wheel_BR);


        //Player Inputs, Move to Player Script to Inherit from this Script
        float torque = Input.GetAxis("Vertical") * motorPower;
        wheel_FL.motorTorque = torque;
        wheel_FR.motorTorque = torque;
        wheel_BL.motorTorque = torque;
        wheel_BR.motorTorque = torque;

        float angle = Input.GetAxis("Horizontal") * steerPower;
        wheel_FL.steerAngle = angle;
        wheel_FR.steerAngle = angle;
        wheelJoint_FrontLeft.transform.localRotation = Quaternion.Euler(0, angle, 0);
        wheelJoint_Frontright.transform.localRotation = Quaternion.Euler(0, angle, 0);

        float brake = Input.GetKey(KeyCode.Space) ? brakePower : 0;
        wheel_FL.brakeTorque = brake;
        wheel_FR.brakeTorque = brake;

        wheelMesh_FL.transform.localPosition = new Vector3(2, wheel_FL.suspensionDistance * -0.5f, 0);
        wheelMesh_FR.transform.localPosition = new Vector3(-2, wheel_FR.suspensionDistance * -0.5f, 0);
        wheelMesh_BL.transform.localPosition = new Vector3(0, wheel_BL.suspensionDistance * -0.5f, 0);
        wheelMesh_BR.transform.localPosition = new Vector3(0, wheel_BR.suspensionDistance * -0.5f, 0);

        HUD.Item.UpdateSpeedometer(rigidBody.velocity.magnitude);


    }

    private void LateUpdate()
    {
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        //Vector3 direction = RayCastOrigin.gameObject.transform.position - Camera.main.transform.position;

        float reverseScalar = 1;

        //Step 1: Calculate rotation angle for the cameras local offset position

        float forwardInput = Input.GetAxisRaw("Vertical");
        if (forwardInput >= 0)
        {
            forwardInput = 1;
            //Moves Camera to the outside of the turn at the steer angle
            //CameraAngleY = -wheel_FL.steerAngle;
            CameraAngleY = Mathf.LerpAngle(CameraAngleY, SwingDirection * wheel_FL.steerAngle, Time.deltaTime * CameraRatationSpeed);
        }
        else
        {
            //Calculate the Reverse Camera Angles and Values
            reverseScalar = CameraReverseLookScalar;
            float maxAngle = 180;
            if (Camera.main.transform.localRotation.y < 0)
            {
                maxAngle *= -1;
            }
            CameraAngleY = Mathf.LerpAngle(CameraAngleY, maxAngle, Time.deltaTime * CameraRatationSpeed);
        }
        Vector3 cameraOffset = gameObject.transform.forward * -cameraFollowDistance * reverseScalar;
        Quaternion rotation = Quaternion.Euler(0, CameraAngleY, 0);
        cameraOffset = rotation * cameraOffset;

        //Step 2: Calculate the Camera Target Position
        Vector3 cameraTargetPosition = RayCastOrigin.gameObject.transform.position + cameraOffset;
        cameraTargetPosition.y = cameraTargetPosition.y + cameraExtraHeight;


        //Step 3: Prevent Camera from clipping other objects - Shoots a Ray out the back and reflects it off an object to prevent camera clipping
        Vector3 targetDirection = (cameraTargetPosition - RayCastOrigin.transform.position).normalized;
        float remainingDistance = cameraFollowDistance * reverseScalar;
        RaycastHit hit;
        if (Physics.Raycast(RayCastOrigin.gameObject.transform.position, targetDirection, out hit, remainingDistance))
        {
            remainingDistance -= hit.distance;
            Vector3 refelctionangle = Vector3.Reflect(targetDirection, hit.normal);
            Ray ray = new Ray(hit.point, refelctionangle);
            //Debug.DrawRay(hit.point, ray.direction * remainingDistance, Color.red, 1.0f);
            cameraTargetPosition = ray.GetPoint(remainingDistance);
        }

        //Step 4: Lerp Camera Position - TODO Lerp Around the Car, not through it
        float destinationDistance = Vector3.Distance(Camera.main.transform.position, cameraTargetPosition) * 0.2f;
        Vector3 interprolationPosition = Vector3.Lerp(Camera.main.transform.position, cameraTargetPosition, Time.deltaTime * destinationDistance);
        Camera.main.transform.position = interprolationPosition;

        //Step 5: Lerp Camera Look Point
        float cameraLookDistance = Vector3.Distance(CurrentCameraLookPoint, gameObject.transform.position + (gameObject.transform.forward * forwardInput * CameraLookOffset)); //Phase out?
        CurrentCameraLookPoint = Vector3.Lerp(CurrentCameraLookPoint, gameObject.transform.position + (gameObject.transform.forward * forwardInput * CameraLookOffset), Time.deltaTime * cameraLookDistance);
        Camera.main.transform.LookAt(CurrentCameraLookPoint);
        //Camera.main.transform.LookAt(gameObject.transform.position + (gameObject.transform.forward  * 15)); //Static Camera look Position

        //Step 6: Give the Camera a little bit of a tilt on turns
        CameraTiltAngle = Mathf.LerpAngle(CameraTiltAngle, wheel_FL.steerAngle * CameraTiltDampener, Time.deltaTime * CameraRatationSpeed);
        Camera.main.transform.Rotate(Vector3.back, CameraTiltAngle * forwardInput);
    }

    void InitializeWheelReferences()
    {
        wheel_FL = wheelJoint_FrontLeft.GetComponent<WheelCollider>();
        wheel_FR = wheelJoint_Frontright.GetComponent<WheelCollider>();
        wheel_BL = wheelJoint_BackLeft.GetComponent<WheelCollider>();
        wheel_BR = wheelJoint_BackRight.GetComponent<WheelCollider>();
        wheelMesh_FL = wheelJoint_FrontLeft.transform.GetChild(0).gameObject;
        wheelMesh_FR = wheelJoint_Frontright.transform.GetChild(0).gameObject;
        wheelMesh_BL = wheelJoint_BackLeft.transform.GetChild(0).gameObject;
        wheelMesh_BR = wheelJoint_BackRight.transform.GetChild(0).gameObject;
    }

    void SetRigidBodyProperties()
    {
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;

        rigidBody.mass = basemass;
        rigidBody.drag = baseDrag;
        rigidBody.angularDrag = baseAngularDrag;
    }

    void SetWheelBasics(WheelCollider wheel)
    {
        wheel.mass = mass;
        wheel.wheelDampingRate = wheelDampingRate;
        wheel.suspensionDistance = suspensionDistance;
        wheel.forceAppPointDistance = forceAppPointDistance;

    }

    void SetWheelSpring(WheelCollider wheel)
    {
        wheel.suspensionSpring = new JointSpring
        {
            spring = SpringForce,
            damper = SpringDamper,
            targetPosition = SpringTargetPosition
        };
    }


    void SetWheelFriction(WheelCollider wheel)
    {
        wheel.forwardFriction = new WheelFrictionCurve
        {
            extremumSlip = fwdExtremumSlip,
            extremumValue = fwdExtremumValue,
            asymptoteSlip = fwdAsymptoteSlip,
            asymptoteValue = fwdAsymptoteValue,
            stiffness = fwdStiffness
        };

        wheel.sidewaysFriction = new WheelFrictionCurve
        {
            extremumSlip = sideExtremumSlip,
            extremumValue = sideExtremumValue,
            asymptoteSlip = sideAsymptoteSlip,
            asymptoteValue = sideAsymptoteValue,
            stiffness = sideStiffness
        };
    }
}
