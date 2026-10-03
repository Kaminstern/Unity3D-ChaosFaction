using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // 人物预设模型
    public GameObject humanPrefab;
    // 人物列表
    public BaseHuman myHuman;
    public Dictionary<string, BaseHuman> otherHumans;
    void Start()
    {
        // 网络模块
        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("Move", OnMove);
        NetManager.AddListener("Leave", OnLeave);
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
        sendStr += eul.y;
        NetManager.Send(sendStr);
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
        BaseHuman bh = gameObject.GetComponent<SyncHuman>();
        bh.desc = desc; ;
        otherHumans.Add(desc, bh);
    }

    void OnMove(string msg)
    {
        Debug.Log($"OnMove {msg}");
    }

    void OnLeave(string msg)
    {
        Debug.Log($"OnLeave {msg}");
    }
}
