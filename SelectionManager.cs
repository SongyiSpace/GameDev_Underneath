using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    private string selectableTag = "Selectable";
    private Transform InteractorSource;
    public float InteractRange;
    private InteractionManager interactionManager;
    private RabbitInteract rabbitInteract;

    private Transform highlight;

    // 시작할 때 selectable태그 가진 오브젝트들에게 outline 컴포넌트 씌우기
    void Start()
    {
        interactionManager = FindFirstObjectByType<InteractionManager>();
        rabbitInteract = FindFirstObjectByType<RabbitInteract>();

        if (InteractorSource == null) InteractorSource = Camera.main.transform;

        //selectable 태그 가진 오브젝트에게 outline 적용
        GameObject[] selectables = GameObject.FindGameObjectsWithTag("Selectable");

        foreach (GameObject obj in selectables)
        {
            if (obj.GetComponent<Outline>() == null)
            {
                Outline outline = obj.AddComponent<Outline>();
                outline.enabled = false;
            }
        }
    }

    //오브젝트들의 outline enabled 조작
    void Update()
    {
        //기존 outline 꺼주기
        if (highlight != null)
        {
            Outline oldOutline = highlight.GetComponent<Outline>();
            if (oldOutline != null)
            {
                oldOutline.enabled = false;
            }
            highlight = null;
        }

        //닿는 물체에 outline있으면 켜주기
        Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out RaycastHit hit, InteractRange))
        {
            Transform target = hit.transform;

            if (target.CompareTag(selectableTag))
            {
                Outline outline = target.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = true;
                    highlight = target;
                }

                // 클릭
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject targetObject = hit.collider.gameObject;
                    
                    Debug.Log("클릭받은 오브젝트 :" + targetObject.name);

                    if (targetObject.name == "rabbit")
                    {
                        rabbitInteract.Interact();
                    }
                    else
                    {
                        interactionManager.Interact(targetObject);    
                    }

                }
            }
        }
    }
}
