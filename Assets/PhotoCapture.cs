using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoCapture : MonoBehaviour
{
    [SerializeField] private Image photoDisplayArea;
    private Texture2D screenCapture;
    [SerializeField] private GameObject photoFrame;
    public bool viewingPhoto;
    [SerializeField] private GameObject CameraFlash;
    [SerializeField] private float flashTime;
    [SerializeField] private Animator fadingAnimation;
    public GameObject cameraUI;
    [SerializeField] private AudioSource cameraAudio;
    [SerializeField] private PlayerMovement playerMovement;

    private void Start()
    {
        screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && !viewingPhoto)
        {
            cameraUI.SetActive(!cameraUI.activeSelf);
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (cameraUI.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                cameraUI.SetActive(false);
                if (!viewingPhoto)
                {
                    StartCoroutine(CapturePhoto());
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            playerMovement.ResetDefaultFieldOfView();
            if (viewingPhoto)
            {
                RemovePhoto();
            }
        }
    }

    private IEnumerator CapturePhoto()
    {
        viewingPhoto = true;
        yield return new WaitForEndOfFrame();
        Rect regionToRead = new Rect(0, 0, Screen.width, Screen.height);
        screenCapture.ReadPixels(regionToRead, 0, 0, false);
        screenCapture.Apply();
        ShowPhoto();
    }

    private void ShowPhoto()
    {
        Sprite photoSprite = Sprite.Create(screenCapture, new Rect(0.0f, 0.0f, screenCapture.width, screenCapture.height), new Vector2(0.5f, 0.5f), 100.0f);
        photoDisplayArea.sprite = photoSprite;
        photoFrame.SetActive(true);
        StartCoroutine(CameraFlashEffect());
        fadingAnimation.Play("PhotoFade");
    }

    private IEnumerator CameraFlashEffect()
    {
        cameraAudio.Play();
        CameraFlash.SetActive(true);
        yield return new WaitForSeconds(flashTime);
        CameraFlash.SetActive(false);
    }

    private void RemovePhoto()
    {
        Cursor.lockState = CursorLockMode.Locked;
        viewingPhoto = false;
        photoFrame.SetActive(false);
        cameraUI.SetActive(false);
    }
}
