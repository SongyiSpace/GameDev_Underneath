using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimatorController : MonoBehaviour
{
    private PlayerMove playerMove;
    private GameObject rabbit;
    private GameObject player;
    private Transform cam;
    private ScreenFader screenFader;
    private MonologueManager monoManager;

    [SerializeField] private GameObject food3;

    private float eatTime = 2f;

    private Animator playerAnimator;

    void Start()
    {
        playerMove = GameObject.Find("Player").GetComponent<PlayerMove>();
        rabbit = GameObject.Find("rabbit");
        player = GameObject.Find("Player");
        cam = Camera.main.transform;
        screenFader = FindFirstObjectByType<ScreenFader>();
        monoManager = FindFirstObjectByType<MonologueManager>();

        playerAnimator = player.GetComponent<Animator>();
        playerAnimator.enabled = false;
    }

    //================[ 공통 메소드 ]================//
    private IEnumerator WaitForAnimation(string stateName)
    {
        yield return new WaitUntil(() =>
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
            playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
    }
    private IEnumerator WaitForMonologue(string[] monologueMessage)
    {
        bool isDone = false;
        monoManager.ShowMonologue(monologueMessage, () => isDone = true);
        yield return new WaitUntil(() => isDone);
    }

    //================= [ 토끼인형 줍는 애니메이션 ] =================//
    public void Ani_PickUpRabbit()
    {
        StartCoroutine(PickUpRabbitAnimation(cam.transform, player.transform));
    }

    private IEnumerator PickUpRabbitAnimation(Transform cam, Transform player)
    {
        playerMove.canMove = false;

        // 1. 기존 카메라 roation값 저장
        Quaternion originalCamRot = cam.rotation;
        Quaternion originalPlayerRot = player.rotation;

        // 2. 고개숙이기 (x축:20)
        float t;
        Quaternion camDown = Quaternion.Euler(cam.eulerAngles.x + 20f, cam.eulerAngles.y, cam.eulerAngles.z);

        t = 0;
        while (t < 1.5f)
        {
            cam.rotation = Quaternion.Slerp(originalCamRot, camDown, t / 1.5f);
            t += Time.deltaTime;
            yield return null;
        }
        rabbit.SetActive(false);

        // 3. 고개 들기
        t = 0;
        while (t < 1.5f)
        {
            cam.rotation = Quaternion.Slerp(camDown, originalCamRot, t / 1.5f);
            t += Time.deltaTime;
            yield return null;
        }
        cam.rotation = originalCamRot;


        // 4. 정면 쳐다보기
        Quaternion playerLookFront = Quaternion.Euler(0f, 0f, 0f);
        Quaternion camLookFront = Quaternion.Euler(7f, 0f, 0f);

        t = 0;
        while (t < 1.5f)
        {
            player.rotation = Quaternion.Lerp(originalPlayerRot, playerLookFront, t / 1.5f);
            cam.rotation = Quaternion.Lerp(originalCamRot, camLookFront, t / 1.5f);
            t += Time.deltaTime;
            yield return null;
        }

        //5. 직진
        Vector3 originalPlayerPos = player.position;
        Vector3 targetPos = originalPlayerPos + player.forward * 10f;

        screenFader.StartFadeOut(3f);

        t = 0;
        while (t < 5f)
        {
            player.position = Vector3.Lerp(originalPlayerPos, targetPos, t / 5f);
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
        //밥먹는 소리 클립 10초
        yield return new WaitForSeconds(eatTime);

        food3.transform.Find("Fork").gameObject.SetActive(true);

        yield return WaitForMonologue(Monologue.Home_DoneEat);

        food3.SetActive(false);

        //상 앞에서 일어난 시점
        player.position = new Vector3(0.33f, 2.27f, -1.3f);
        player.rotation = Quaternion.Euler(0f, 180f, 0f);
        cam.localPosition = new Vector3(0f, 0.543f, 0f);
        cam.localRotation = Quaternion.Euler(0f, 0f, 0f);

        playerMove.canMove = true;

        monoManager.ShowMonologue(Monologue.Home_LetsWork); //수정필요
    }

    //================= [ 일하는 애니메이션 ] =================//
    public void Ani_Study()
    {
        StartCoroutine(StudyAnimation(cam.transform, player.transform));
    }
    private IEnumerator StudyAnimation(Transform cam, Transform player)
    {
        playerMove.canMove = false;

        // 1. 정면보기
        player.transform.position = new Vector3(1.1f, 2.27f, 0.28f);
        player.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        cam.localPosition = new Vector3(0f, 0.42f, 0f);
        cam.localRotation = Quaternion.Euler(15f, 0f, 0f);

        playerAnimator.enabled = true;

        playerAnimator.Play("PlayerStayOrigin");
        yield return new WaitForSeconds(1f);

        // 2. 오른쪽 보기 애니메이션
        playerAnimator.SetBool("focusSound", true);
        yield return WaitForAnimation("PlayerLookRight");

        // 3. 모놀로그 : ?
        yield return StartCoroutine(WaitForMonologue(Monologue.Doubt));

        // 4. 정면 보기 애니메이션
        playerAnimator.SetBool("focusSound", false);
        yield return WaitForAnimation("PlayerLookOrigin");

        // 5. 정면 유지
        playerAnimator.Play("PlayerStayOrigin");
        yield return new WaitForSeconds(1f);

        // 6. 오른쪽 보기 애니메이션
        playerAnimator.SetBool("focusSound", true);
        yield return WaitForAnimation("PlayerLookRight");

        // 7. 오른쪽 본 채로 1초 대기
        yield return new WaitForSeconds(1f);

        // 8. 정면 보기 애니메이션
        playerAnimator.SetBool("focusSound", false);
        yield return WaitForAnimation("PlayerLookOrigin");

        // 9. 모놀로그
        yield return StartCoroutine(WaitForMonologue(Monologue.Home_LetsShower));

        //10. 일어나고 플레이어 조작 통제 해제
        player.transform.position = new Vector3(0.4f, 2.27f, -0.12f);
        player.transform.rotation = Quaternion.Euler(0f, 80f, 0f);
        cam.localPosition = new Vector3(0f, 0.54f, 0f);
        cam.localRotation = Quaternion.Euler(14.11f, 0f, 0f);
        yield return new WaitForSeconds(0.5f);

        playerMove.canMove = true;
        playerAnimator.enabled = false;
        player.GetComponent<Collider>().enabled = true;
    }


    

}
