using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.Events;

public class EndingManager : MonoBehaviour
{

    public static EndingManager instance;
    public Text[] nickname;
    public Text gameClear;

    public List<EndingPlayerManager> endingPlayerManagers;

    public List<NetGamePlayer> players;

    [SerializeField]
    private GameObject EndingSceneObject;

    private List<SkinnedMeshRenderer> heads;
    private List<SkinnedMeshRenderer> bodys;

    public UnityEvent OnChangeEndingSceneObject;


    private bool isclear; //���� Ŭ���� ����

    void Awake()
    {
        instance = this;

        isclear = true;

        heads = new List<SkinnedMeshRenderer>();
        bodys = new List<SkinnedMeshRenderer>();

        foreach (var player in endingPlayerManagers)
        { // �÷��̾ ������ ���߱� ���� �÷��̾���� Mesh�� �̿���
            heads.Add(player.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>());
            bodys.Add(player.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>());
        }
    }

    public void DisconnectRoom()
    {
        if (NetworkManager.singleton)
        {
            NetworkManager.singleton.StopClient(); // �������� ���ư�
        }
    }

    public void StartEnding()
    {
        EndingSceneObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update() // EndingMessage���� �÷��̾� �̸�, ������ �׾����� ���°� ���Ե�.
    {
        if (!EndingSceneObject.activeSelf)
        {
            return;
        }
                //�ٸ� �÷��̾���� ������ Ŭ�����Ұ�� EndingMessage�� NetManager.endingmessage�� �����Ͱ� ���޵�
        ShowPlayers();
        ShowPlayerText();
    }

    private void ShowPlayers() // ���ӵ� �÷��̾�� ������ Mesh�� ���̵��� ��
    {
        for (int id = 0; id < players.Count; id++)
        {
            heads[id].gameObject.SetActive(true);
            bodys[id].gameObject.SetActive(true);


            if (players[id].EndState == PlayerEndingState.Escape)
            {
                endingPlayerManagers[id].isLive();
            }
            else
            {
                endingPlayerManagers[id].isDead();
            }


        }
        for (int id = players.Count; id < 4; id++)
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
        for (int id = 0; id < players.Count; id++)
        {
            nickname[id].text = players[id].Nickname;
        } //���ʿ������� id ������ �г��� ���

        for (int id = players.Count; id < 4; id++)
        {
            nickname[id].text = "";
        } // ���� �÷��̾���� �г��� ǥ�� ����
    }
}

