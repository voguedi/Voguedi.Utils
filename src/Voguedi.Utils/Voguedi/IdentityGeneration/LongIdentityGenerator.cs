namespace Voguedi.IdentityGeneration
{
    public class LongIdentityGenerator : ILongIdentityGenerator
    {
        #region Public Properties

        public static LongIdentityGenerator Instance => new LongIdentityGenerator();

        #endregion

        #region ILongIdentityGenerator

        public long Generate() => SnowflakeIdentityGenerator.Instance.Generate();

        #endregion
    }
}
