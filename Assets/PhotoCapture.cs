using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoCapture : MonoBehaviour
{
    [SerializeField] private Image photoDisplayArea;
    private Texture2D screenCapture;
    [SerializeField] private GameObject photoFrame;
    private bool viewingPhoto;
    [SerializeField] private GameObject CameraFlash;
    [SerializeField] private float flashTime;
    [SerializeField] private Animator fadingAnimation;
    [SerializeField] private GameObject cameraUI;
    [SerializeField] private AudioSource cameraAudio;

    private void Start()
    {
        screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!viewingPhoto) {
                StartCoroutine(CapturePhoto());
            } else {
                RemovePhoto();
            }
        }
    }

    private IEnumerator CapturePhoto()
    {
        cameraUI.SetActive(false);
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
        viewingPhoto = false;
        photoFrame.SetActive(false);
        cameraUI.SetActive(true);
    }
}
