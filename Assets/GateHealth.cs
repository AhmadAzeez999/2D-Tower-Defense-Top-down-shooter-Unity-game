using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateHealth : MonoBehaviour
{
    public int maxHealth = 10;
    public float currentHealth;

    private bool isInvincible;

    [SerializeField] GameData gameData;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckObjectives()
    {
        if (currentHealth == maxHealth)
        {
            gameData.AddBoneEarned();
        }

        if (currentHealth >= 8)
        {
            gameData.AddBoneEarned();
        }

        if (currentHealth >= 5)
        {
            gameData.AddBoneEarned();
        }
    }

    public void TakeDamage(int amount)
    {
        if (!isInvincible)
        {
            currentHealth -= amount;

            if (currentHealth > 0)
            {
                BecomeInvincible();
            }
            else
            {
                GameManager.instance.GameOver();
            }
        }
    }

    public void BecomeInvincible()
    {
        isInvincible = true;
        StartCoroutine(Invincibility());
    }

    private IEnumerator Invincibility()
    {
        isInvincible = true;

        yield return new WaitForSeconds(0.5f);
        isInvincible = false;
    }
}
