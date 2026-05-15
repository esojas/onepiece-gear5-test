using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDrawingScript : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private float camSpeed;
    [SerializeField] private float camOffsetRight;

    [SerializeField] private Transform cam;
    [SerializeField] private GameObject crossHair; // Need this to remove the crosshair during the drawing mode
    [SerializeField] private GameObject linePrefab;

    Line activeLine;

    public event Action IsDrawing, CancelledDrawing;

    private bool isDrawingMode = false;
    //private bool isDrawing = false;

    private void OnEnable()
    {
        playerInput.OnSwitchPlayerToDrawingMode += DrawingMode;
        playerInput.OnDrawingPressed += StartDrawing;
        playerInput.OnDrawingReleased += StopDrawing;
        playerInput.OnSwitchDrawingToPlayerMode += StopDrawingMode;
    }

    private void OnDisable()
    {
        playerInput.OnSwitchPlayerToDrawingMode -= DrawingMode;
        playerInput.OnDrawingPressed -= StartDrawing;
        playerInput.OnDrawingReleased -= StopDrawing;
        playerInput.OnSwitchDrawingToPlayerMode -= StopDrawingMode;
    }

    private void StopDrawingMode()
    {
        isDrawingMode = false;
        crossHair.SetActive(true);
        ThirdPersonCamera.Instance.UnlockedCam();
        CancelledDrawing?.Invoke();
    }

    private void DrawingMode()
    {
        isDrawingMode = true;
        crossHair.SetActive(false);
        IsDrawing?.Invoke();
        ThirdPersonCamera.Instance.LockedCam();
    }

    private void MoveCamera()
    {
        float speed = camSpeed * Time.deltaTime;

        Vector3 targetPosition = transform.position + transform.forward * 5f + transform.right * camOffsetRight;

        Vector3 lookTarget = transform.position + transform.right * camOffsetRight;
        Vector3 direction = lookTarget - cam.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);


        cam.position = Vector3.Lerp(cam.position, targetPosition, speed);
        cam.rotation = Quaternion.Slerp(cam.rotation, targetRotation, speed);
    }

    private void StartDrawing()
    {
        GameObject lineGO = Instantiate(linePrefab);
        activeLine = lineGO.GetComponent<Line>();
    }

    private void StopDrawing()
    {
        activeLine = null;
    }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDrawingMode)
        {
            MoveCamera();
        }

        if (activeLine != null)
        {
            Vector3 screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(screenPos);
            activeLine.UpdateLine(mousePos);
        }
    }
}
