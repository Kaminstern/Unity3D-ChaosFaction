using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // 人物预设模型
    public GameObject humanPrefab;
    // 人物列表
    public BaseHuman myHuman;
    public Dictionary<string, BaseHuman> otherHumans;

    private void Awake()
    {
        otherHumans = new Dictionary<string, BaseHuman>();
    }
    void Start()
    {
        // 网络模块
        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("Move", OnMove);
        NetManager.AddListener("Leave", OnLeave);
        NetManager.AddListener("List", OnList);
        NetManager.AddListener("Attack", OnAttack);
        NetManager.AddListener("Die", OnDie);
        NetManager.Connect("127.0.0.1", 8888);

        // 添加一个角色
        GameObject obj = (GameObject)Instantiate(humanPrefab);
        float x = Random.Range(-5, 5);
        float z = Random.Range(-5, 5);
        obj.transform.position = new Vector3(x, 0, z);
        myHuman = obj.AddComponent<CtrlHuman>();            // 这里通过代码的方式挂上了脚本，就不要在编辑器中重复添加脚本
        myHuman.desc = NetManager.GetDesc();

        // 发送协议
        Vector3 pos = myHuman.transform.position;
        Vector3 eul = myHuman.transform.eulerAngles;
        string sendStr = "Enter|";
        sendStr += NetManager.GetDesc() + ",";
        sendStr += pos.x + ",";
        sendStr += pos.y + ",";
        sendStr += pos.z + ",";
        sendStr += eul.y + "|";
        NetManager.Send(sendStr);

        // 请求玩家列表
        NetManager.Send("List||");
    }

    // Update is called once per frame
    void Update()
    {
        NetManager.Update();
    }

    void OnEnter(string msg)
    {
        Debug.Log($"OnEnter {msg}");
        // 解析参数
        string[] split = msg.Split(',');    // 已经在NetManager.Update中做了动作与角色描述和坐标角度的拆分，这里传入的就是剩下的角色描述和坐标角度
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        float eulY = float.Parse(split[4]);
        // 是自己
        if (desc == NetManager.GetDesc())
        {
            return;
        }
        Debug.Log($"自己是{desc}，不是自己：{NetManager.GetDesc()}");
        // 添加一个角色
        GameObject gameObject = (GameObject)Instantiate(humanPrefab);
        gameObject.transform.position = new Vector3(x, y, z);
        gameObject.transform.eulerAngles = new Vector3(0, eulY, 0);
        BaseHuman bh = gameObject.AddComponent<SyncHuman>();
        bh.desc = desc; ;
        otherHumans.Add(desc, bh);
    }

    void OnMove(string msg)
    {
        Debug.Log($"OnMove {msg}");
        // 解析参数
        string[] split = msg.Split(",");
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);

        // 移动
        if (!otherHumans.ContainsKey(desc))
        {
            return;
        }
        BaseHuman h = otherHumans[desc];
        Vector3 targetPos = new Vector3(x, y, z);
        h.MoveTo(targetPos);
    }

    void OnLeave(string msg)
    {
        Debug.Log($"OnLeave {msg}");
        // 解析参数
        string[] split = msg.Split(",");
        string desc = split[0];
        // 删除
        if (!otherHumans.ContainsKey(desc))
        {
            return;
        }
        BaseHuman h = otherHumans[desc];
        Destroy(h.gameObject);          // 销毁h所在的整个GameObject对象
        otherHumans.Remove(desc);
    }

    void OnList(string msgArgs)
    {
        Debug.Log($"OnList {msgArgs}");
        // 解析参数
        string[] splits = msgArgs.Split(",");
        int entityCount = (splits.Length - 1) / 6;         // 减1是因为服务端那边是循环放置各客户端的信息，最后会有一个逗号分割
        for (int i = 0; i < entityCount; i++)
        {
            string desc = splits[i * 6];
            float x = float.Parse(splits[i * 6 + 1]);
            float y = float.Parse(splits[i * 6 + 2]);
            float z = float.Parse(splits[i * 6 + 3]);
            float eulY = float.Parse(splits[i * 6 + 4]);
            int hp = int.Parse(splits[i * 6 + 5]);
            // 是自己
            if (desc == NetManager.GetDesc())
            {
                continue;
            }
            // 添加一个角色
            GameObject gameObject = (GameObject)Instantiate(humanPrefab);
            gameObject.transform.position = new Vector3(x, y, z);
            gameObject.transform.eulerAngles = new Vector3(0, eulY, 0);
            BaseHuman bh = gameObject.AddComponent<SyncHuman>();
            bh.desc = desc; ;
            otherHumans.Add(desc, bh);
        }
    }

    void OnAttack(string msgArgs)
    {
        Debug.Log($"OnAttack {msgArgs}");
        // 解析参数
        string[] split = msgArgs.Split(',');
        string desc = split[0];
        float eulY = float.Parse(split[1]);
        // 攻击动作
        if(!otherHumans.ContainsKey(desc))
        {
            return;
        }
        SyncHuman h = (SyncHuman)otherHumans[desc];
        h.SyncAttack(eulY);
    }

    void OnDie(string msgArgs)
    {
        Debug.Log($"OnDie {msgArgs}");
        // 解析参数
        string[] split = msgArgs.Split(",");
        string hitDesc = split[0];
        // 自己阵亡
        if( hitDesc == NetManager.GetDesc())
        {
            Debug.Log("Game Over");
            return;
        }
        // 死亡判定
        if (!otherHumans.ContainsKey(hitDesc))
        {
            return;
        }
        SyncHuman h = (SyncHuman)(otherHumans[hitDesc]);
        h.gameObject.SetActive(false);
    }
}
