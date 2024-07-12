using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEnabler : MonoBehaviour
{
    [SerializeField] GameObject objectToEnable;

    public void EnableObject()
    {
        objectToEnable.SetActive(true);
    }
}
