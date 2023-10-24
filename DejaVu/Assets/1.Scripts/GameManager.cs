using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool isStart;
    public static bool isEnd;

    private int pointIdx = -1;
    private Transform point;
    private List<BoxCollider> points = new List<BoxCollider>();

    [SerializeField] private TextMeshProUGUI countDownText;
    [SerializeField] private TextMeshProUGUI timerText;
    private float timer;

    [SerializeField] private GameObject endCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //최대 프레임 안 정해주면 FixedUpdate로도 보정이 잘 안 될 정도로 프레임이 겁나 튐
        Application.targetFrameRate = 60;
        SceneManager.LoadScene("Car", LoadSceneMode.Additive);

        isStart = isEnd = false;

        GameObject point = GameObject.Find("Points");
        if (point != null)
        {
            this.point = point.transform;
            pointIdx = -1;

            //points = BoxColliders[]
            point.GetComponentsInChildren(points);
            points.RemoveAt(0);
        }

        StartCoroutine(CountDown());
    }

    private void Update()
    {
        if (isStart && !isEnd)
        {
            timer += Time.deltaTime;
            timerText.text = $"{(int)timer / 60:00}:{(int)timer % 60:00}";
        }
    }

    public void SetPoint(Collider a)
    {
        if (pointIdx >= points.Count - 1)
        {
            if (a.name == "Points")
            {
                GameEnd();
            }
            return;
        }

        if (a == points[pointIdx + 1])
        {
            point = a.transform;
            pointIdx++;
        }
    }

    public void ReturnPoint(Transform car)
    {
        if (pointIdx < 0)
        {
            car.SetPositionAndRotation(point.position, point.rotation);
        }
        else
        {
            Transform tr = points[pointIdx].transform;
            car.SetPositionAndRotation(tr.position, tr.rotation);
        }
    }

    private void GameEnd()
    {
        isEnd = true;
        AudioManager.Instance.StopBGM();
        timerText.transform.localPosition = Vector3.zero;
        endCanvas.SetActive(true);
    }

    public void ReturnToMain()
    {
        endCanvas.SetActive(false);
        SceneManager.LoadScene("Main");
    }

    private IEnumerator CountDown()
    {
        int count = 3;
        while (!isStart)
        {
            if (count > 0)
            {
                countDownText.text = count.ToString();
                yield return new WaitForSeconds(1f);
                count--;
            }
            else
            {
                isStart = true;
                countDownText.gameObject.SetActive(false);
            }
        }
    }
}
