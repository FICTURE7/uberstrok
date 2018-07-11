using System;

namespace UberStrok
{
    public abstract class DataComponent : Component
    {
        protected sealed override void OnUpdate()
        {
            /* DataComponent holds data only and does not need to be updated. */
        }
    }
}
