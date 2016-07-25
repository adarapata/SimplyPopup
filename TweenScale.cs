using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Events;
using System;

[Serializable]
public class TweenScale
{
    public Vector3 from, to;
    public float duration;
    public AnimationCurve curve = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 1));
    public UnityEvent onBegin, onEnd;
    private GameObject target;

    private Subject<Unit> scaleStartStream = new Subject<Unit> ();

    public IObservable<Unit> scaleStartAsObservable { get { return scaleStartStream.AsObservable (); } }

    private Subject<Unit> scaleEndStream = new Subject<Unit> ();

    public IObservable<Unit> scaleEndAsObservable { get { return scaleEndStream.AsObservable (); } }

    public void Setup (GameObject t)
    {
        target = t;
        scaleStartAsObservable.Subscribe (_ => onBegin.Invoke ());
        scaleEndAsObservable.Subscribe (_ => onEnd.Invoke ());
        scaleEndAsObservable.Subscribe (_ => target.transform.localScale = Vector3.Lerp (from, to, curve.Evaluate (1.0F)));
    }

    public void Play ()
    {
        scaleStartStream.OnNext (Unit.Default);
        Observable.EveryFixedUpdate ()
			.Take (System.TimeSpan.FromSeconds (duration))
			.Select (_ => Time.fixedDeltaTime)
			.Scan ((acc, current) => acc + current)
			.Subscribe (time => {
            float t = time / duration;
            target.transform.localScale = Vector3.Lerp (from, to, curve.Evaluate (t));
        },
            _ => {
            },
            () => scaleEndStream.OnNext (Unit.Default)
        ).AddTo (target);
    }
}
