using UnityEngine;
using Zenject;
using Megatowel.NetObject;

public class NetInstaller : MonoInstaller<NetInstaller>
{
    public override void InstallBindings()
    {
        Debug.Log("<color=#8C8940>[NetObject]:</color> Running Installer...");
        Container.BindInterfacesAndSelfTo<NetManager>().AsSingle().NonLazy();
    }
}