using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    //Collect animation
    //Make player stop -> Close collider -> Close interaction canvas -> Go up -> Fix rotation while player looks at collectible ->
    //-> Approach player -> Unlock item -> Remove object

    [Header("Assign")]
    [SerializeField] private float fixingRotationTime = 1;
    [SerializeField] private float goingUpTime = 1;
    [SerializeField] private float goingUpDistance = 1;
    [SerializeField] private float approachingPlayerTime = 1;

    private Transform playerTransform;
    private PlayerExtensionData ped;
    private PlayerStateData psd;

    private Collider col;
    private GameObject interactionCanvas;

    private bool isArms;

    private void Awake()
    {
        playerTransform = GameObject.Find("Player").transform;
        ped = playerTransform.GetComponent<PlayerExtensionData>();
        psd = playerTransform.GetComponent<PlayerStateData>();

        col = GetComponent<Collider>();
        interactionCanvas = transform.GetChild(0).gameObject;

        isArms = name == "CollectibleArms";
    }

    public async void Collect()
    {
        psd.playerMainState = PlayerStateData.PlayerMainState.ScriptedEvent;
        col.enabled = false;
        interactionCanvas.SetActive(false);

        transform.DOMoveY(transform.position.y + goingUpDistance, goingUpTime);
        await UniTask.WaitForSeconds(goingUpTime);

        //With this way player won't look up
        Vector3 playerLookAtTarget = transform.position;
        playerLookAtTarget.y = playerTransform.position.y;

        //With this way collectible will look at the player's direction
        Vector3 signedDistance = transform.position - playerTransform.position;
        Vector3 collectibleLookAtTarget = transform.position + signedDistance;

        playerTransform.DOLookAt(playerLookAtTarget, fixingRotationTime);

        transform.DOLookAt(collectibleLookAtTarget, fixingRotationTime / 2);
        await UniTask.WaitForSeconds(fixingRotationTime / 2);

        //This way collectible won't look down
        Vector3 collectibleRotationTarget = transform.eulerAngles;
        collectibleRotationTarget.x = 0;

        transform.DORotate(collectibleRotationTarget, fixingRotationTime / 2);
        await UniTask.WaitForSeconds(fixingRotationTime / 2);

        Transform approachingTarget;
        if (isArms) approachingTarget = playerTransform.GetChild(1);
        else approachingTarget = playerTransform.GetChild(2);

        transform.DOMove(approachingTarget.position, approachingPlayerTime);
        await UniTask.WaitForSeconds(approachingPlayerTime + 0.2f);

        if (isArms) ped.UnlockArms();
        else ped.UnlockGun();

        psd.playerMainState = PlayerStateData.PlayerMainState.Normal;
        gameObject.SetActive(false); //CloseInteractionText gives null ref error because of the interactionCanvas if we destroy. //TODO: fix
    }
}
