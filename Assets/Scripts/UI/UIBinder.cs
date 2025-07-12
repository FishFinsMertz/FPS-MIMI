using UnityEngine;

public class UIBinder : MonoBehaviour
{
    [SerializeField] private HUDView hudView;

    private void OnEnable()
    {
        
    }
    public void BindAll(MonoBehaviour[] components)
    {
        foreach (var component in components)
        {
            if (component is IComponent iComponent){
                ViewModel vm = iComponent.CreateAndBindViewModel();
                if (vm != null)
                {
                    hudView.Bind(vm);
                }
            }
            
        }
    }
}