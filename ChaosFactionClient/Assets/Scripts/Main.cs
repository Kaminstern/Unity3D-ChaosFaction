using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("Move", OnMove);
        NetManager.AddListener("Leave", OnLeave);
        NetManager.Connect("127.0.0.1", 8888);
    }

    // Update is called once per frame
    void Update()
    {
        NetManager.Update();
    }

    void OnEnter(string msg)
    {
        Debug.Log($"OnEnter {msg}");
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
