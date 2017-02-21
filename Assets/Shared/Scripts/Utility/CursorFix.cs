using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorFix : MonoBehaviour
{
    private void Update()
    {
        //Alt-tabbing fix (put is somewhere else?)
        if (Cursor.lockState == CursorLockMode.None)
        {
            if (Time.timeScale == 0.0f)
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
