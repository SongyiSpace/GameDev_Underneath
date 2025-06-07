using System.Collections;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    private GameObject player;
    private MonologueManager monoManager;
    [SerializeField] private ParticleSystem waterFall;

    void Awake()
    {
        player = GameObject.Find("Player");
        monoManager = FindFirstObjectByType<MonologueManager>();
    }

    //----------Trigger Method----------//
    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.name == "showerCollider")
        {
            FindFirstObjectByType<HomeSceneEventManager>().StartShowerTriggerSequence();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (gameObject.name == "bathDoorCollider")
            // if (other.gameObject == player)
            if (doorAnimator.GetBool("isOpen"))
                doorAnimator.SetBool("isOpen", false);
    }
    
    
    public IEnumerator ShowerTrigger(System.Action onComplete)
    {
        monoManager.ShowMonologue(Monologue.Home_Showering);
        yield return new WaitForSeconds(3f);

        if (waterFall.isPlaying)
        {
            waterFall.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            waterFall.Clear();
        }
        yield return new WaitForSeconds(1f);

        monoManager.ShowMonologue(Monologue.Home_EndShower);

        onComplete?.Invoke();
    }
}