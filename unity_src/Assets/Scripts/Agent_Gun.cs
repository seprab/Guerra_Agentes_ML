using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public class Agent_Gun : Agent
{
    private Transform target;
    private Rigidbody m_Rigidbody;
    private bool ShotAvaliable = true;

    [SerializeField] private string enemy_tag;
    [SerializeField] private Gun_Train_Scenario controller;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private bool trainingMode;
    [SerializeField] private float m_TurnSpeed = 180f;
    [SerializeField] private float timeBetweenShots = 1f;

    public override void Initialize()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        GetComponent<RayPerceptionSensorComponent3D>().DetectableTags[0] = enemy_tag;
        if (!trainingMode) MaxStep = 0; //El valor de 0 significa infinito
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        if (target == null)
        {
            sensor.AddObservation(new float[4]);
            return;
        }

        // 3 observations
        Vector3 toEnemy = target.localPosition - transform.localPosition;
        sensor.AddObservation(toEnemy.normalized);

        // 1 observation
        float dot = Vector3.Dot(transform.forward, (target.localPosition - transform.localPosition).normalized); //1 if pointing forward target,  -1 if facing oppsitve
        sensor.AddObservation(dot);

        // 1 observation
        sensor.AddObservation(ShotAvaliable);
    }
    public override void OnEpisodeBegin()
    {
        m_Rigidbody.angularVelocity = Vector3.zero;
        if (trainingMode)
        {
            ShotAvaliable = true;          
            transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(-180f, 180f), 0);
            target = controller.IniciarEntrenamiento();
        }
    }
    public override void Heuristic(float[] actionsOut)
    {
        float right = 0;
        if (Input.GetKey(KeyCode.D)) right = 1;
        else if (Input.GetKey(KeyCode.A)) right = -1;

        float shoot = 0;
        if (Input.GetKey(KeyCode.F)) shoot = 1;

        actionsOut[0] = right;
        actionsOut[1] = shoot;
    }
    public override void OnActionReceived(float[] vectorAction)
    {
        //rotar
        float m_TurnInputValue = vectorAction[0];
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);


        //disparar
        if(vectorAction[1] >= 0.5f)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (!ShotAvaliable)
            return;

        int enemy_layerMask = 1 << LayerMask.NameToLayer(enemy_tag);

        Vector3 direction = transform.forward;

        Instantiate(projectile, shootingPoint);
        Debug.DrawRay(transform.position, direction, Color.blue, 1f);

        if (Physics.Raycast(shootingPoint.position, direction, out var hit, 200f, enemy_layerMask))
        {
            AddReward(1f);
            EndEpisode();
        }
        else
        {
            AddReward(-0.1f);
        }

        ShotAvaliable = false;
        StartCoroutine(CountToEnableShot(timeBetweenShots));
    }
    private void Update()
    {
        if (trainingMode )
        {
            float dot = Vector3.Dot(transform.forward, (target.localPosition - transform.localPosition).normalized); //1 if pointing forward target,  -1 if facing oppsitve
            AddReward(dot/100f);
        }
    }
    private IEnumerator CountToEnableShot(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShotAvaliable = true;
        yield return null;
    }
}
