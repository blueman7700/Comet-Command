using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private Camera cam;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shotForce;

    private Vector2 mousePos;
    private Mouse mouse;
    private GameController mGameController;

    public void Start()
    {
        mouse = Mouse.current;
        mGameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    public void Fire(InputAction.CallbackContext context) 
    {
        if(context.performed) {

            if (mGameController.PlayerCanShoot())
            {
                mousePos = cam.ScreenToWorldPoint(mouse.position.ReadValue());
                Shoot();
                mGameController.HandlePlayerShot();
            }
        }
    }

    public void Look(InputAction.CallbackContext context) 
    {
        mousePos = cam.ScreenToWorldPoint(mouse.position.ReadValue());
    }

}
