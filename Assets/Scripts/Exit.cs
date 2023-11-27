using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
#if !UNITY_STANDALONE
    private void Awake()
    {
        gameObject.SetActive(false);
    }
#endif

    public void Quit()
    {
        Application.Quit();
    }
}
