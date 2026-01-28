using UnityEngine;

public class DeadState : MobBaseState
{
    public override void Enter()
    {
        // 停止导航代理，确保不再移动
        if (brain.agent != null && brain.agent.isOnNavMesh)
        {
            brain.agent.isStopped = true; 
            brain.agent.ResetPath();
        }

        // 可以在这里播放死亡动画或改变颜色
        if (brain.spriteRenderer != null)
        {
            brain.spriteRenderer.color = Color.gray; // 变灰表示死亡
        }

        Debug.Log($"{brain.gameObject.name} 已死亡。");
    }

    public override void Perform()
    {

    }

    public override void Exit()
    {

    }
}