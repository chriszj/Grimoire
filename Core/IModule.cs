namespace GLIB.Core
{

    using UnityEngine;
    using System.Collections;

    public interface IModule
    {
        bool IsModuleRunning { get; }

        void StartModule();

        void EndModule();
    }

}