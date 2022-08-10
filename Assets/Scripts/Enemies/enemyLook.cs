using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyLook : MonoBehaviour
{

    public Transform player;

    float speed = 7.0f;

    public void LookAtPlayer(){

        Vector3 direction = player.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, speed * Time.deltaTime);
    }

}
