using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineVsMachine : MonoBehaviour
{
    [System.Serializable]
    public struct Team
    {
        public GameObject prefab;
        public Transform clones_parent;
        public Transform spawning_positions;
        public List<Agente_Tanque> agents;
    }

    public Team teamA;
    public Team teamB;
    public static MachineVsMachine singleton;
    [SerializeField] private int number_agents = 4;

    //private List<Agente_Tanque> teamA_tanks;
    //private List<Agente_Tanque> teamB_tanks;


    private void Awake()
    {
        singleton = this;
        teamA.agents = new List<Agente_Tanque>();
        teamB.agents = new List<Agente_Tanque>();
        for (int i = 0; i < number_agents; i++)
        {
            teamA.agents.Add(SpawnTank(teamA));
            teamB.agents.Add(SpawnTank(teamB));
        }
    }
    private void Start()
    {
        //teamA_tanks = new List<Agente_Tanque>();
        //teamB_tanks = new List<Agente_Tanque>();
        //for(int i=0; i<number_agents; i++)
        //{
        //    teamA_tanks.Add(SpawnTank(prefab_tankA, teamA_parent));
        //    teamB_tanks.Add(SpawnTank(prefab_tankB, teamB_parent));
        //}
        //teamA_tanks.ForEach(x => AssignRandomTarget(x, teamB_tanks));
        //teamB_tanks.ForEach(x => AssignRandomTarget(x, teamA_tanks)); 
        teamA.agents.ForEach(x => x.ChooseTarget(teamB.agents));
        teamB.agents.ForEach(x => x.ChooseTarget(teamA.agents));
    }
    private Agente_Tanque SpawnTank(Team team)
    {
        int random_position = UnityEngine.Random.Range(0, team.spawning_positions.childCount);
        Vector3 position = team.spawning_positions.GetChild(random_position).position;
        Destroy(team.spawning_positions.GetChild(random_position).gameObject);
        Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(-180f, 180f), 0);
        GameObject agent = Instantiate(team.prefab, position, rotation, team.clones_parent);
        agent.name = agent.transform.parent.name + agent.transform.parent.childCount;
        return agent.GetComponent<Agente_Tanque>();
    }
    public void AgentDied(Agente_Tanque agent)
    {
        switch (agent.tag)
        {
            default:
            case "TeamA":
                teamA.agents.Remove(agent);
                if (teamA.agents.Count == 0)
                {
                    FinalizarActividad();
                    return;
                }
                teamB.agents.ForEach(x => x.ChooseTarget(teamA.agents));
                break;
            case "TeamB":
                teamB.agents.Remove(agent);
                if (teamB.agents.Count == 0)
                {
                    FinalizarActividad();
                    return;
                }
                teamA.agents.ForEach(x => x.ChooseTarget(teamB.agents));
                break;
        }
        
    }
    public void AssignRandomTarget(Agente_Tanque agent, List<Agente_Tanque> options)
    {
        Transform rand_target = options[UnityEngine.Random.Range(0, options.Count)].transform;
        agent.Target = rand_target;
        agent.transform.GetChild(0).GetComponentInChildren<Agent_Gun>().Target = rand_target;
    }
    public void FinalizarActividad()
    {
        Debug.Log("Actividad finalizada, uno de los equipos no tiene tanques restantes");
        Debug.Break();
    }
}
