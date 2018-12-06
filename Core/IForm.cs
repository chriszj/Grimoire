namespace GLIB.Core
{

    using UnityEngine;
    using System.Collections;

    public interface IForm
    {
        bool IsModuleRunning { get; }

        bool DataSubmitted { get; }

        void StartForm();

        void EndForm();

        void SubmitData();

    }

}