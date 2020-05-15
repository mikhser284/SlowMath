namespace Diet
{
    using System;
    using SlowMath;

    class Program
    {
        static void Main(string[] args)
        {
            //Matrix m = new Matrix(new double[,]
            //{
            //    { 7.0,  0.0, 55.0, 230.0 },
            //    { 5.0,  2.5,  4.8,  60.0 },
            //    { 1.0, 97.0,  0.0, 780.0 },
            //    { 2.0,  0.1, 19.7,  90.0 }
            //}).View();

            //Console.WriteLine($"Det({nameof(m)}) = {m.Determinant()}");
            //Console.WriteLine($"#Expected Det({nameof(m)}) = 212378,9");

            Matrix signsMatrix = new Matrix(9, (r, c) => ((r % 2 == 1) != (c % 2 == 1)) ? -1 : 1).View();


            Matrix m = new Matrix(new double[,]
            {
                { 3, 2, 0, 1 },
                { 4, 0, 1, 2 },
                { 3, 0, 2, 1 },
                { 9, 2, 3, 1 }
            }).View();

            //Matrix m = new Matrix(new double[,]
            //{
            //    { 3, 0, 1, },
            //    { 4, 1, 2, },
            //    { 3, 2, 1, },
            //}).View();

            Console.WriteLine($"Det({nameof(m)}) = {m.Determinant()}");
            Console.WriteLine($"#Expected Det({nameof(m)}) = 24");

            //Matrix minors = Matrix.GetDeterminant(m).View();



            //Int32? index = m.FindCheapestRowOrCol(out typeOfMatrix);

            //Console.WriteLine($"The cheapest {typeOfMatrix} is {index}");


            //Matrix m2 = m.LowerMatrixExcept(1, 1).View();

        }

        //public static void IrasTask()
        //{
        //    Matrix a = new Matrix(new double[,]
        //    {
        //        { 7.0,  0.0, 55.0, 230.0 },
        //        { 5.0,  2.5,  4.8,  60.0 },
        //        { 1.0, 97.0,  0.0, 780.0 },
        //        { 2.0,  0.1, 19.7,  90.0 }
        //    }).View("[A]: Енергетична цiннiсть продуктiв:\n - c: бiлки, мг; жири, мг; вуглеводи, мг; ернергозатрати, ккал\n - r: хлiб пшеничний; молоко; мало вершкове; картопля");

        //    Matrix b = new Matrix(new Double[] { 105, 100, 415, 2900 }).View("[B]: Середня добова потреба студента в поживних речовинах та енергозатратах\n - r: бiлки, мг; жири, мг; вуглеводи, мг; ернергозатрати, ккал");

        //    Matrix x = new Matrix(4, 1).View("[X]: Шуканi величини\n - r: кiлькiсть продуктiв: хлiб пшеничний; молоко; мало вершкове; картопля");

        //    if (!a.IsSquareMatrix)
        //    {
        //        Console.WriteLine("Не має розв'язку - матриця не квадратна.");
        //        return;
        //    }
        //    Double detA = a.Det();
        //    if (detA != 0.0)
        //    {
        //        Console.WriteLine($"Дерермінант матриці A = {detA}");

        //        Matrix aReversed = a.Reverse().View("A' (матриця обернена до матриці A)");

        //        x = (aReversed * b).View("[X]: Шуканi величини\n - r: кiлькiсть продуктiв: хлiб пшеничний; молоко; мало вершкове; картопля");
        //    }
        //    else Console.WriteLine("Не має розв'язку - детермінант матриці A = 0");
        //}
    }
}
