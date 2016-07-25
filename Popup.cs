using UnityEngine;
using System.Collections;
using UniRx;

public class Popup : MonoBehaviour
{
    public enum State
    {
        Open,
        Close,
        UnUsed
    }

    public State state { get; private set; }

    public TweenScale open, close;

    void Start ()
    {
        open.Setup (gameObject);
        open.scaleEndAsObservable.Subscribe (_ => state = State.Open);
        close.Setup (gameObject);
        close.scaleEndAsObservable.Subscribe (_ => state = State.Close);
    }

    public void Open ()
    {
        open.Play ();
    }

    public void Close ()
    {
        close.Play ();
    }

    public void Toggle ()
    {
        switch (state) {
        case State.UnUsed:
        case State.Close:
            open.Play ();
            break;
        case State.Open:
            close.Play ();
            break;
        }
    }
}
