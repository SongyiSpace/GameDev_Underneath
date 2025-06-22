using System.Collections;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    private MonologueManager monoManager;
    private ParticleSystem waterFall;
    [SerializeField] private GameObject doorCollider;

    void Awake()
    {
        monoManager = FindFirstObjectByType<MonologueManager>();
        waterFall = GameObject.Find("waterFall").GetComponent<ParticleSystem>();
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
        if (gameObject == doorCollider)
            if (doorAnimator.GetBool("isOpen"))
                doorAnimator.SetBool("isOpen", false);
    }
    
    
    public IEnumerator ShowerTrigger(System.Action onComplete)
    {
        monoManager.ShowMonologue(Monologue.Home_Showering);
        yield return new WaitForSeconds(5f);

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