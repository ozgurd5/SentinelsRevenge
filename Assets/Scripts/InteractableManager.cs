using UnityEngine;

public class InteractableManager : MonoBehaviour, IInteractable
{
    private Canvas canvas;
    private bool isHighlighted;

    //This is stupid 2/4
    private CollectibleManager cm;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;

        //This is stupid 3/4
        cm = GetComponentInParent<CollectibleManager>();
    }

    public void OpenInteractionText()
    {
        if (isHighlighted) return;

        canvas.enabled = true;
        isHighlighted = true;
    }

    public void CloseInteractionText()
    {
        if (!isHighlighted) return;

        canvas.enabled = false;
        isHighlighted = false;
    }

    //This is stupid 4/4
    public void Interact()
    {
        cm.Collect();
    }
}
