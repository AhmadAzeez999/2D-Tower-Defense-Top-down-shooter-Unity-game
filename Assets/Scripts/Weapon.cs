using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireForce = 20f;
    public float cooldownDuration = 2f;

    float secondarycooldownTimer = 0f;
    public float secondaryCooldownDuration = 5f;

    public Rigidbody2D rb;

    private float cooldownTimer = 0f;

    Vector3 mousePosition;

    // Animation States
    const string GUN_IDLE = "Gun_idle";
    const string GUN_FIRE = "Gun_fire";

    [SerializeField] AnimationStateChanger animationChange;

    // Camera shake
    [Header("Shake")]
    [SerializeField] float intensity = 2f;
    [SerializeField] float shakeTime = 0.1f;

    AudioManager audioManager;

    PlayerHealth health;
    [SerializeField] SpriteRenderer sprite;

    [SerializeField] Slider secondaryWeaponLoadbar;

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        health = GetComponentInParent<PlayerHealth>();

        secondaryWeaponLoadbar.gameObject.SetActive(false);
        secondaryWeaponLoadbar.maxValue = secondaryCooldownDuration;
    }

    private void Update()
    {
        Debug.Log(Time.timeScale);
        Debug.Log(GameManager.Instance.isChatting);

        if (Time.timeScale == 0 || GameManager.Instance.isChatting)
            return;

        if (health.tempDead)
        {
            sprite.enabled = false;
            return;
        }

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction = mousePosition - transform.position;
        transform.right = direction;

        Vector2 scale = transform.localScale;
        if (direction.x < 0)
        {
            scale = new Vector2(-1f, -1f);
        }
        else if (direction.x > 0)
        {
            scale = new Vector2(1f, 1f);
        }

        transform.localScale = scale;

        if (cooldownTimer > 0) // To stop it from changing the cooldownTimer every frame
            cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0)
        {
            if (Input.GetMouseButton(0)) // When left mouse button is pressed
            {
                Fire();
            }
        }

        if (secondarycooldownTimer > 0)
        {
            secondarycooldownTimer -= Time.deltaTime;
            secondaryWeaponLoadbar.gameObject.SetActive(true);
            secondaryWeaponLoadbar.value = secondarycooldownTimer;
        }
        else
        {
            secondaryWeaponLoadbar.gameObject.SetActive(false);
        }

        if (secondarycooldownTimer <= 0)
        {
            if (Input.GetMouseButton(1)) // When right mouse button is pressed
            {
                SecondaryFire();
            }
        }

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void Fire()
    {
        cooldownTimer = cooldownDuration;

        audioManager.PlaySFX(audioManager.gunShot);
        CinemachineShake.Instance.ShakeCamera(intensity, shakeTime);
        animationChange.ChangeAnimationState(GUN_FIRE);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody2D>().AddForce(firePoint.right * fireForce, ForceMode2D.Impulse);
    }

    void SecondaryFire()
    {
        secondarycooldownTimer = secondaryCooldownDuration;

        audioManager.PlaySFX(audioManager.gunShot);
        CinemachineShake.Instance.ShakeCamera(intensity * 4, shakeTime);
        animationChange.ChangeAnimationState(GUN_FIRE);

        Vector2 offset = new Vector2(0.2f, 0.2f);
        Vector2 offset2 = new Vector2(0.3f, 0.3f);
        Vector2 shotDir = firePoint.right;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody2D>().AddForce(shotDir * fireForce, ForceMode2D.Impulse);
        GameObject bullet2 = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet2.GetComponent<Rigidbody2D>().AddForce((shotDir + offset) * fireForce, ForceMode2D.Impulse);
        GameObject bullet3 = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet3.GetComponent<Rigidbody2D>().AddForce((shotDir - offset) * fireForce, ForceMode2D.Impulse);
        GameObject bullet4 = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet4.GetComponent<Rigidbody2D>().AddForce((shotDir - offset2) * fireForce, ForceMode2D.Impulse);
        GameObject bullet5 = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet5.GetComponent<Rigidbody2D>().AddForce((shotDir - offset2) * fireForce, ForceMode2D.Impulse);
    }
}
