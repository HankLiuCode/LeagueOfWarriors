using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPlayerController : MonoBehaviour
{

    [SerializeField] float speed = 10f;
    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        transform.position += input * speed * Time.deltaTime;
    }
}
