using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool isStart;

    private int pointIdx = -1;
    private Transform point;
    private List<BoxCollider> points = new List<BoxCollider>();

    [SerializeField] private TextMeshProUGUI countDownText;

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
        Application.targetFrameRate = 60;
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene", UnityEngine.SceneManagement.LoadSceneMode.Additive);

        GameObject point = GameObject.Find("Points");
        if (point != null)
        {
            this.point = point.transform;
            pointIdx = -1;

            point.GetComponentsInChildren(points);
        }

        StartCoroutine(CountDown());
    }

    public void Init()
    {
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

        if (a.transform == points[pointIdx + 1].transform)
        {
            point = a.transform;
            pointIdx++;
        }
    }

    public void ReturnPoint(Transform car)
    {
        if (pointIdx < 0)
        {
            car.SetPositionAndRotation(point.position, Quaternion.Euler(0, point.position.y, 0));
        }
        else
        {
            Transform tr = points[pointIdx].transform;
            car.SetPositionAndRotation(tr.position, Quaternion.Euler(0, tr.position.y, 0));
        }
    }

    private void GameEnd()
    {

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
