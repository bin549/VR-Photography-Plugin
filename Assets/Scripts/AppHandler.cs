using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppHandler : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ScreenshotHandler.TakeScreenshot_Static(500, 500);
        }
    }
}
