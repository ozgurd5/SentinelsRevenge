using Cysharp.Threading.Tasks;
using UnityEngine;

public class MaterialInterpolation : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] private Material[] targetMaterials;

    private SkinnedMeshRenderer smr;
    private Material[] startingMaterials;

    private void Awake()
    {
        smr = GetComponentInChildren<SkinnedMeshRenderer>();
        startingMaterials = smr.materials;
    }

    public async void ChangeMaterial(float duration)
    {
        float timePassed = 0f;

        while (timePassed <= duration + 0.01f)
        {
            for (int i = 0; i < targetMaterials.Length; i++)
            {
                smr.materials[i].Lerp(startingMaterials[i], targetMaterials[i], timePassed / (duration * 3)); //TODO: why
            }

            timePassed += Time.deltaTime;
            await UniTask.NextFrame();
        }
    }
}
