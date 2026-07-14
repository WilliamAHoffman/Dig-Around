using System;
using UnityEngine;

public class LinearMovement : MonoBehaviour
{
    [SerializeField] float xSpeed;
    [SerializeField] float ySpeed;
    [SerializeField] Rigidbody2D rg;

    // Update is called once per frame
    void Update()
    {
        rg.linearVelocity = new Vector3(xSpeed,ySpeed);
    }
}
