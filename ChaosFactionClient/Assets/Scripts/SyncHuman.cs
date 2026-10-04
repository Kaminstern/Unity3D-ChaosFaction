using UnityEngine;

public class SyncHuman : BaseHuman
{
    new void Start()
    {
        base.Start();
    }

    new void Update()
    {
        base.Update();
    }

    // 同步攻击动作
    public void SyncAttack(float eulY)
    {
        transform.eulerAngles = new Vector3(0,eulY, 0);
        Attack();
    }
}
