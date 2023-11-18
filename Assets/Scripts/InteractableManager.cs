using UnityEngine;

public class InteractableManager : MonoBehaviour, IInteractable
{
    private Canvas canvas;
    private bool isHighlighted;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    public void OpenInteractionText()
    {
        if (isHighlighted) return;

        canvas.enabled = true;
        isHighlighted = true;
        Debug.Log("highlight text: " + name);
    }

    public void CloseInteractionText()
    {
        if (!isHighlighted) return;

        canvas.enabled = false;
        isHighlighted = false;
        Debug.Log("unhighlight text: " + name);
    }

    public void Interact()
    {
        
    }
}
