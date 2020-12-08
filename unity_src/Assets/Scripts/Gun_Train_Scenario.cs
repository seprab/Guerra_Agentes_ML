using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Train_Scenario : MonoBehaviour
{
    [SerializeField] Transform parent_enemies;
    [SerializeField] GameObject target_prefab;

    public Transform IniciarEntrenamiento()
    {
        if (parent_enemies.childCount > 0)
        {
            for (int i = parent_enemies.childCount - 1; i >= 0; i--)
            {
                Destroy(parent_enemies.GetChild(i).gameObject);
            }
        }

        GameObject target = Instantiate(target_prefab, parent_enemies);
        target.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(-180f, 180f), 0);
        return target.transform.GetChild(0);
    }
}
