using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MissileBehaviour : MonoBehaviour
{

    [SerializeField] private double radius;
    [SerializeField] private GameObject explosionAnim;
    [SerializeField] private float speed;

    private Vector2 target;
    private Mouse mouse;
    private Rigidbody2D rb;
    private AudioSource fx;

    private void Start()
    {
        mouse = Mouse.current;
        target = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
        rb = this.GetComponent<Rigidbody2D>();
        fx = GetComponent<AudioSource>();
        Vector2 lookDir = target - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        rb.rotation = angle - 90f;
        fx.Play();
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if ((Vector2)transform.position == target) 
        {
            Explode();
        }
    }

    private void Explode() 
    {
        Instantiate(explosionAnim, this.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
