using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarManager : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    private PlayerCombatManager pcm;

    private void Awake()
    {
        pcm = GetComponent<PlayerCombatManager>();
        pcm.OnDamageTaken += UpdateHealthBar;

        //Default value
        UpdateHealthBar(10);
    }

    private void UpdateHealthBar(int newHealth)
    {
        healthBar.value = newHealth;
        healthText.text = $"Health: {newHealth}";
    }
}
