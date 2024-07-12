using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableScript : MonoBehaviour
{
    GameObject uiDisplay;

    public int value = 1;

    private void Start()
    {
        uiDisplay = GameObject.FindGameObjectWithTag("BoneUI");
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            GameManager.instance.AddBones(value);

            if (uiDisplay)
                uiDisplay.transform.GetChild(0).gameObject.SetActive(true);

            Destroy(gameObject);
        }
    }
}
