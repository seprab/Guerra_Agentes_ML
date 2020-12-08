using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Train : MonoBehaviour
{
    private float speed { get; set; }
    private float movingInterval { get; set; }
    private Collider obj_collider { get; set; }
    private Vector2 scenarioBounds_X { get; set; }
    private Vector2 scenarioBounds_Y { get; set; }
    private Vector2 scenarioBounds_Z { get; set; }


    private void Awake()
    {
        obj_collider = GetComponent<Collider>();
        obj_collider.enabled = false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_speed"></param>
    /// <param name="_movingInterval">How long to wait before moving to another position</param>
    /// <param name="_scenarioBounds_X"></param>
    /// <param name="_scenarioBounds_Y"></param>
    /// <param name="_scenarioBounds_Z"></param>
    public void ConfigureRandom(float _speed, float _movingInterval, Vector2 _scenarioBounds_X, Vector2 _scenarioBounds_Y, Vector2 _scenarioBounds_Z)
    {
        speed = _speed;
        movingInterval = _movingInterval;
        scenarioBounds_X = _scenarioBounds_X;
        scenarioBounds_Y = _scenarioBounds_Y;
        scenarioBounds_Z = _scenarioBounds_Z;
        transform.localPosition = RandomPosition();
        obj_collider.enabled = true;
        StartCoroutine(RandomMovementLoop());
    }
    public void ConfigureRoute(float _speed, Vector3 startPoint, Vector3 finalPoint)
    {
        speed = _speed;
        transform.localPosition = startPoint;
        StartCoroutine(MovementRoute(finalPoint));
    }
    public void ConfigureSpotPoint(Vector3 startPoint)
    {
        transform.localPosition = startPoint;
    }
    private void MoveToPosition(Vector3 newPosition)
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, Time.deltaTime * speed);
    }
    private Vector3 RandomPosition()
    {
        float x = UnityEngine.Random.Range(scenarioBounds_X.x, scenarioBounds_X.y);
        float y = UnityEngine.Random.Range(scenarioBounds_Y.x, scenarioBounds_Y.y);
        float z = UnityEngine.Random.Range(scenarioBounds_Z.x, scenarioBounds_Z.y);
        return new Vector3(x, y, z);
    }
    private IEnumerator MovementRoute(Vector3 finalPoint)
    {
        while (Vector3.Distance(transform.localPosition, finalPoint) > 0.5f)
        {
            MoveToPosition(finalPoint);
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator RandomMovementLoop()
    {
        Vector3 newPosition = Vector3.zero;
        while(true)
        {
            newPosition = RandomPosition();
            while (Vector3.Distance(transform.position, newPosition) > 5)
            {
                MoveToPosition(newPosition);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(movingInterval);
        }
        yield return null;
    }
}
