﻿using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*  WaitingRoomScene의 게임오브젝트를 다루는 Manager 객체입니다.
 *  NetRoomPlayer.cs가 이 스크립트의 메소드를 호출합니다
 * 
 */

public class WaitingRoom_MultiGameManager : MonoBehaviour
{
    private NetManager netManager;
    public static WaitingRoom_MultiGameManager instance;
    public PlayerSpace[] playerSpaces = new PlayerSpace[NetManager.PLAYER_MAXNUM];
    public StartButton startButton;


    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        netManager = NetManager.instance;

    }

    //룸플레이어가 들어올경우 룸플레이어를 룸플레이어 공간에 옮기고 롬플레이어 공간은 비활성화
    public void AddPlayerToPlayerSpace(NetRoomPlayer newPlayer)
    {
        
        foreach (PlayerSpace space in playerSpaces)
        {
            if (space.player == null)
            {
                space.player = newPlayer;
                RectTransform playerSpaceRectTrans = space.GetComponentInChildren<RectTransform>();
                if (playerSpaceRectTrans == null)
                {
                    Debug.LogError("PlayerSpace must get Rect Transform! - heeunAn");
                    return;
                }

                space.player.Rect_Trans.localPosition = playerSpaceRectTrans.localPosition;
                space.gameObject.SetActive(false);
                break;
            }
        }
    }
    //룸 플레이어가 나갈시 플레이어 공간을 다시 활성화
    public void RemovePlayerFromPlayerSpace(NetRoomPlayer roomPlayer)
    {
        foreach (PlayerSpace space in playerSpaces)
        {
            if(space.player.gameObject == roomPlayer.gameObject)
            {
                space.gameObject.SetActive(true);
                break;
            }
        }
    }

    //NetRoomPlayer의 방장을 교체함. (순서순)
    public void ReplaceLeader(NetRoomPlayer roomPlayer)
    {
        foreach (NetRoomPlayer player in netManager.roomSlots)
        {
            if(player != roomPlayer && !player.isLeader)
            {
                player.isLeader = true;
                player.setNicknameText();
                AssignLeaderAuthority(player);
                break;
            }
        }
    }

    //StartButtom은 방장에게만 보이게 함.
    public void AssignLeaderAuthority(NetRoomPlayer player)
    {
        startButton.AssignAuthority(player);
    }






}
