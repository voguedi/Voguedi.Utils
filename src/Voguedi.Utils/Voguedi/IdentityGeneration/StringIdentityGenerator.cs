using System.Linq;

namespace Voguedi.IdentityGeneration
{
    public class StringIdentityGenerator : IStringIdentityGenerator
    {
        #region Private Fields

        readonly ObjectIdentityGenerator generator = new ObjectIdentityGenerator();
        readonly uint[] lookup32 = Enumerable.Range(0, 256).Select(item =>
        {
            var str = item.ToString("x2");
            return str[0] + ((uint)str[1] << 16);
        }).ToArray();

        #endregion

        #region Public Properties

        public static StringIdentityGenerator Instance => new StringIdentityGenerator();

        #endregion

        #region IStringIdentityGenerator

        public string Generate()
        {
            var objectId = generator.Generate();
            var chars = new char[24];

            for (int i = 0; i < 12; i++)
            {
                var val = lookup32[objectId[i]];
                chars[2 * i] = (char)val;
                chars[2 * i + 1] = (char)(val >> 16);
            }

            return new string(chars);
        }

        #endregion
    }
}
