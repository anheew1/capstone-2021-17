using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using keypadSystem;

public class MissionController : MonoBehaviour
{
    [SerializeField] private GameObject missionObject;
    [SerializeField] private Mission1Controller mission1Controller;
    [SerializeField] private Mission2Controller mission2Controller;
    void Awake()
    {
        missionObject = transform.parent.gameObject;
        if(missionObject.name == "Mission1(Clone)")
        {
            mission1Controller = gameObject.GetComponent<Mission1Controller>();
        }
        if (missionObject.name == "Mission2(Clone)")
        {
            mission2Controller = gameObject.GetComponent<Mission2Controller>();
        }
    }

    //�̼ǿ�����Ʈ ��ȣ�ۿ� ����
    public void UnableMission()
    {
        if (missionObject.name == "Mission1(Clone)")
        {
            mission1Controller.UnableMission();
        }
        if (missionObject.name == "Mission2(Clone)")
        {
            mission2Controller.UnableMission();
        }
    }

    //�̼�â Ȱ��ȭ
    public void ShowMission()
    {
        if (missionObject.name == "Mission1(Clone)")
        {
            mission1Controller.ShowMission();
        }
        if (missionObject.name == "Mission2(Clone)")
        {
            mission2Controller.ShowMission();
        }
    }

    //�̼�â ��Ȱ��ȭ
    public void CloseMission()
    {
        if (missionObject.name == "Mission1(Clone)")
        {
            mission1Controller.CloseMission();
        }
        if (missionObject.name == "Mission2(Clone)")
        {
            mission2Controller.CloseMission();
        }
    }
}
