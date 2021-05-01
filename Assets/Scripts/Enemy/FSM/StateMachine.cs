using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine 
{
    public State currentState { get; private set; }

    //������Ʈ �ʱ�ȭ
    public void Initialize(State startingState)
    {
        currentState = startingState;
        startingState.Enter();
    }

    //������Ʈ ����
    public void ChangeState(State newState)
    {
        currentState.Exit();

        currentState = newState;
        newState.Enter();
    }
}
