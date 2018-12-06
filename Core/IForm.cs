namespace GLIB.Core
{
    using System;
    using UnityEngine;
    using System.Collections;

    public interface IForm
    {
        
        bool IsModuleRunning { get; }

        bool DataSubmitted { get; }

        void StartForm(Action onFormStart);

        void EndForm(Action onFormEnd);

        void SubmitData(Action<string> onDataSubmission);

    }

}