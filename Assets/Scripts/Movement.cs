using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float zoomSpeed = 10.0f;
    public float smoothSpeed = 20.0f;

    public float moveSpeed = .05f;

    public float minOrtho = 1.0f;
    public float maxOrtho = 200.0f;

    public float targetOrtho;

    // Use this for initialization
    void Start ()
    {
        targetOrtho = Camera.main.orthographicSize;
    }

    private void LateUpdate()
    {
        CheckZoom(Input.GetAxis("Mouse ScrollWheel"));
        CheckWASD();
    }

    void CheckZoom(float scroll)
    {
        if (Math.Abs(scroll) > 0.0f)
        {
            targetOrtho -= scroll * zoomSpeed;
            targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho);
        }

        Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, 
            targetOrtho, smoothSpeed * Time.deltaTime);
    }

    void CheckWASD()
    {
        var direction = new Vector3();

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            direction += new Vector3(0, 1);
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            direction += new Vector3(-1, 0);
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            direction += new Vector3(0, -1);
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            direction += new Vector3(1, 0);

        transform.Translate(direction * Camera.main.orthographicSize * moveSpeed, Space.World);
    }
}
