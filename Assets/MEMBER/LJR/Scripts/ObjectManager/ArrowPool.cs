using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject arrowPrefab;    // 화살 프리팹
    [SerializeField] private int poolSize = 20;         // 풀 크기
    [SerializeField] private bool expandPool = true;    // 풀이 부족할 때 확장할지 여부

    private Queue<GameObject> arrowPool;
    [SerializeField] private List<GameObject> activeArrows;

    public static ArrowPool Instance { get; private set; }

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePool()
    {
        arrowPool = new Queue<GameObject>();
        activeArrows = new List<GameObject>();

        // 풀에 화살 객체들을 미리 생성
        for (int i = 0; i < poolSize; i++)
        {
            GameObject arrow = CreateNewArrow();
            arrowPool.Enqueue(arrow);
        }
    }

    private GameObject CreateNewArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, transform);
        arrow.SetActive(false);
        return arrow;
    }

    public GameObject GetArrow(Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject arrow;

        // 풀에 사용 가능한 화살이 있는지 확인
        if (arrowPool.Count > 0)
        {
            arrow = arrowPool.Dequeue();
        }
        else if (expandPool)
        {
            // 풀이 비어있고 확장이 가능하다면 새로운 화살 생성
            arrow = CreateNewArrow();
        }
        else
        {
            // 풀이 비어있고 확장이 불가능하다면 null 반환
            return null;
        }

        // 화살 설정
        arrow.transform.position = position;
        arrow.transform.rotation = rotation;
        arrow.transform.SetParent(parent);
        arrow.SetActive(true);

        // 화살 컨트롤러 초기화
        ArrowController arrowController = arrow.GetComponent<ArrowController>();
        if (arrowController != null)
        {
            arrowController.ResetArrow(); // 화살 상태 초기화
        }
        ReturnArrowAfterDelay(arrow, 5f); // 5초 후 자동 회수
        activeArrows.Add(arrow);
        return arrow;
    }

    public void ReturnArrow(GameObject arrow)
    {
        if (arrow == null)
            return;

        // 활성 화살 리스트에서 제거
        if (activeArrows.Contains(arrow))
        {
            activeArrows.Remove(arrow);
        }

        // 화살 비활성화 및 초기화
        arrow.SetActive(false);
        arrow.transform.SetParent(transform);

        // 화살 컨트롤러 초기화
        arrow.GetComponent<ArrowController>().ResetArrow(); 

        // 풀로 반환
        arrowPool.Enqueue(arrow);
    }

    public void ReturnAllArrows()
    {
        // activeArrows를 복사해서 순회 (컬렉션 수정 중 순회 문제 방지)
        List<GameObject> arrowsToReturn = new List<GameObject>(activeArrows);

        foreach (GameObject arrow in arrowsToReturn)
        {
            ReturnArrow(arrow);
        }
    }

    // 일정 시간 후 회수
    public void ReturnArrowAfterDelay(GameObject arrow, float delay)
    {
        if (arrow != null)
        {
            StartCoroutine(ReturnArrowCoroutine(arrow, delay));
        }
    }

    // 코루틴을 사용하여 일정 시간 후 화살을 회수
    private IEnumerator ReturnArrowCoroutine(GameObject arrow, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnArrow(arrow);
    }
}
