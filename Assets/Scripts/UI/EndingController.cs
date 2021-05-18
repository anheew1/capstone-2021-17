using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class EndingController : MonoBehaviour
{

    public static EndingController instance;
    public Text[] nickname;
    public Text gameClear;

    public List<EndingPlayerMessage> messages;

    public List<SkinnedMeshRenderer> heads;
    public List<SkinnedMeshRenderer> bodys;
    public List<EndingPlayerManager> endingPlayerManagers;



    private bool isclear; //���� Ŭ���� ����

    void Awake()
    {
        instance = this;

        isclear = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (NetworkManager.singleton is NetManager netManager)
        {
            netManager.EndingController = this;
        }
        else if (NetworkManager.singleton is DebugInGameNetManager debugInGameManager)
        {
            debugInGameManager.EndingManager = this;
        }

        messages = new List<EndingPlayerMessage>();



        UpdatePlayers();
    }

    public void DisconnectRoom()
    {
        if (NetworkManager.singleton)
        {
            NetworkManager.singleton.StopClient(); // �������� ���ư�
        }
    }

    public void UpdatePlayers() // EndingMessage���� �÷��̾� �̸�, ������ �׾����� ���°� ���Ե�.
    {
        if (NetworkManager.singleton is NetManager netManager)
        {
            messages = netManager.EndingMessages;
        }
        else if (NetworkManager.singleton is DebugInGameNetManager debugInGameManager)
        {
            messages = debugInGameManager.EndingMessages;
        }

        //�ٸ� �÷��̾���� ������ Ŭ�����Ұ�� EndingMessage�� NetManager.endingmessage�� �����Ͱ� ���޵�
        ShowPlayers();
        ShowPlayerText();
    }

    private void ShowPlayers() // ���ӵ� �÷��̾�� ������ Mesh�� ���̵��� ��
    {
        for (int id = 0; id < messages.Count; id++)
        {
            heads[id].gameObject.SetActive(true);
            bodys[id].gameObject.SetActive(true);


            if (messages[id].endingState == PlayerEndingState.Dead)
            {
                endingPlayerManagers[id].isDead();
            }
            else
            {
                endingPlayerManagers[id].isLive();
            }


        }
        for (int id = messages.Count; id < 4; id++)
        {
            heads[id].gameObject.SetActive(false);
            bodys[id].gameObject.SetActive(false);
        }
    }








    //ĳ���� �� �ε� �� ĳ���� ���¿� ���� EndingPlayerManager�� islive or lsdead ȣ��

    private void ShowClearText() //���� Ŭ����, ���� ���� ���� ���
    {
        //���� Ŭ�����
        if (isclear)
        {
            gameClear.text = "Game Clear!";
        }
        else //���� ����(���� ���) ��
        {
            gameClear.text = "Game Over...";
        }

    }

    private void ShowPlayerText() //�÷��̾�ĳ�� �г��� ���
    {
        for (int id = 0; id < messages.Count; id++)
        {
            nickname[id].text = messages[id].PlayerName;
        } //���ʿ������� id ������ �г��� ���

        for (int id = messages.Count; id < 4; id++)
        {
            nickname[id].text = "";
        } // ���� �÷��̾���� �г��� ǥ�� ����
    }
}

