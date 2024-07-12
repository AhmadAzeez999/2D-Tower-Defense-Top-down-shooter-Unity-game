using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyStats : MonoBehaviour
{

    public float health;
    public float maxHealth = 20;
    public FloatingHealthBar healthBar;

    public GameObject floatingPoints;

    public int deathMoneyAmount = 20;

    private void Awake()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
    }

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        healthBar.UpdateHealthBar(health, maxHealth);

        healthBar.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Kill()
    {
        GameObject points = Instantiate(floatingPoints, transform.position, Quaternion.identity);
        points.transform.GetChild(0).GetComponent<TextMeshPro>().text = "$" + deathMoneyAmount;
        GameManager.Instance.AddCoins(deathMoneyAmount);
        GameManager.Instance.IncreaseCombo();
        Destroy(gameObject);
    }
   
    public void TakeDamage(float damage)
    {
        healthBar.gameObject.SetActive(true);
        GameObject points = Instantiate(floatingPoints, transform.position, Quaternion.identity);
        points.transform.GetChild(0).GetComponent<TextMeshPro>().text = "$" + (int)damage;
        GameManager.Instance.AddCoins((int)damage);

        health = health - damage;
        healthBar.UpdateHealthBar(health, maxHealth);

        if (health <= 0)
        {
            Kill();
        }
    }
}
