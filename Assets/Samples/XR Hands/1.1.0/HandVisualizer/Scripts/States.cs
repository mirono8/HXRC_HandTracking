using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class States : MonoBehaviour
{
	public enum State
	{
		Paused, Active, End
	}

	[SerializeField]
	State currentState = new();

    private void Awake()
    {
        currentState = State.Paused;
    }

	protected void ChangeState(State s)
	{
		currentState = s;
	}

    public State CurrentState()
	{
		return currentState;
	}
}
