using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion_Behaviour : MonoBehaviour
{
    void FixedUpdate()
    {
        Destroy(gameObject, 1.5f);
    }
}
