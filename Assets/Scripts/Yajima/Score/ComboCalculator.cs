namespace Yajima
{
    public static class ComboCalculator
    {
        static int _bonus = 10;

        public static int CalculateBonus(int combo)
        {
            return _bonus * combo;
        }
    }
}
