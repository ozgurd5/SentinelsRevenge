using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarManager : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] private Slider healthBar;

    private EnemyCombatManager ecm;

    private void Awake()
    {
        ecm = GetComponent<EnemyCombatManager>();
        ecm.OnDamageTaken += UpdateHealthBar;

        //Default value
        UpdateHealthBar(10);
    }

    private void UpdateHealthBar(int newHealth)
    {
        healthBar.value = newHealth;
    }
}
