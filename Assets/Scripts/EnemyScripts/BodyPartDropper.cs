using UnityEngine;

public class BodyPartDropper : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] private GameObject bodyPartToDrop;
    [SerializeField] private GameObject bodyPartToRemove;

    private EnemyManager em;
    private bool isBodyPartDropped;

    private void Awake()
    {
        em = GetComponent<EnemyManager>();
    }

    private void Update()
    {
        //TODO: EVENT
        if (em.enemyState == EnemyManager.EnemyState.Dead && !isBodyPartDropped) DropBodyPart();
    }

    private void DropBodyPart()
    {
        isBodyPartDropped = true;

        bodyPartToDrop.SetActive(true);
        bodyPartToRemove.SetActive(false);

        Vector3 eulerAngles = new Vector3(90f, 0f, 0f);
        bodyPartToDrop.transform.eulerAngles = eulerAngles;
    }
}
