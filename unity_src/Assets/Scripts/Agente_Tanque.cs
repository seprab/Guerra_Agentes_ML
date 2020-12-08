using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public class Agente_Tanque : Agent
{
    [SerializeField] private string enemy_tag;
    [SerializeField] private bool trainingMode;
    [SerializeField] private float m_Speed = 12f;
    [SerializeField] private float m_TurnSpeed = 180f; 
    [SerializeField] private Scenario_Manager controller;

    private Rigidbody m_Rigidbody;
    private Transform target;

    public override void Initialize()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.isKinematic = false;
        transform.GetChild(0).GetComponent<RayPerceptionSensorComponent3D>().DetectableTags[1] = enemy_tag;
        if (!trainingMode) MaxStep = 0; //El valor de 0 significa infinito
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        if (target==null)
        {
            sensor.AddObservation(new float[12]);
            return;
        }

        // 4 observations
        sensor.AddObservation(transform.localRotation.normalized);

        // 3 observations
        Vector3 toEnemy = target.localPosition - transform.localPosition;
        sensor.AddObservation(toEnemy.normalized);


        // 1 observation
        float dot = Vector3.Dot(transform.forward, (target.localPosition - transform.localPosition).normalized); //1 if pointing forward target,  -1 if facing oppsitve
        sensor.AddObservation(dot);


        // 1 observation
        float distance = Vector3.Distance(transform.localPosition, target.localPosition);
        distance = Mathf.Clamp01(distance / 200f);
        sensor.AddObservation(distance);

        // 3 observation (Modelo 30)
        Vector3 speed = m_Rigidbody.velocity.normalized;
        sensor.AddObservation(speed);
    }
    public override void OnEpisodeBegin()
    {
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.angularVelocity = Vector3.zero;

        if (trainingMode)
        {
            int enemies = 1;
            int level = 0;
            if (CompletedEpisodes < 500)
            {
                level = 0;
            }
            else/* if (CompletedEpisodes < 2000)*/
            {
                level = 1;
            }
            //else
            //{
            //    level = 2;
            //    enemies = Mathf.Clamp(CompletedEpisodes / 5000, 1, 3);
            //}
            transform.localPosition = new Vector3(0, 0.4f, 0);
            transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(-180f,180f), 0);
            target = controller.IniciarEntrenamiento(level, enemies);
        }
    }
    public override void Heuristic(float[] actionsOut)
    {
        float forward = 0;
        float right = 0;

        if (Input.GetKey(KeyCode.W)) forward = 1;
        else if (Input.GetKey(KeyCode.S)) forward = -1;

        if (Input.GetKey(KeyCode.D)) right = 1;
        else if (Input.GetKey(KeyCode.A)) right = -1;

        actionsOut[0] = forward;
        actionsOut[1] = right;
    }
    public override void OnActionReceived(float[] vectorAction)
    {
        float m_MovementInputValue = Mathf.Clamp01(vectorAction[0]);
        float m_TurnInputValue = vectorAction[1];


        //mover
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);

        //rotar
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }


    private void Update()
    {
        if(trainingMode)
        {
            EvaluateGeneralPosition();
        }
    }
    private void EvaluateGeneralPosition()
    {
        float enemy_threshold = 50f;
        float far_threshold = 80;

        float distance = Vector3.Distance(transform.localPosition, target.localPosition);
        float dot = Vector3.Dot(transform.forward, (target.localPosition - transform.localPosition).normalized); //1 if pointing forward target,  -1 if facing oppsitve
        float distance_reward = 0;
        float angle_reward = 0;
        if(distance < enemy_threshold)
        {
            //inner radius closer to the enemies center position
            distance_reward = -(distance / enemy_threshold);
            angle_reward = dot * -1;
        }
        else if(distance > far_threshold)
        {
            //outer radius away from the enemies center position
            distance_reward = -Mathf.Clamp01((distance - 80) / 200f);
            angle_reward = dot;
        }
        else
        {
            float safe_space = (distance - enemy_threshold) / (far_threshold- enemy_threshold); //Expected [0-1]
            safe_space = Mathf.Abs(safe_space - 0.5f); //expected [0, 0.5f] including negatives into positives. 0 is closer to center of the safe spot
            safe_space = (Mathf.Abs(safe_space - 0.5f))/0.5f; //expected [0,1] 1 closer to center of safe sport, 0 on the edge of the safe spot
            distance_reward = safe_space;
        }
        AddReward(distance_reward);
        AddReward(angle_reward);
    }
    private void OnTriggerEnter(Collider other)
    {
        TriggerEnterOrStay(other);
    }
    private void OnTriggerStay(Collider other)
    {
        TriggerEnterOrStay(other);
    }
    private void TriggerEnterOrStay(Collider other)
    {
        //if (other.CompareTag("Enemigo"))
        //{
        //    float distance2enemy = Vector3.Distance(transform.position, other.transform.position);
        //    float distance_reward = -(distance2enemy / other.transform.localScale.x);
        //    float angle_reward = AngleReward(other.transform.position);
            
        //    AddReward(distance_reward + angle_reward);
        //}
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (trainingMode)
        {
            AddReward(-0.5f);
        }
    }
}
