using UnityEngine;
using TMPro; // 如果你用的是 Text Mesh Pro

public class TimeManager : MonoBehaviour
{
    [Header("Settings")]
    public float realSecondsPerGameDay = 300f; // 5分钟
    public TextMeshProUGUI dayText; // 拖入你的 Text(TMP) 组件

    private float timer;
    private int currentDay = 1;

    void Start()
    {
        // 游戏启动时立即刷新一次 UI
        UpdateUI();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= realSecondsPerGameDay)
        {
            timer = 0;
            currentDay++;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (dayText != null)
        {
            // 这里决定了屏幕上显示的最终样子
            dayText.text = $"Day {currentDay}";
        }
    }
}