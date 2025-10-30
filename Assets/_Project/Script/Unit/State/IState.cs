using UnityEngine;
public interface IState<T>
{
    void OperateEnter(T sender);
    
    void OperateUpdate();
    
    void OperateExit();
    
}
