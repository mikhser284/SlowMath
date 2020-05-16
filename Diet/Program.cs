namespace Diet
{
    using System;
    using SlowMath;

    class Program
    {
        static void Main(string[] args)
        {
            IrasTask();   
        }

        public static void IrasTask()
        {
            Matrix a = new Matrix(new double[,]
            {
                { 7.0,  0.0, 55.0, 230.0 },
                { 5.0,  2.5,  4.8,  60.0 },
                { 1.0, 97.0,  0.0, 780.0 },
                { 2.0,  0.1, 19.7,  90.0 }
            }).View("[A]: Енергетична цiннiсть продуктiв:\n - c: бiлки, мг; жири, мг; вуглеводи, мг; ернергозатрати, ккал\n - r: хлiб пшеничний; молоко; мало вершкове; картопля");

            Console.WriteLine($"Det = {a.Det()}");

            Matrix b = new Matrix(new Double[] { 105, 100, 415, 2900 }).View("[B]: Середня добова потреба студента в поживних речовинах та енергозатратах\n - r: бiлки, мг; жири, мг; вуглеводи, мг; ернергозатрати, ккал");

            Matrix x = new Matrix(4, 1).View("[X]: Шуканi величини\n - r: кiлькiсть продуктiв: хлiб пшеничний; молоко; мало вершкове; картопля");

            if (!a.IsSquareMatrix)
            {
                Console.WriteLine("Не має розв'язку - матриця не квадратна.");
                return;
            }

            Double detA;
            Matrix aReversed = a.Reverse(out detA);
            Console.WriteLine($"Дерермiнант матрицi A = {detA}");

            if (detA != 0.0)
            {
                aReversed.View("A' (матриця обернена до матрицi A)", 2, 10);
                (aReversed * a).View(1, 0, false, "Перевiряємо коректнiсть розрахунку оберненої матрицi - якщо все вiрно ми повиннi отримати одиничну матрицю");
                x = (aReversed.Transpose() * b).View("[X]: Шуканi величини\n - r: кiлькiсть продуктiв: хлiб пшеничний; молоко; мало вершкове; картопля");
            }
            else Console.WriteLine("Не має розв'язку");
        }
    }
}
