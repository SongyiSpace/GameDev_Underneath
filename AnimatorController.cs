using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimatorController : MonoBehaviour
{
    private PlayerMove playerMove;
    private MonologueManager monoManager;
    private TransparentSwitcher switcher;
    private ScreenFader screenFader;

    private GameObject rabbit;
    private GameObject player;
    private GameObject lamp;
    private GameObject food3;

    private Transform cam;
    private Transform playerTF;

    private Animator playerAnimator;
    private Animator lampAnimator;

    [SerializeField] public Material monitorMat;
    [SerializeField] public Material blackMat;
    private MeshRenderer monitorMeshrenderer;

    private float eatTime = 7f;

    void Start()
    {
        playerMove = GameObject.Find("Player").GetComponent<PlayerMove>();
        rabbit = GameObject.Find("rabbit");
        player = GameObject.Find("Player");
        playerTF = GameObject.Find("Player").GetComponent<Transform>();
        lamp = GameObject.Find("lamp");
        food3 = GameObject.Find("food3");
        cam = Camera.main.transform;
        screenFader = FindFirstObjectByType<ScreenFader>();
        monoManager = FindFirstObjectByType<MonologueManager>();
        string currentScene = SceneManager.GetActiveScene().name;
        switcher = FindFirstObjectByType<TransparentSwitcher>();

        playerAnimator = player.GetComponent<Animator>();
        playerAnimator.enabled = false;
        if (currentScene == "Home")
        {
            monitorMeshrenderer = GameObject.Find("monitor").GetComponent<MeshRenderer>();
            lampAnimator = lamp.GetComponent<Animator>();
        }

    }

    //================[ 공통 메소드 ]================//
    private IEnumerator WaitForAnimation(string stateName)
    {
        yield return new WaitUntil(() =>
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
            playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
    }
    private IEnumerator WaitForMonologue(string[] monologueMessage)
    {
        bool isDone = false;
        monoManager.ShowMonologue(monologueMessage, () => isDone = true);
        yield return new WaitUntil(() => isDone);
    }

    //================[ 토끼 줍는 애니메이션 ]================//
    public void Ani_PickUpRabbit()
    {
        if (playerAnimator != null)
        {
            StartCoroutine(PlayPickUpRabbit());
        }
    }
    private IEnumerator PlayPickUpRabbit()
    {
        playerMove.canMove = false;
        playerAnimator.enabled = true;
        playerAnimator.SetTrigger("pickupRabbit");
        yield return new WaitForSeconds(2f);
        switcher.SwitchToTransparent(rabbit);
        yield return new WaitForSeconds(3f);

        // 1. 위치 및 회전값 저장
        Quaternion originalCamRot = cam.rotation;
        Quaternion originalPlayerRot = playerTF.rotation;
        
        // 2. 정면 쳐다보기
        Quaternion playerLookFront = Quaternion.Euler(0f, 0f, 0f);
        Quaternion camLookFront = Quaternion.Euler(0f, 0f, 0f);
        float t = 0;
        while (t < 1f)
        {
            playerTF.rotation = Quaternion.Slerp(originalPlayerRot, playerLookFront, t / 1f);
            cam.rotation = Quaternion.Slerp(originalCamRot, camLookFront, t / 1f);
            t += Time.deltaTime;
            yield return null;
        }

        //3. 직진
        Vector3 originalPlayerPos = playerTF.position;
        Vector3 targetPos = originalPlayerPos + playerTF.forward * 10f;
        SoundManager.PlayFootStepSound(FootstepType.ROADFOOTSTEP, 1f, 1.2f);
        screenFader.StartFadeOut(3f);
        t = 0;
        while (t < 5f)
        {
            playerTF.position = Vector3.Lerp(originalPlayerPos, targetPos, t / 5f);
            t += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene("Home");
        screenFader.StartFadeIn(3f);
    }

    //================= [ 밥 먹는 애니메이션 ] =================//
    public void Ani_EatFood()
    {
        StartCoroutine(EatFoodAnimation(cam.transform, player.transform));
    }
    private IEnumerator EatFoodAnimation(Transform cam, Transform player)
    {
        playerMove.canMove = false;

        //상 앞에 앉은 시점
        player.position = new Vector3(0.33f, 2.3f, -1.3f);
        player.rotation = Quaternion.Euler(0f, 180f, 0f);
        cam.localPosition = new Vector3(0f, 0.11f, 0f);
        cam.localRotation = Quaternion.Euler(30f, 0f, 0f);

        yield return new WaitForSeconds(1f);
        food3.transform.Find("Fork").gameObject.SetActive(false);
        SoundManager.PlaySFXSound(SFXType.EATING_DINNER);
        yield return new WaitForSeconds(eatTime);
        SoundManager.StopSFXSound();
        food3.transform.Find("Fork").gameObject.SetActive(true);

        yield return WaitForMonologue(Monologue.Home_DoneEat);

        food3.SetActive(false);

        //상 앞에서 일어난 시점
        player.position = new Vector3(0.33f, 2.27f, -1.3f);
        player.rotation = Quaternion.Euler(0f, 180f, 0f);
        cam.localPosition = new Vector3(0f, 0.543f, 0f);
        cam.localRotation = Quaternion.Euler(0f, 0f, 0f);
        playerMove.rt = player.rotation.eulerAngles;
        playerMove.rt.x = cam.localRotation.eulerAngles.x;
        monoManager.ShowMonologue(Monologue.Home_LetsWork);
    }

    //================= [ 일하는 애니메이션 ] =================//
    public void Ani_Work()
    {
        StartCoroutine(WorkAnimation(cam.transform, player.transform));
    }
    private IEnumerator WorkAnimation(Transform cam, Transform player)
    {
        playerMove.canMove = false;

        player.transform.position = new Vector3(1.1f, 2.27f, 0.28f);
        player.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        cam.localPosition = new Vector3(0f, 0.42f, 0f);
        cam.localRotation = Quaternion.Euler(15f, 0f, 0f);

        //모니터 on
        Material[] mats = monitorMeshrenderer.materials; // 현재 머티리얼 배열 가져오기
        mats[1] = monitorMat;                            // 0번 슬롯에 켜진 머티리얼 적용
        monitorMeshrenderer.materials = mats; 
        // Material[] mats = monitorMeshrenderer.materials;
        // for (int i = 0; i < mats.Length; i++)
        //     if (mats[i].name.Contains("black"))
        //         mats[i] = monitorMat;
        // monitorMeshrenderer.materials = mats;

        playerAnimator.enabled = true;

        playerAnimator.Play("PlayerStayOrigin");
        SoundManager.PlayLoopSound(LoopType.KEYBOARD_TYPING);
        yield return new WaitForSeconds(7f);
        SoundManager.StopLoopSound();

        playerAnimator.SetBool("focusSound", true);

        yield return WaitForAnimation("PlayerLookRight");

        yield return StartCoroutine(WaitForMonologue(Monologue.Doubt));

        playerAnimator.SetBool("focusSound", false);
        yield return WaitForAnimation("PlayerLookOrigin");

        playerAnimator.Play("PlayerStayOrigin");
        SoundManager.PlayLoopSound(LoopType.KEYBOARD_TYPING);
        yield return new WaitForSeconds(7f);
        SoundManager.StopLoopSound();

        playerAnimator.SetBool("focusSound", true);
        yield return WaitForAnimation("PlayerLookRight");

        yield return new WaitForSeconds(3f);

        playerAnimator.SetBool("focusSound", false);
        yield return WaitForAnimation("PlayerLookOrigin");

        //모니터 off
        mats = monitorMeshrenderer.materials;           // 배열 새로 가져오기
        mats[1] = blackMat;                              // 0번 슬롯에 검은 머티리얼 적용
        monitorMeshrenderer.materials = mats;           // 다시 할당
        // for (int i = 0; i < mats.Length; i++)
        //     if (mats[i].name.Contains("computerScreen"))
        //         mats[i] = blackMat;
        // monitorMeshrenderer.materials = mats;

        yield return StartCoroutine(WaitForMonologue(Monologue.Home_LetsShower));

        player.transform.position = new Vector3(0.4f, 2.27f, -0.12f);
        player.transform.rotation = Quaternion.Euler(0f, 80f, 0f);
        cam.localPosition = new Vector3(0f, 0.54f, 0f);
        cam.localRotation = Quaternion.Euler(14.11f, 0f, 0f);
        yield return new WaitForSeconds(0.5f);

        playerMove.canMove = true;
        playerAnimator.enabled = false;
        player.GetComponent<Collider>().enabled = true;
    }
    
    //================= [ 침대 눕는 애니메이션 ] =================//
    public void Ani_Sleep()
    {
        StartCoroutine(SleepAnimation(cam.transform, player.transform));
    }
    private IEnumerator SleepAnimation(Transform cam, Transform player)
    {
        playerMove.canMove = false;

        player.transform.position = new Vector3(1.2f, 2.27f, -2.2f);
        player.transform.rotation = Quaternion.Euler(90f, 180f, 0f);
        cam.localRotation = Quaternion.Euler(15f, 80f, 0f);

        yield return new WaitForSeconds(5f);

        lampAnimator.Play("lampFallDown");
    }


}
