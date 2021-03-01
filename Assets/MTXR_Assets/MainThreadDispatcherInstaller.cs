using UnityEngine;
using Megatowel.Debugging;
using Zenject;

public class MainThreadDispatcherInstaller : MonoInstaller<MainThreadDispatcherInstaller>
{
    public override void InstallBindings()
    {
        MTDebug.Log("Main Thread Dispatcher Installer Running");
        Container.BindInterfacesAndSelfTo<UnityMainThreadDispatcher>().AsSingle().NonLazy();

    }
}