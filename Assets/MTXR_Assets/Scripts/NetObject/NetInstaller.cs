using UnityEngine;
using Zenject;
using Megatowel.NetObject;

public class NetInstaller : MonoInstaller<NetInstaller>
{
    public override void InstallBindings()
    {
        Debug.Log("NetObject Installer Running");
        Container.BindInterfacesAndSelfTo<NetManager>().AsSingle().NonLazy();
    }
}