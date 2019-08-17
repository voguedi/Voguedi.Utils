using System;

namespace Voguedi.ObjectMappers
{
    public class ObjectMapperAttribute : Attribute
    {
        #region Ctors

        public ObjectMapperAttribute(params Type[] targetTypes) => TargetTypes = targetTypes;

        #endregion

        #region Public Properties

        public Type[] TargetTypes { get; }

        public bool IsSource { get; set; } = true;

        public bool IsDestination { get; set; } = true;

        #endregion
    }
}
