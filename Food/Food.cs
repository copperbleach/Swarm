using UnityEngine;

public class Food : MonoBehaviour
{
    [Header("贴图配置")]
    [Tooltip("1-10点时显示的贴图")]
    [SerializeField] private Sprite food10Sprite; 
    [Tooltip("11-50点时显示的贴图")]
    [SerializeField] private Sprite food50Sprite; 

    [Header("当前数值")]
    [SerializeField] private int currentPoints = 50; // 默认满额50点

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisuals();
    }

    /// <summary>
    /// 当被 Mob 吃掉时调用此方法
    /// </summary>
    /// <param name="biteSize">Mob 这一口吃掉的点数，例如 5</param>
    public void BeEaten(int biteSize)
    {
        // 1. 确保 biteSize 是正数，避免反向加血的 Bug
        if (biteSize <= 0) return;

        // 2. 扣除对应的点数
        currentPoints -= biteSize;
        
        Debug.Log($"食物被吃掉了 {biteSize} 点，剩余: {currentPoints} 点");

        // 3. 状态检查
        if (currentPoints <= 0)
        {
            // 如果点数小于等于0，执行销毁
            ExecuteDestruction();
        }
        else
        {
            // 如果还有剩余，更新视觉效果
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        if (spriteRenderer == null) return;

        // 根据你的要求：
        // 11-50点 -> Food50
        // 1-10点  -> Food10
        if (currentPoints > 10)
        {
            spriteRenderer.sprite = food50Sprite;
        }
        else
        {
            spriteRenderer.sprite = food10Sprite;
        }
    }

    private void ExecuteDestruction()
    {
        // 这里可以添加粒子效果，比如“食物碎屑”
        Debug.Log("食物已被吃完，物体销毁。");
        Destroy(this.gameObject);
    }
}