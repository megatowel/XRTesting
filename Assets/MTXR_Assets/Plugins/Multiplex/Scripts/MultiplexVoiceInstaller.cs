using UnityEngine;
using Megatowel.Multiplex;
using Megatowel.Debugging;
using Zenject;

public class MultiplexVoiceInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        MTDebug.Log("Running Multiplex Voice Installer...");
        Container.BindInterfacesAndSelfTo<MultiplexVoice>().AsSingle().NonLazy();
    }
}