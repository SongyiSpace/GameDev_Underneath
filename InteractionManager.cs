using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private GameObject fridgeDoor;
    [SerializeField] private GameObject microDoor;
    [SerializeField] private GameObject food1;
    [SerializeField] private GameObject food2;

    private Animator fridgeAnimator;
    private Animator microAnimator;
    private bool fridgeisOpen = false;
    private bool microisOpen = false;
    private HomeSceneEventManager homeEventManager;

    private void Awake()
    {
        fridgeAnimator = fridgeDoor.GetComponent<Animator>();
        microAnimator = microDoor.GetComponent<Animator>();
        homeEventManager = FindFirstObjectByType<HomeSceneEventManager>();
    }

    public void Interact(GameObject obj)
    {
        if (obj.name == "fridgeDoor") FridgeInteract();
        if (obj.name == "microDoor") MicroInteract();
        if (obj.name == "food1") Food1Interact();
        if (obj.name == "food2") Food2Interact();
    }



    private void FridgeInteract()
    {
        fridgeisOpen = !fridgeisOpen;
        fridgeAnimator.SetBool("isOpen", fridgeisOpen);
    }
    private void MicroInteract()
    {
        microisOpen = !microisOpen;
        microAnimator.SetBool("isOpen", microisOpen);
    }
    private void Food1Interact()
    {
        homeEventManager.NextEvent();
    }
    private void Food2Interact()
    {
        homeEventManager.NextEvent();
    }

}
