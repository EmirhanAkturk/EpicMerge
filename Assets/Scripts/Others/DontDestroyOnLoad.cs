using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    [SerializeField] private bool hide = false;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(!hide);
    }
}
