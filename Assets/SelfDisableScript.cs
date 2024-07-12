using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDisableScript : MonoBehaviour
{
    public void DisableSelf()
    {
        gameObject.SetActive(false);
    }
}
