public interface IDamageable
{
    public void GetDamage();
}

public interface IInteractable
{
    void OpenInteractionText();
    void CloseInteractionText();
    void Interact();
}
