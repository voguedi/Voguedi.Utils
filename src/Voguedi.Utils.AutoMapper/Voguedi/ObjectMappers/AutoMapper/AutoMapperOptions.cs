using System;
using System.Collections.Generic;
using System.Reflection;
using IAutoMapperMapperConfigurationExpression = AutoMapper.IMapperConfigurationExpression;

namespace Voguedi.AutoMapper
{
    public class AutoMapperOptions
    {
        #region Public Properties

        public List<Action<IAutoMapperMapperConfigurationExpression>> MapConfigs { get; } = new List<Action<IAutoMapperMapperConfigurationExpression>>();

        public Assembly[] Assemblies { get; set; }

        #endregion
    }
}
