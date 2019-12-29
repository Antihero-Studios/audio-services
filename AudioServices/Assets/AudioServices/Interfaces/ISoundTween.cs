using System;

namespace AudioServices.Interfaces
{
    public interface ISoundTween
    {
        event EventHandler Completed;

        void OnStart(ISound sound);
        void OnUpdate(ISound sound);
    }
}