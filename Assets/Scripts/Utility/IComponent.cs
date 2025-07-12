using UnityEngine;

public interface IComponent
{
    public void Initialize(LocalEventBusManager localEventBusManager);
    public virtual ViewModel CreateAndBindViewModel(){
        return null;
    }
}
