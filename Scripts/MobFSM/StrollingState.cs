using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class StrollingState : MobBaseState
{
    private NavMeshAgent agent;
    private float waitTimer;
    private Tilemap tilemap;
    
    private Vector3 reachableCenter;
    private float roamRadius = 6f; 
    private float stuckCheckTimer;

    private Queue<Vector3> orthogonalPath = new Queue<Vector3>();

    public override void Enter()
    {
        // 1. 先从 brain 中同步已经获取好的组件引用
        agent = brain.agent; 
        tilemap = Object.FindFirstObjectByType<Tilemap>();

        // 2. 检查确保 brain 里的 data 和 agent 都不是空的
        if (brain.data != null && agent != null)
        {
            // 这里的赋值才安全
            agent.speed = brain.data.moveSpeed; 
            
            // 锁定 Steering 设置，防止滑步
            agent.acceleration = 1000f; 
            agent.angularSpeed = 0f;
            agent.autoBraking = false;
        }
        else
        {
            Debug.LogError("MobStrollingState: Brain Data 或 Agent 为空！");
            return;
        }

        // 3. 基础配置
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // 4. Z轴安全检查：确保棋子在 NavMesh 上且 Z=0
        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(brain.transform.position, out NavMeshHit hit, 3.0f, NavMesh.AllAreas))
            {
                Vector3 warpPos = hit.position;
                warpPos.z = 0; 
                agent.Warp(warpPos);
            }
        }

        if (agent.isOnNavMesh) 
        {
            reachableCenter = agent.transform.position;
            reachableCenter.z = 0;
        }

        PickNewTarget();
    }

    public override void Perform()
    {
        if (agent == null || !agent.isOnNavMesh) return;

        // 检查当前小段路程是否走完
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
        {
            if (orthogonalPath.Count > 0)
            {
                // 走下一段直角路径
                agent.SetDestination(orthogonalPath.Dequeue());
            }
            else
            {
                // 整条路走完了，开始计时等待下一次游荡
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0) PickNewTarget();
            }
        }

        // 卡死检测
        HandleStuckDetection();
    }

    private void PickNewTarget()
    {
        if (agent == null || !agent.isOnNavMesh || tilemap == null) return;

        for (int i = 0; i < 30; i++)
        {
            Vector2 randomCirclePoint = Random.insideUnitCircle * roamRadius;
            Vector3 randomPos = reachableCenter + new Vector3(randomCirclePoint.x, randomCirclePoint.y, 0);

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                Vector3Int cellPos = tilemap.WorldToCell(hit.position);
                Vector3 snappedCenter = tilemap.GetCellCenterWorld(cellPos);
                snappedCenter.z = 0; // 确保目标点在 2D 平面上
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(snappedCenter, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    // --- 核心：重构为直角路径 ---
                    ReconstructPathToOrthogonal(path.corners);
                    waitTimer = Random.Range(2f, 5f);
                    return;
                }
            }
        }
        waitTimer = 1f;
    }

    private void ReconstructPathToOrthogonal(Vector3[] corners)
    {
        orthogonalPath.Clear();
        if (corners.Length < 2) return;

        Vector3 currentPos = corners[0];

        for (int i = 1; i < corners.Length; i++)
        {
            Vector3 nextCorner = corners[i];

            // 拆解 X 和 Y 的移动
            // 先移动 X 轴
            if (Mathf.Abs(nextCorner.x - currentPos.x) > 0.01f)
            {
                currentPos = new Vector3(nextCorner.x, currentPos.y, currentPos.z);
                orthogonalPath.Enqueue(currentPos);
            }

            // 再移动 Y 轴
            if (Mathf.Abs(nextCorner.y - currentPos.y) > 0.01f)
            {
                currentPos = new Vector3(currentPos.x, nextCorner.y, currentPos.z);
                orthogonalPath.Enqueue(currentPos);
            }
        }

        // 启动第一段移动
        if (orthogonalPath.Count > 0)
        {
            agent.SetDestination(orthogonalPath.Dequeue());
        }
    }

    private void HandleStuckDetection()
    {
        if (agent.hasPath && agent.velocity.sqrMagnitude < 0.01f)
        {
            stuckCheckTimer += Time.deltaTime;
            if (stuckCheckTimer > 1.5f)
            {
                stuckCheckTimer = 0;
                orthogonalPath.Clear();
                PickNewTarget();
            }
        }
        else stuckCheckTimer = 0;
    }

    public override void Exit()
    {
        if (agent != null && agent.isOnNavMesh) agent.ResetPath();
        orthogonalPath.Clear();
    }
}