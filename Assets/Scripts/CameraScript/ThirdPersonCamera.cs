using UnityEngine;
using DG.Tweening;

public class ThirdPersonCamera : MonoBehaviour
{
    public static ThirdPersonCamera Instance { get; private set; }
    public float turnSpeed = 4.0f;
    public Transform cameraPositionDistance;
    [SerializeField] float camDistanceMultiplier = 5f;
    private Transform target;
    [SerializeField] private float setCameraDistance;
    //private CharacterData characterDataScript;
    private Transform cameraPosition;
    private float targetDistance;
    public float verticalOffset = 0f;
    public float minTurnAngle = -90.0f;
    public float maxTurnAngle = 0.0f;
    private float rotX;
    [SerializeField] private GameObject playerGameObject;

    private float normalDistance;
    private float desiredDistance;
    [SerializeField] private float camDistanceLerpSpeed = 5f;

    private float normalVerticalOffset;
    private float desiredVerticalOffset;

    public float NormalDistance => normalDistance;

    private bool camLocked = false;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!camLocked)
        {
            ControlCamera();
        }
        
    }

    void Start()
    {


        //turnSpeed = LocalPlayerSettings.Sensitivity;
        //LocalPlayerSettings.OnSensitivityChanged += OnSensitivityChanged;
    }

    public void UnlockedCam() => camLocked = false;

    public void LockedCam() => camLocked = true;

    private void OnSensitivityChanged(float newSensitivity)
    {
        turnSpeed = newSensitivity;
    }

    private void OnDestroy()
    {
        //LocalPlayerSettings.OnSensitivityChanged -= OnSensitivityChanged; 
    }

    public Transform ReturnRelativeCamPos() => transform;

    public void SetCameraTarget(Transform playerTarget)
    {
        target = playerTarget;
        cameraPosition = cameraPositionDistance;
        targetDistance = Vector3.Distance(cameraPosition.position, target.position) * camDistanceMultiplier;

        normalDistance = targetDistance;  
        desiredDistance = targetDistance;
        normalVerticalOffset = verticalOffset;
        desiredVerticalOffset = verticalOffset;
    }

    private void ControlCamera()
    {
        if (target == null || cameraPosition == null) return;

        float y = Input.GetAxis("Mouse X") * turnSpeed;
        rotX += Input.GetAxis("Mouse Y") * turnSpeed;
        rotX = Mathf.Clamp(rotX, minTurnAngle, maxTurnAngle);

        transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y + y, 0);

        float heightOffset = verticalOffset;
        targetDistance = Mathf.Lerp(targetDistance, desiredDistance, camDistanceLerpSpeed * Time.deltaTime);
        verticalOffset = Mathf.Lerp(verticalOffset, desiredVerticalOffset, camDistanceLerpSpeed * Time.deltaTime);
        transform.position = target.position + new Vector3(0, verticalOffset, 0) - (transform.forward * targetDistance);

    }

    public void SetCameraVerticalOffset(float offset) => desiredVerticalOffset = normalVerticalOffset + offset;
    public void ResetCameraVerticalOffset() => desiredVerticalOffset = normalVerticalOffset;

    public void SetCameraDistance(float newDistance) => desiredDistance = newDistance;
    public void ResetCameraDistance() => desiredDistance = normalDistance;

    public void DoFOV(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }
}
