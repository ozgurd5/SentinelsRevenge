using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    [Header("Sound")] [SerializeField] private AudioSource extensionUnlockSound;

    private PlayerInputManager pim;
    private CrosshairManager cm;

    private bool isLookingAtInteractable;

    private void Awake()
    {
        pim = GetComponent<PlayerInputManager>();
        cm = GetComponent<CrosshairManager>();
    }

    private void Update()
    {
        HandleInteractionText();

        if (cm.canInteract && pim.isInteractKeyDown)
        {
            cm.interactable.Interact();
            extensionUnlockSound.Play();
        }
    }

    private void HandleInteractionText()
    {
        if (cm.canInteract) cm.interactable.OpenInteractionText();
        else cm.previousInteractable?.CloseInteractionText();

        //TODO: Fix looking at another interactable while looking at an interactable, without looking to any other object
    }
}
