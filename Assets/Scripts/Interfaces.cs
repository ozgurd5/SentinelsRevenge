using UnityEngine;

public interface IDamageable
{
    public void GetDamage(int damageTakenAmount, Vector3 attackerTransformForward);
}

public interface IInteractable
{
    void OpenInteractionText();
    void CloseInteractionText();

    //This is stupid 1/4 -> InteractableManager.cs
    void Interact();
}