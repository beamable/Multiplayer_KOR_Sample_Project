using Unity.Entities;
using UnityEngine;

/// <summary>
/// Calls DefaultWorldInitialization.Initialize() if UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOTSTRAP is enabled.
/// Prevents the world from initializing before the scene catalog is loaded.
/// </summary>
public class WorldInitializer : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOTSTRAP
        World.DefaultGameObjectInjectionWorld = DefaultWorldInitialization.Initialize("Default World");
#endif  
    }
}