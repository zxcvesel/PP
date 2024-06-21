using System;
using UnityEngine;


public class CubeOfMeat : MonoBehaviour
{

    private void Update()
    {
        if (this.player != null)
        {
            base.transform.RotateAround(this.player.transform.position, this.player.transform.forward, 100f * Time.deltaTime);
        }
    }


    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    private GameObject player;
}