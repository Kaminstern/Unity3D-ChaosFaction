using UnityEngine;

public class CtrlHuman : BaseHuman
{
    new void Start()
    {
        base.Start();
    }

    new void Update()
    {
        base.Update();

        // 移动
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log($"frame = {Time.frameCount}, instance={GetEntityId()}, go={gameObject.name}, time={Time.realtimeSinceStartup}");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if (hit.collider.tag == "Terrain")
            {
                MoveTo(hit.point);
                // 发送协议
                string sendStr = "Move|";
                sendStr += $"{NetManager.GetDesc()},";
                sendStr += $"{hit.point.x},";
                sendStr += $"{hit.point.y},";
                sendStr += $"{hit.point.z},";

                sendStr += "|";     // 重新定义了协议，末尾以|作为分隔，防止粘包
                NetManager.Send(sendStr);
            }
        }

        // 攻击
        if (Input.GetMouseButtonDown(1))
        {
            if (!isAttacking && !isMoving)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);

                transform.LookAt(hit.point);
                Attack();
                // 发送协议
                string sendStr = "Attack|";
                sendStr += $"{NetManager.GetDesc()},";
                sendStr += $"{transform.eulerAngles.y},";

                sendStr += "|";     // 重新定义了协议，末尾以|作为分隔，防止粘包

                NetManager.Send(sendStr);
            }
        }
    }
}
