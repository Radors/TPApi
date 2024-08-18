using Microsoft.SqlServer.Server;
using System.Reflection.Metadata;

namespace TPApi.Food
{
    // Genomsnittligt RI / Rekommenderat Intag eller Tillräckligt intag (AI, Adequate Intake) (NNR 2023) för män och kvinnor 18-70 år
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
        public static float B1 = 0.1f; // mg/MJ
        public static float B2 = 1.6f; // mg
        public static float B3 = 1.6f; // NE/MJ / NE = Niacinekvivalent (1 NE = 1 mg niacin = 60 mg tryptofan).
        public static float B6 = 1.7f; // mg
        public static float B9 = 330f; // µg
        public static float B12 = 4.0f; // µg
        public static float C = 102.5f; // mg
        public static float D = 10f; // µg
        public static float E = 10f; // α-TE3 / Baserat på ett intag av fleromättade fettsyror motsvarande 7,5 % av energiintaget. α-TE = alfa-tokoferolekvivalenter = 1 mg RRR alfa-tokoferol.
    }
}
