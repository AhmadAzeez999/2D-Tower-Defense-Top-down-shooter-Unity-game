using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComboUIScript : MonoBehaviour
{
    Animator animator;

    [SerializeField] TMP_Text moneyWonDisplayUI;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ComboTimeOut(int moneyWon)
    {
        GameManager.instance.AddCoins(moneyWon);
        if (animator)
            animator.Play("Money Won");
        moneyWonDisplayUI.text = "+$" + moneyWon;

    }
}
