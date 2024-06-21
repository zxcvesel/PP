using System;
using UnityEngine;


public class CameraController : MonoBehaviour
{

    private void Start()
    {
        this.target = base.transform.position;
    }


    public void Move(Transform pointZero)
    {
        this.target = pointZero.position;
    }


    private void Update()
    {
        if (this.target != base.transform.position)
        {
            base.transform.position = Vector3.MoveTowards(base.transform.position, this.target, this.speed * Time.deltaTime);
        }
    }


    public void ResetPosition()
    {
        this.target = new Vector3(0f, 0f, 15f);
    }

    private float speed = 50f;

    private Vector3 target;
}