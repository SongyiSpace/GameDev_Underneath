using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private GameObject fridgeDoor;
    [SerializeField] private GameObject microDoor;
    [SerializeField] private GameObject bathDoor;
    [SerializeField] private GameObject food1;
    [SerializeField] private GameObject food2;
    [SerializeField] private GameObject food3;
    [SerializeField] private GameObject computer;
    [SerializeField] private GameObject showerHead;
    [SerializeField] private GameObject switchToggleObject;
    [SerializeField] private GameObject[] lightsToControl;


    private Animator fridgeAnimator;
    private Animator microAnimator;
    private Animator bathDoorAnimator;
    private bool fridgeIsOpen = false;
    private bool microIsOpen = false;
    private bool bathDoorIsOpen = false;
    public bool isBathSwitchOn = true;

    private HomeSceneEventManager homeEventManager;

    private Dictionary<string, Action> interactionActions;

    private void Awake()
    {
        fridgeAnimator = fridgeDoor.GetComponent<Animator>();
        microAnimator = microDoor.GetComponent<Animator>();
        bathDoorAnimator = bathDoor.GetComponent<Animator>();
        homeEventManager = FindFirstObjectByType<HomeSceneEventManager>();

        interactionActions = new Dictionary<string, Action>
        {
            { "fridgeDoor", () => ToggleDoor(fridgeAnimator, ref fridgeIsOpen) },
            { "microDoor", () => ToggleDoor(microAnimator, ref microIsOpen) },
            { "bathDoor", () => ToggleDoor(bathDoorAnimator, ref bathDoorIsOpen) },
            { "food1", () => homeEventManager.PickUpFridgeFood() },
            { "food2", () => homeEventManager.MicroFood() },
            { "food3", () => homeEventManager.EatFood() },
            { "computer", () => homeEventManager.Study() },
            { "showerHead", () => homeEventManager.Shower() },
            { "lightSwitch", ()=> ToggleSwitch()}
        };
    }

    public void Interact(GameObject obj)
    {
        if (interactionActions.TryGetValue(obj.name, out Action action))
        {
            action.Invoke();
        }
        else Debug.LogWarning($"{obj.name}에 대한 상호작용 없음");
    }

    private void ToggleDoor(Animator animator, ref bool isOpen)
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);
    }
    public void ToggleSwitch()
    {
        isBathSwitchOn = !isBathSwitchOn;

        if (isBathSwitchOn)
        {
            switchToggleObject.transform.rotation = Quaternion.Euler(0, 0, -50f);
            foreach (GameObject lightObj in lightsToControl) lightObj.SetActive(true);
        }
        else
        {
            switchToggleObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            foreach (GameObject lightObj in lightsToControl) lightObj.SetActive(false);
        }
    }


}
