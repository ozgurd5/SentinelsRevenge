using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] private float interactRange = 30f;
    [SerializeField] private float meleeAttackRange = 20f;
    [SerializeField] private float rangeAttackRange = 100f;
    [SerializeField] [Range(0, 1)] private float opacity = 0.3f;

    [Header("Info - No Touch")]
    public bool canInteract;
    public bool canMeleeAttack;
    public bool canRangedAttack;
    public float distanceToHitTarget;

    public IDamageable damagable;
    public IInteractable interactable;

    private Camera cam;
    private PlayerStateData psd;
    private Image crosshairImage;

    public RaycastHit crosshairHit;
    private Ray crosshairRay;
    private int layerMask = ~(1 << 7); //evil bit hack

    private Color temporaryColor;

    [Header("Debug Info")] [SerializeField] private GameObject lookingTarget;

    private void Awake()
    {
        cam = Camera.main;
        psd = GetComponent<PlayerStateData>();
        crosshairImage = GetComponentInChildren<Image>();

        //Default value
        temporaryColor = Color.white;
        temporaryColor.a = opacity;
        crosshairImage.color = temporaryColor;
    }

    private void Update()
    {
        if (psd.playerMainState != PlayerStateData.PlayerMainState.Normal) return;

        CastRays();
        HandleCrosshairColor();

        //DEBUG
        if (crosshairHit.collider != null) lookingTarget = crosshairHit.collider.gameObject;
    }

    private void CastRays()
    {
        crosshairRay = cam.ScreenPointToRay(crosshairImage.rectTransform.position);

        canInteract = false;
        canMeleeAttack = false;
        canRangedAttack = false;

        damagable = null;
        interactable = null;

        if (Physics.Raycast(crosshairRay, out crosshairHit, rangeAttackRange, layerMask))
        {
            distanceToHitTarget = Vector3.Distance(transform.position, crosshairHit.collider.transform.position);

            if (distanceToHitTarget < meleeAttackRange)
            {
                interactable = crosshairHit.collider.GetComponent<IInteractable>();
                canInteract = interactable != null;

                damagable = crosshairHit.collider.GetComponent<IDamageable>();
                canMeleeAttack = damagable != null;

                damagable = crosshairHit.collider.GetComponent<IDamageable>();
                canRangedAttack = damagable != null;
            }

            else if (distanceToHitTarget < interactRange)
            {
                interactable = crosshairHit.collider.GetComponent<IInteractable>();
                canInteract = interactable != null;

                damagable = crosshairHit.collider.GetComponent<IDamageable>();
                canRangedAttack = damagable != null;
            }

            else if (distanceToHitTarget < rangeAttackRange)
            {
                damagable = crosshairHit.collider.GetComponent<IDamageable>();
                canRangedAttack = damagable != null;
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
}
