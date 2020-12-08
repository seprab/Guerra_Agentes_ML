using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform Enemigo;
    void Update()
    {
        float dot = Vector3.Dot(transform.forward, (Enemigo.position - transform.position).normalized);
        Debug.Log(dot);
    }
}
