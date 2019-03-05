using System;

namespace Voguedi.ObjectMapping
{
    public class MapperAttribute : Attribute
    {
        #region Ctors

        public MapperAttribute(params Type[] targetTypes) => TargetTypes = targetTypes;

        #endregion

        #region Public Properties

        public Type[] TargetTypes { get; }

        public bool IsSource { get; set; } = true;

        public bool IsDestination { get; set; } = true;

        #endregion
    }
}
