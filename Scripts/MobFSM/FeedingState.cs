using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class FeedingState : MobBaseState
{
    private GameObject targetFood;
    private float eatingTimer = 5f;
    private bool isEating = false;

    public override void Enter()
    {
        // 再次获取最近的可达食物（此时基本确定能找到）
        targetFood = FindClosestReachableFood(); 

        if (targetFood != null)
        {
            brain.agent.SetDestination(targetFood.transform.position);
            brain.agent.isStopped = false;
        }
        else
        {
            // 极其罕见的情况（比如刚要吃的时候食物被删了），退回
            AbortFeeding();
        }
    }

    private void AbortFeeding()
    {
        // 告诉 Status 停止进食锁定，并切回闲逛
        brain.GetComponent<MobStatus>().ResetFeedingStatus();
        brain.ChangeState(new StrollingState());
    }

    public override void Perform()
    {
        if (targetFood == null) return;

        // 检查是否到达（使用 agent 的 stoppingDistance）
        if (!isEating && !brain.agent.pathPending && brain.agent.remainingDistance <= brain.agent.stoppingDistance + 0.1f)
        {
            isEating = true;
            brain.agent.isStopped = true;
        }

        if (isEating)
        {
            eatingTimer -= Time.deltaTime;
            if (eatingTimer <= 0) FinishEating();
        }
    }



    private GameObject FindClosestReachableFood()
    {
        // 这里可以直接复用 MobStatus 里的逻辑或重新搜一遍
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
        return foods
            .Where(f => {
                NavMeshPath p = new NavMeshPath();
                return brain.agent.CalculatePath(f.transform.position, p) && p.status == NavMeshPathStatus.PathComplete;
            })
            .OrderBy(f => Vector3.Distance(brain.transform.position, f.transform.position))
            .FirstOrDefault();
    }

    private void FinishEating()
    {
        // 补满饥饿值并回到闲逛
        MobStatus status = brain.GetComponent<MobStatus>();
        status.FullHealHunger();
        
        brain.agent.isStopped = false;
        brain.ChangeState(new StrollingState());
    }

    public override void Exit()
    {
        if(brain.agent != null) brain.agent.isStopped = false;
    }
}