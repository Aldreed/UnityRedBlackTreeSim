using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMove : MonoBehaviour
{

    private Vector3 startPos;
    private Vector3 resetPos;
    private Vector3 delta;
    private bool drag;
    private bool canDrag = true;

    public float ZoomChange;
    public float SmoothChange;
    public float MinZoom, MaxZoom;

    public Transform treeRoot;

    private Camera cam;


    private void Awake()
    {
        startPos = new Vector3(0, 0, -10);
        cam = GetComponent<Camera>();
    }


    // Start is called before the first frame update
    void Start()
    {

    }


    private void Update()
    {
        if (Input.mouseScrollDelta.y > 0) cam.orthographicSize -= ZoomChange * Time.deltaTime * SmoothChange;
        if (Input.mouseScrollDelta.y < 0) cam.orthographicSize += ZoomChange * Time.deltaTime * SmoothChange;

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, MinZoom, MaxZoom);
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButton(0) && canDrag)
        {
            delta = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (drag == false)
            {
                drag = true;
                startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            drag = false;
        }

        if (drag)
        {
            Camera.main.transform.position = startPos - delta;
        }

        if (Input.GetMouseButton(1))
        {
            //this.transform.parent.position = new Vector3(0, 0, -10);
            this.transform.position = new Vector3(0, 0, -10);
        }
    }

    public void stopDrag()
    {
        canDrag = false;
        Debug.Log("CantDrag");
    }

    public void startDrag()
    {
        canDrag = true;
        Debug.Log("CanDrag");
    }

}
