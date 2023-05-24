using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ���µ��� ��ӽ�ų �θ�Ŭ���� ����
public abstract class StateBase : MonoBehaviour
{
    public abstract void SetUp();
    public abstract void Enter();       // ����
    public abstract void Update();      // ����
    public abstract void Exit();        // Ż��
}
