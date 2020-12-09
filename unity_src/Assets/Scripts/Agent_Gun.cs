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
    private int SuccesfulShots = 0;

    [SerializeField] private string enemy_tag;
    [SerializeField] private Gun_Train_Scenario train_controller;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private bool trainingMode;
    [SerializeField] private float m_TurnSpeed = 180f;
    [SerializeField] private float timeBetweenShots = 1f;
    public Transform Target
    {
        get { return target; }
        set { target = value; }
    }


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
            sensor.AddObservation(new float[1]);
            return;
        }

        // 3 observations
        //Vector3 toEnemy = target.localPosition - transform.localPosition;
        //sensor.AddObservation(toEnemy.normalized);

        // 1 observation
        float dot = Vector3.Dot(transform.forward, (target.position - transform.position).normalized); //1 if pointing forward target,  -1 if facing oppsitve
        sensor.AddObservation(dot);


        // 1 observation
        //sensor.AddObservation(ShotAvaliable);
    }
    public override void OnEpisodeBegin()
    {
        m_Rigidbody.angularVelocity = Vector3.zero;
        if (trainingMode)
        {
            SuccesfulShots = 0;
            ShotAvaliable = true;          
            transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(-180f, 180f), 0);
            Debug.Log(CompletedEpisodes);
            int level = Mathf.FloorToInt(CompletedEpisodes / 300) + 1;
            Target = train_controller.IniciarEntrenamiento(level);
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

        Instantiate(projectile, shootingPoint);

        if(trainingMode)
        {
            int enemy_layerMask = 1 << LayerMask.NameToLayer(enemy_tag);
            Vector3 direction = transform.forward;
            
            if (Physics.Raycast(shootingPoint.position, direction, out var hit, 50f, enemy_layerMask))
            {
                Debug.DrawRay(transform.position, direction*50f, Color.blue, 1f);
                AddReward(1f);
                SuccesfulShots++;
                if(SuccesfulShots >= 10)
                {
                    EndEpisode();
                }
                
            }
            else
            {
                Debug.DrawRay(transform.position, direction*50f, Color.red, 1f);
                AddReward(-0.03f);
            }
        }
        

        ShotAvaliable = false;
        StartCoroutine(CountToEnableShot(timeBetweenShots));
    }
    private void FixedUpdate()
    {
        if (trainingMode)
        {
            if (target == null) return;
            float dot = Vector3.Dot(transform.forward, (target.localPosition - transform.localPosition).normalized); //1 if pointing forward target,  -1 if facing oppsitve
            float reward = dot - 0.8f;
            AddReward(reward/100f);
        }
    }
    private IEnumerator CountToEnableShot(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShotAvaliable = true;
        yield return null;
    }
}
