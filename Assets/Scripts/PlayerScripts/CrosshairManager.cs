using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] private float interactRange = 5f;
    [SerializeField] private float rangeAttackRange = 40f;
    [SerializeField] [Range(0, 1)] private float opacity = 0.3f;
    [SerializeField] private Vector3 overlapBoxHalfExtends = new Vector3(0.1f, 0.5f, 0.1f);
    [SerializeField] private float overlapSphereSize = 0.6f;
    [SerializeField] private Transform overlapBoxCenterTransform;
    [SerializeField] private Transform overlapSphereCenterTransform;

    [Header("Info - No Touch")]
    public bool canInteract;
    public bool canMeleeAttack;
    public bool canRangedAttack;
    public float distanceToHitTarget;
    public Collider[] overlapColliders;

    public IDamageable damageable;
    public IInteractable interactable;
    public IInteractable previousInteractable;

    private PlayerExtensionData ped;
    private PlayerStateData psd;
    private Camera cam;
    private Image crosshairImage;

    private RaycastHit crosshairHit;
    private Ray crosshairRay;
    private int layerMask = ~(1 << 7); //evil bit hack

    private Color temporaryColor;

    private void Awake()
    {
        ped = GetComponent<PlayerExtensionData>();
        psd = GetComponent<PlayerStateData>();
        cam = Camera.main;
        crosshairImage = GetComponentInChildren<Image>();

        //Default value
        temporaryColor = Color.white;
        temporaryColor.a = opacity;
        crosshairImage.color = temporaryColor;
    }

    private void Update()
    {
        if (psd.playerMainState != PlayerStateData.PlayerMainState.Normal) return;

        CastRay();
        OverlapBoxOrSphere();
        HandleCrosshairColor();

        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.G)) Debug.Log(crosshairHit.collider.gameObject.name);
        #endif
    }

    //TODO: MORE FLEXIBLE CODE
    private void CastRay()
    {
        crosshairRay = cam.ScreenPointToRay(crosshairImage.rectTransform.position);

        canInteract = false;
        canMeleeAttack = false;
        canRangedAttack = false;

        damageable = null;
        interactable = null;

        if (Physics.Raycast(crosshairRay, out crosshairHit, rangeAttackRange, layerMask))
        {
            distanceToHitTarget = Vector3.Distance(transform.position, crosshairHit.collider.transform.position);

            if (distanceToHitTarget < interactRange)
            {
                canInteract = crosshairHit.collider.CompareTag("Interactable");
                if (canInteract)
                {
                    interactable = crosshairHit.collider.GetComponentInChildren<IInteractable>();
                    previousInteractable = interactable;
                }

                canRangedAttack = crosshairHit.collider.CompareTag("Enemy");
                if (canRangedAttack) damageable = crosshairHit.collider.GetComponent<IDamageable>();
            }

            else if (distanceToHitTarget < rangeAttackRange)
            {
                canRangedAttack = crosshairHit.collider.CompareTag("Enemy");
                if (canRangedAttack) damageable = crosshairHit.collider.GetComponent<IDamageable>();
            }
        }
    }

    private void OverlapBoxOrSphere()
    {
        if (ped.hasArms)
        {
            overlapColliders = Physics.OverlapSphere(overlapSphereCenterTransform.position, overlapSphereSize);
            foreach (Collider item in overlapColliders)
            {
                if (item.CompareTag("Enemy"))
                {
                    canMeleeAttack = true;
                    damageable = item.GetComponent<IDamageable>();
                    break;
                }
            }
        }

        else
        {
            overlapColliders = Physics.OverlapBox(overlapBoxCenterTransform.position, overlapBoxHalfExtends);
            foreach (Collider item in overlapColliders)
            {
                if (item.CompareTag("Enemy"))
                {
                    canMeleeAttack = true;
                    damageable = item.GetComponent<IDamageable>();
                    break;
                }
            }
        }
    }

    private void HandleCrosshairColor()
    {
        //We can not directly change crosshairImage.color.a
        //We can only assign a color variable to it. Therefore we need a temporary color variable..
        //..to make changes upon and finally assign it

        temporaryColor = Color.white;

        if (canMeleeAttack)
        {
            temporaryColor = Color.red;
            temporaryColor.a = 1f;
        }

        else if (canInteract)
        {
            temporaryColor.a = 1f;
        }

        else if (canRangedAttack && psd.isAiming)
        {
            temporaryColor = Color.blue;
            temporaryColor.a = 1f;
        }

        else temporaryColor.a = opacity;

        crosshairImage.color = temporaryColor;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (ped.hasArms) Gizmos.DrawWireSphere(overlapSphereCenterTransform.position, overlapSphereSize);
        else Gizmos.DrawWireCube(overlapBoxCenterTransform.position, overlapBoxHalfExtends * 2);
    }
}
