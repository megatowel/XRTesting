using Megatowel.Debugging;
using Megatowel.Multiplex;
using UnityEngine;
using Zenject;

public class MultiplexInstaller : MonoInstaller<MultiplexInstaller>
{
    public override void InstallBindings()
    {
        MTDebug.Log("Multiplex Installer Running");
        Container.BindInterfacesAndSelfTo<MultiplexManager>().AsSingle().NonLazy();
    }
}