using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Scenario_Manager : MonoBehaviour
{
    [SerializeField] Transform parent_enemies;
    [SerializeField] GameObject enemy_prefab;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nivel"></param>
    /// <param name="cantidad"></param>
    /// <returns>Returns de Enemy Transform</returns>
    public Transform IniciarEntrenamiento(int nivel, int cantidad=1)
    {
        if(parent_enemies.childCount>0)
        {
            for (int i=parent_enemies.childCount-1; i>=0; i--)
            {
                Destroy(parent_enemies.GetChild(i).gameObject);
            }
        }
        Vector3 startPoint;
        Vector3 finaltPoint;
        GameObject enemy = Instantiate(enemy_prefab, parent_enemies);
        Vector2 xyBounds = new Vector2(-100, 100);
        Enemy_Train enemy_component = enemy.GetComponent<Enemy_Train>();
        switch (nivel)
        {
            default:
            case 0:
                startPoint = Vector3.one * UnityEngine.Random.Range(-35, 35);
                startPoint.y = 0;
                enemy_component.ConfigureSpotPoint(startPoint);
                break;
            case 1:
                startPoint = Vector3.one * UnityEngine.Random.Range(-35, 35);
                finaltPoint = startPoint * -1f;
                startPoint.y = 0;
                finaltPoint.y = 0;
                enemy_component.ConfigureRoute(0.1f, startPoint, finaltPoint);
                break;
            case 3:
                enemy_component.ConfigureRandom(0.1f, cantidad * 5, xyBounds, Vector2.one, xyBounds);
                break;
        }
        return enemy.transform;
    }

}
