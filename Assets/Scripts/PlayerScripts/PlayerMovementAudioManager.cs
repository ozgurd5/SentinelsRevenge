using UnityEngine;

//TODO: THIS SCRIPT IS SHIT BUT I AM VERY SLEEPY, HAVE FEW HOURS LEFT UNTIL JAM DEADLINE, IMPORTANT THINGS TO FIX AND DONT HAVE ANY BRAIN CELL TO SPARE

public class PlayerMovementAudioManager : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] private AudioSource walkSource;
    [SerializeField] private AudioSource runSource;
    [SerializeField] private AudioSource jumpSource;

    private bool isWalkSourcePlaying;
    private bool isRunSourcePlaying;
    private bool isJumpSourcePlaying;

    private PlayerStateData psd;

    private void Awake()
    {
        psd = GetComponent<PlayerStateData>();
    }

    private void Update()
    {
        //Walk - Run Group
        if (!psd.isMoving)
        {
            isWalkSourcePlaying = false;
            walkSource.Stop();

            isRunSourcePlaying = false;
            runSource.Stop();
        }
        else if (psd.isWalking && !isWalkSourcePlaying)
        {
            isWalkSourcePlaying = true;
            walkSource.Play();

            isRunSourcePlaying = false;
            runSource.Stop();
        }
        else if (psd.isRunning && !isRunSourcePlaying)
        {
            isWalkSourcePlaying = false;
            walkSource.Stop();

            isRunSourcePlaying = true;
            runSource.Play();
        }

        //Jump Group
        //if (psd.isJumping && !isJumpSourcePlaying)
        //{
        //    isJumpSourcePlaying = true;
        //    jumpSource.Play();
        //}
        //else if (!psd.isJumping && isJumpSourcePlaying)
        //{
        //    isJumpSourcePlaying = false;
        //    jumpSource.Stop();
        //}
    }
}
