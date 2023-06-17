using UnityEngine;

public abstract class State<T> : ScriptableObject where T : MonoBehaviour
{
    protected T _runner;
    
    public virtual void Initialize (T parent) => _runner = parent;
    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void Exit();
}
