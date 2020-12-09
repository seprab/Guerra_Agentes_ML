using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody m_Rigidbody;
    private float m_Speed = 30f;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        Invoke("DestroyBullet", 5f);
    }
    void Update()
    {
        Vector3 movement = transform.forward  * m_Speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }
    void DestroyBullet()
    {
        Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        CancelInvoke();
        transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        LeanTween.scale(gameObject, Vector3.zero, 0.3f);
        Destroy(gameObject,1f);
        enabled = false;
    }
}
