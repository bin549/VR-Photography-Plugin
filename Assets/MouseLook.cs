using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
   public Transform playerBody;
   float xRotation = 0f;



   private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Update()
    {
        float MouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float MouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        this.xRotation -= MouseY;
        this.xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        this.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        this.playerBody.Rotate(Vector3.up * MouseX);
    }

}
