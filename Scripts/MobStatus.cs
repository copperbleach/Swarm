using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class MobStatus : MonoBehaviour
{
    public MobProperty data; 
    public float currentHealth;
    public float currentHunger;
    
    private MobBrain brain;
    private bool isDead = false;
    private bool isFeeding = false; 

    private TimeManager timeManager;

    void Awake()
    {
        brain = GetComponent<MobBrain>();
        timeManager = Object.FindFirstObjectByType<TimeManager>();
        
        if (brain != null && brain.data != null)
        {
            data = brain.data;
        }
    }

    void Start()
    {
        if (data != null)
        {
            currentHealth = data.maxHealth;
            currentHunger = data.foodConsumptionPerDay;
        }
    }

    void Update()
    {
        if (isDead) return;

        // --- 修复点 1: 定义并执行饥饿逻辑 ---
        HandleHungerAndStarvation();

        // --- 修复点 2: 触发进食判定 ---
        if (currentHunger <= data.foodConsumptionPerDay * 0.2f && !isFeeding)
        {
            // 确保只有在闲逛时才去吃饭
            if (brain.currentStateName == "StrollingState") 
            {
                GameObject reachableFood = FindReachableFood();
                if (reachableFood != null)
                {
                    StartFeeding();
                }
            }
        }

        if (currentHealth <= 0) Die();
    }

    // 实现 HandleHungerAndStarvation 函数
    private void HandleHungerAndStarvation()
    {
        if (data == null || timeManager == null) return;

        // 饥饿平滑扣除
        float hungerLoss = (data.foodConsumptionPerDay / timeManager.realSecondsPerGameDay) * Time.deltaTime;
        currentHunger = Mathf.Max(0, currentHunger - hungerLoss);

        // 如果饥饿为0，每帧按比例扣除生命值（每天扣25）
        if (currentHunger <= 0)
        {
            float starvationDamage = (25f / timeManager.realSecondsPerGameDay) * Time.deltaTime;
            TakeDamage(starvationDamage);
        }
    }

    private GameObject FindReachableFood()
    {
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
        if (foods.Length == 0) return null;

        var sortedFoods = foods.OrderBy(f => Vector3.Distance(transform.position, f.transform.position));

        foreach (var food in sortedFoods)
        {
            UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
            // 检查路径是否完整可达
            if (brain.agent.CalculatePath(food.transform.position, path))
            {
                if (path.status == UnityEngine.AI.NavMeshPathStatus.PathComplete)
                {
                    return food; 
                }
            }
        }
        return null;
    }

    public void StartFeeding()
    {
        isFeeding = true;
        brain.ChangeState(new FeedingState());
    }

    public void ResetFeedingStatus() { isFeeding = false; }
    public void FullHealHunger() { currentHunger = data.foodConsumptionPerDay; isFeeding = false; }
    public void TakeDamage(float amount) { currentHealth -= amount; }

    private void Die()
    {
        isDead = true;
        if (brain != null) brain.ChangeState(new DeadState());
    }
}