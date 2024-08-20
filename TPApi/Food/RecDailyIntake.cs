using Microsoft.SqlServer.Server;
using System.Reflection.Metadata;

namespace TPApi.Food
{
    public static class RecDailyIntake
    {
        public static float Jod = 150f; // µg
        public static float Jarn = 10.8f; // mg
        public static float Kalcium = 966f; // mg
        public static float Kalium = 3500f; // mg
        public static float Magnesium = 325; // mg
        public static float Selen = 82.5f; // µg
        public static float Zink = 11.1f; // mg
        public static float A = 750f; // (1 RE = 1 μg retinol, 2 μg beta-karoten från tillskott, 6 μg beta-karoten från mat, eller 12 μg andra former av karotenoider[provitamin A] såsom alfa-karoten och beta-kryptoxantin)
        public static float B1 = 1.1f; // mg. Antaget totalt intag av energi: 10mj.
        public static float B2 = 1.6f; // mg
        public static float B3 = 16f; // NE = Niacinekvivalenter. Antaget totalt intag av energi: 10mj.
        public static float B6 = 1.7f; // mg
        public static float B9 = 330f; // µg
        public static float B12 = 4.0f; // µg
        public static float C = 102.5f; // mg
        public static float D = 10f; // µg
        public static float E = 11f; // α-TE = alfa-tokoferolekvivalenter
    }
}
