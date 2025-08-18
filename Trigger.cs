using System.Collections;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    private MonologueManager monoManager;
    private InteractionManager interactionManager;
    private ParticleSystem waterFall;
    [SerializeField] private GameObject doorCollider;
    private GameObject bathDoor;

    void Awake()
    {
        monoManager = FindFirstObjectByType<MonologueManager>();
        waterFall = GameObject.Find("waterFall").GetComponent<ParticleSystem>();
        interactionManager = FindFirstObjectByType<InteractionManager>();

        bathDoor = GameObject.Find("bathDoor");
    }

    //----------Trigger Method----------//
    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.name == "showerCollider" && interactionManager.isBathSwitchOn)
        {
            FindFirstObjectByType<HomeSceneEventManager>().Shower2();
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
        bathDoor.GetComponent<Collider>().enabled = false;

        if (waterFall.isPlaying)
        {
            waterFall.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            waterFall.Clear();
            SoundManager.PlaySFXSound(SFXType.SHOWERVALVE);
            yield return new WaitForSeconds(0.2f);
            SoundManager.Stop3DSound(GameObject.Find("showerHead")?.GetComponent<AudioSource>());
        }
        yield return new WaitForSeconds(1f);
        bathDoor.GetComponent<Collider>().enabled = true;
        monoManager.ShowMonologue(Monologue.Home_EndShower);

        onComplete?.Invoke();
    }
}