using UnityEngine;

public class EnemyCombatManager : MonoBehaviour, IDamageable
{
    public void GetDamage()
    {
        Debug.Log("getting damage: " + name);
    }
}
