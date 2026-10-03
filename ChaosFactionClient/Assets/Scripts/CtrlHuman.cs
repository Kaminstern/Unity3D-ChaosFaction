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

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log($"frame = {Time.frameCount}, instance={GetEntityId()}, go={gameObject.name}, time={Time.realtimeSinceStartup}");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if (hit.collider.tag == "Terrain")
            {
                MoveTo(hit.point);
                NetManager.Send($"Move|{NetManager.GetDesc()},{hit.point.x},{hit.point.y},{hit.point.z},{transform.eulerAngles}");
            }
        }
    }
}
