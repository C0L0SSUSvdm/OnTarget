using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCamera : MonoBehaviour
{
    [Header("----- Camera Settings -----")]
    [Tooltip("This is the Raycast Origin for the Camera")]
    [SerializeField] GameObject CameraAnchor;
    //[SerializeField] GameObject RayCastOrigin;
    [SerializeField] baseCar carScript;

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

    private void Start()
    {


        CameraAnchor = transform.parent.gameObject;
        carScript = CameraAnchor.transform.parent.GetComponent<baseCar>();
    }

    private void LateUpdate()
    {
        UpdateCamera();
    }

    private void UpdateCamera()
    {

        SwingDirection = ToggleCameraSwing ? -1 : 1;

        float reverseScalar = 1;

        //Step 1: Calculate rotation angle for the cameras local offset position

        float forwardInput = Input.GetAxisRaw("Vertical");

        if (forwardInput >= 0)
        {
            forwardInput = 1;
            //Moves Camera to the outside of the turn at the steer angle
            //CameraAngleY = -wheel_FL.steerAngle;
            CameraAngleY = Mathf.LerpAngle(CameraAngleY, SwingDirection * carScript.GetSteeringAngle(), Time.deltaTime * CameraRatationSpeed);
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
        Vector3 cameraOffset = CameraAnchor.transform.parent.forward * -cameraFollowDistance * reverseScalar;
        Quaternion rotation = Quaternion.Euler(0, CameraAngleY, 0);
        cameraOffset = rotation * cameraOffset;

        //Step 2: Calculate the Camera Target Position
        Vector3 cameraTargetPosition = CameraAnchor.gameObject.transform.position + cameraOffset;
        cameraTargetPosition.y = cameraTargetPosition.y + cameraExtraHeight;


        //Step 3: Prevent Camera from clipping other objects - Shoots a Ray out the back and reflects it off an object to prevent camera clipping
        Vector3 targetDirection = (cameraTargetPosition - CameraAnchor.transform.position).normalized;
        float remainingDistance = cameraFollowDistance * reverseScalar;
        RaycastHit hit;
        if (Physics.Raycast(CameraAnchor.gameObject.transform.position, targetDirection, out hit, remainingDistance))
        {
            remainingDistance -= hit.distance;
            Vector3 refelctionangle = Vector3.Reflect(targetDirection, hit.normal);
            Ray ray = new Ray(hit.point, refelctionangle);
            //Debug.DrawRay(hit.point, ray.direction * remainingDistance, Color.red, 1.0f);
            cameraTargetPosition = ray.GetPoint(remainingDistance);
        }

        //Step 4: Lerp Camera Position - TODO Lerp Around the Car, not through it
        float destinationDistance = Vector3.Distance(transform.position, cameraTargetPosition) * 0.2f;
        Vector3 interprolationPosition = Vector3.Lerp(transform.position, cameraTargetPosition, Time.deltaTime * destinationDistance);
        transform.position = interprolationPosition;

        //Step 5: Lerp Camera Look Point
        float cameraLookDistance = Vector3.Distance(CurrentCameraLookPoint, CameraAnchor.transform.parent.position + (CameraAnchor.transform.parent.forward * forwardInput * CameraLookOffset)); //Phase out?
        CurrentCameraLookPoint = Vector3.Lerp(CurrentCameraLookPoint, CameraAnchor.transform.parent.position + (CameraAnchor.transform.parent.forward * forwardInput * CameraLookOffset), Time.deltaTime * cameraLookDistance);
        transform.LookAt(CurrentCameraLookPoint);
        //Camera.main.transform.LookAt(gameObject.transform.position + (gameObject.transform.forward  * 15)); //Static Camera look Position

        //Step 6: Give the Camera a little bit of a tilt on turns
        CameraTiltAngle = Mathf.LerpAngle(CameraTiltAngle, carScript.GetSteeringAngle() * CameraTiltDampener, Time.deltaTime * CameraRatationSpeed);
        transform.Rotate(Vector3.back, CameraTiltAngle * forwardInput);
    }
}