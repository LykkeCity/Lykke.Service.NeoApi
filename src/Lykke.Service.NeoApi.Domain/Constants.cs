namespace Lykke.Service.NeoApi.Domain
{
    public static class Constants
    {
        public static class Assets
        {
            public static class Neo
            {

                public const string AssetId = "Neo";
                public const string Name = "Neo";
                public const int Accuracy = 0; // The minimum unit of NEO is 1 and tokens cannot be subdivided.
            }

            public static class Gas
            {

                public const string AssetId = "Gas";
                public const string Name = "Gas";
                public const int Accuracy = 8; 
            }
        }
    }
}
