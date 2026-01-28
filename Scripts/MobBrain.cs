using UnityEngine;
using UnityEngine.AI;

public class MobBrain : MonoBehaviour
{
    public MobProperty data;
    private MobBaseState activeState;
    public string currentStateName; 
    
    [HideInInspector] public UnityEngine.AI.NavMeshAgent agent;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    void Awake()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (data != null && agent != null)
        {
            agent.speed = data.moveSpeed;
        }
    }

    void Start()
    {
        ChangeState(new StrollingState());
    }

    void Update()
    {
        activeState?.Perform();
    }

    public void ChangeState(MobBaseState newState)
    {
        activeState?.Exit();
        activeState = newState;
        activeState.brain = this;
        currentStateName = newState.GetType().Name;
        activeState.Enter();
    }
}