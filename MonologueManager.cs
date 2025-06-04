using TMPro;
using UnityEngine;

public class MonologueManager : MonoBehaviour
{
    private GameObject monologueBG;
    private TextMeshProUGUI monologue;
    public PlayerMove playerMove;

    private string[] lines;
    private int currentIndex = -1;

    private System.Action onComplete;

    void Start()
    {

    }

    void Awake()
    {
        monologueBG = GameObject.Find("MonologueBG");
        monologue = monologueBG.GetComponentInChildren<TextMeshProUGUI>();
        playerMove = GameObject.Find("Player").GetComponent<PlayerMove>();
    }

    void Update()
    {
        // 대사가 보이는 상태면 스페이스바 눌렀을 때 다음 대사 출력
        if (monologueBG.activeSelf && Input.GetKeyDown(KeyCode.Space)) ShowNextLine();
    }

    // 대사 배열 받아서 보여주기 + 플레이어 조작 비활성화
    public void ShowMonologue(string[] messages, System.Action callback = null)
    {
        if (messages == null || messages.Length == 0)
        {
            Debug.LogWarning("대사 배열이 비어있습니다.");
            return;
        }

        monologueBG.SetActive(true);
        playerMove.canMove = false;

        lines = messages;
        currentIndex = 0;
        onComplete = callback;

        monologue.text = lines[currentIndex];
    }

    // 다음 대사 출력, 마지막 대사면 대사UI 숨기기 실행
    public void ShowNextLine()
    {
        currentIndex++;

        if (currentIndex < lines.Length) monologue.text = lines[currentIndex];
        else
        {
            HideMonologue();
            onComplete?.Invoke(); // 콜백 실행
            onComplete = null; 
        }
    }

    // 대사UI 숨기기 + 플레이어 조작 활성화
    public void HideMonologue()
    {
        monologueBG.SetActive(false); 

        playerMove.canMove = true;

        lines = null;
        currentIndex = -1;
    }
    
    //대사 활성화 상태
    public bool IsMonologueActive()
    {
        return monologueBG.activeSelf;
    }
}
