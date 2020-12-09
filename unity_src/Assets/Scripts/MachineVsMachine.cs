using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineVsMachine : MonoBehaviour
{
    [SerializeField] private Transform teamA_parent;
    [SerializeField] private Transform teamB_parent;
    [SerializeField] private Transform spawning_positions;
    [SerializeField] private GameObject prefab_tankA;
    [SerializeField] private GameObject prefab_tankB;

    private List<Agente_Tanque> teamA_tanks;
    private List<Agente_Tanque> teamB_tanks;
    private int number_agents = 4;

    public static MachineVsMachine singleton;

    private void Awake()
    {
        singleton = this;
    }
    private void Start()
    {
        teamA_tanks = new List<Agente_Tanque>();
        teamB_tanks = new List<Agente_Tanque>();
        for(int i=0; i<number_agents; i++)
        {
            teamA_tanks.Add(SpawnTank(prefab_tankA, teamA_parent));
            teamB_tanks.Add(SpawnTank(prefab_tankB, teamB_parent));
        }
        teamA_tanks.ForEach(x => AssignRandomTarget(x, teamB_tanks));
        teamB_tanks.ForEach(x => AssignRandomTarget(x, teamA_tanks));
    }
    private Agente_Tanque SpawnTank(GameObject prefab, Transform parent)
    {
        Vector3 position = spawning_positions.GetChild(UnityEngine.Random.Range(0, spawning_positions.childCount)).position;
        Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(-180f, 180f), 0);
        return Instantiate(prefab, position, rotation, parent).GetComponent<Agente_Tanque>();
    }
    public void AgentDied(Agente_Tanque agent)
    {
        Agente_Tanque destroyer;
        switch (agent.tag)
        {
            default:
            case "TeamA":
                teamA_tanks.Remove(agent);
                destroyer = teamB_tanks.Find(x => x.Target == agent.transform);
                AssignRandomTarget(destroyer, teamA_tanks);
                break;
            case "TeamB":
                teamB_tanks.Remove(agent);
                destroyer = teamA_tanks.Find(x => x.Target == agent.transform);
                AssignRandomTarget(destroyer, teamB_tanks);
                break;
        } 
    }
    public void AssignRandomTarget(Agente_Tanque agent, List<Agente_Tanque> options)
    {
        Transform rand_target = options[UnityEngine.Random.Range(0, options.Count)].transform;
        agent.Target = rand_target;
        agent.transform.GetChild(0).GetComponentInChildren<Agent_Gun>().Target = rand_target;
    }
}
