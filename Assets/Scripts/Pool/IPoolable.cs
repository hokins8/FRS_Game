using System;

namespace Tools.UnityUtilities
{
    public interface IPoolable
    {
        void Initialize();
        void OnFree();
        void OnUse();

        bool IsActive { get; set; }
        Action RequestReturn { get; set; }
    }
}
