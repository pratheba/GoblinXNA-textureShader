using System;

namespace AdvTerrain
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TerrainClass terrainObj = new TerrainClass())
            {
                terrainObj.Run();
            }
        }
    }
#endif
}

