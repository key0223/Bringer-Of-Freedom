
using System;
using System.Collections;
using UnityEngine;
using static Define;

public abstract class QTEBase
{
    public string qteName;
    public QTEType qteType;
    public float duration;
    public string successAction;
    public string failureAction;

    public event Action OnSuccess;
    public event Action OnFail;

    public virtual void InvokeSuccess()
    {
        OnSuccess?.Invoke();
    }
    public virtual void InvokeFail ()
    {
        OnFail?.Invoke();
    }

    public abstract IEnumerator CoExecute();
}
