using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static event Action OnPlayerFire;
    public static event Action<bool> OnPlayerPause;

    public int MissileSpeed { get; set; } = 5;

    [SerializeField] private Camera cam;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float missileSpeed;
    [SerializeField] private GameObject explosionAnim;

    private GameController mGameController;
    private bool pause = false;

    public void Start()
    {
        GameController.OnFireAllowed += Shoot;
        
    }

    private void OnDestroy()
    {
        GameController.OnFireAllowed -= Shoot;
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }

    private void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    public void UpgradeMissiles()
    {
        MissileSpeed++;
    }

    public void Fire(InputAction.CallbackContext context) 
    {
        if(context.performed && !pause) {
            OnPlayerFire?.Invoke();
        }
    }

    public void Pause(InputAction.CallbackContext context)
    {
        pause = !pause;
        OnPlayerPause?.Invoke(pause);
    }
}
