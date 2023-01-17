using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
     public CharacterController controller;
     public float speed = 12f;
        public float gravity = -9.81f;
        public float jump = 1f;
      public Transform groundCheck;
      public float groundDistance = 0.4f;
      public LayerMask groundMask;
      Vector3 velocity;
     private bool isGrounded;
     public float rotateSpeed = 30;
     [SerializeField] private PhotoCapture photoCapture;
     [SerializeField] private float zoomSpeed = 0.3f;
     public Camera playerCamera;
     public float defaultFieldOfView;

     private void Start() {
        defaultFieldOfView = playerCamera.fieldOfView;
    }

    private void Update() {
        if (!photoCapture.cameraUI.activeSelf && !photoCapture.viewingPhoto)
        {
            PlayerMove();
        }
        else if (photoCapture.cameraUI.activeSelf && !photoCapture.viewingPhoto){
            CameraMove();
        }
    }

    private void PlayerMove()
    {
     isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
     if(isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * speed * Time.deltaTime);

                        if (Input.GetKey(KeyCode.LeftArrow))
                            {
                                transform.RotateAround(transform.position, Vector3.up, -rotateSpeed * 5 * Time.deltaTime);
                            }

                            if (Input.GetKey(KeyCode.RightArrow))
                            {
                                transform.RotateAround(transform.position, Vector3.up, rotateSpeed * 5 * Time.deltaTime);
                            }

            if(Input.GetButtonDown("Jump") && isGrounded)
                {
                    velocity.y = Mathf.Sqrt(jump * -2f * gravity);
                }
                velocity.y += gravity * Time.deltaTime;
                        controller.Move(velocity * Time.deltaTime);


    }

    private void CameraMove()
    {
                if (Input.GetKey(KeyCode.A))
                    {
                        transform.RotateAround(transform.position, Vector3.up, -rotateSpeed * 5 * Time.deltaTime);
                    }

                    if (Input.GetKey(KeyCode.D))
                    {
                        transform.RotateAround(transform.position, Vector3.up, rotateSpeed * 5 * Time.deltaTime);
                    }

                    if (Input.GetKey(KeyCode.W) && playerCamera.fieldOfView>=15)
                        {
                            playerCamera.fieldOfView -= zoomSpeed;
                        }

                        if (Input.GetKey(KeyCode.S)  && playerCamera.fieldOfView<=60)
                        {
                            playerCamera.fieldOfView+= zoomSpeed;
                        }

    }


    public void ResetDefaultFieldOfView()
    {
        while (true)
        {
            if (playerCamera.fieldOfView <= defaultFieldOfView)
            {
                playerCamera.fieldOfView += zoomSpeed;
            }
             else {
                playerCamera.fieldOfView = defaultFieldOfView;
                break;
            }
        }
    }
}
