using UnityEngine;

public class InteractionText : MonoBehaviour, IInteractable
{
    public bool isHighlighted;

    public void HighlightText()
    {
        if (isHighlighted) return;

        isHighlighted = true;
        Debug.Log("highlight text");
    }

    public void UnhighlightText()
    {
        if (!isHighlighted) return;

        isHighlighted = false;
        Debug.Log("unhighlight text");
    }
}
