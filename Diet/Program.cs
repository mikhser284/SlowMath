namespace Diet
{
    using System;
    using SlowMath;
    using MathNet.Numerics.LinearAlgebra;

    class Program
    {
        static void Main(string[] args)
        {
            Matrix m = new Matrix(new double[,]
            {
                { 0, 0, 2 },
                { 1, 0, 3 },
                { 0, 0, 0 }
            }).View();

            //Console.WriteLine($"Det({nameof(m)}) = {m.Det()}");

            //Matrix minors = Matrix.GetDeterminant(m).View();

            MatrixIs typeOfMatrix;
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

namespace SlowMath
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Headers;

    public enum MatrixIs
    {
        Row,
        Column,
        NotFound
    }

    // https://people.richland.edu/james/lecture/m116/matrices/determinant.html
    public partial class Matrix
    {
        private Double[][] _matrix;

        /// <summary> Элемент матрицы находящийся на пересечении указанной строки и столбца </summary>
        /// <param name="r"> Индекс строки </param>
        /// <param name="c"> Индекс столбца </param>
        /// <returns> Значение указанного элемента </returns>
        public Double this[Int32 r, Int32 c]
        {
            get => _matrix[r][c];
            set => _matrix[r][c] = value;
        }

        /// <summary> Количество строк матрицы </summary>
        public Int32 RowsCount { get => _matrix.GetLength(0); }

        /// <summary> Получить строку матрицы </summary>
        /// <param name="rowIndex"> Индекс строки </param>
        /// <returns> Матрица содержащая указанную строку </returns>
        public Matrix GetRow(Int32 rowIndex)
        {
            Double[][] row = new Double[1][] { new Double[ColumnsCount] };
            for (int c = 0; c < ColumnsCount; c++) row[0][c] = this[rowIndex, c];
            return new Matrix(row);
        }

        /// <summary> Количество столбцов матрицы </summary>
        public Int32 ColumnsCount { get => _matrix[0].GetLength(0); }

        /// <summary> Получить столбец матрицы </summary>
        /// <param name="columnIndex"> Индекс столбца </param>
        /// <returns> Мотрица содержащая указанный столбец </returns>
        public Matrix GetColumn(Int32 columnIndex)
        {
            Double[][] column = new double[RowsCount][];
            for (int r = 0; r < RowsCount; r++) column[r] = new Double[] { this[r, columnIndex] };
            return new Matrix(column);
        }

        /// <summary> Является ли матрица квадратной </summary>
        public Boolean IsSquareMatrix { get => RowsCount == ColumnsCount; }

        /// <summary> Нулевая квадратная матрица </summary>
        /// <param name="order"> Порядок матрицы </param>
        public Matrix(Int32 order) : this(order, order) { }

        /// <summary> Квадратная матрица </summary>
        /// <param name="matrix"> Массив чисел определяющих матрицу </param>
        public Matrix(Double[] matrix)
        {
            _matrix = CreateMatrixArray(matrix.GetLength(0), 1);
            for (Int32 r = 0; r < RowsCount; r++) for (Int32 c = 0; c < ColumnsCount; c++) this[r, c] = matrix[r];
        }

        /// <summary> Нулевая прямоугольная матрица </summary>
        /// <param name="rowsCount"> Количество строк матрицы </param>
        /// <param name="columnsCount"> Количество столбцов матрицы </param>
        public Matrix(Int32 rowsCount, Int32 columnsCount)
        {
            if (rowsCount < 1) throw new ArgumentException("Количество столбцов матрицы не может быть меньше 1");
            if (columnsCount < 1) throw new ArgumentException("Количество строк матрицы не может быть меньше 1");
            _matrix = CreateMatrixArray(rowsCount, columnsCount);
        }

        public Matrix(Int32 rowsCount, Int32 columnsCount, Func<Int32, Int32, Double> valueByRowCol)
        {
            if (rowsCount < 1) throw new ArgumentException("Количество столбцов матрицы не может быть меньше 1");
            if (columnsCount < 1) throw new ArgumentException("Количество строк матрицы не может быть меньше 1");
            _matrix = CreateMatrixArray(rowsCount, columnsCount);
            for (Int32 r = 0; r < RowsCount; r++) for (Int32 c = 0; c < ColumnsCount; c++) this[r, c] = valueByRowCol(r, c);
        }

        public Matrix(Int32 rowsCount, Int32 columnsCount, Double valuesRowCol) : this (rowsCount, columnsCount, (r, c) => valuesRowCol) { }

        public Matrix(Int32 rank, Double valuesRowCol) : this(rank, rank, (r, c) => valuesRowCol) { }

        /// <summary> Прямоугольная матрица </summary>
        /// <param name="matrix"> Массив чисел определяющих матрицу </param>
        public Matrix(Double[,] matrix)
        {
            _matrix = CreateMatrixArray(matrix.GetLength(0), matrix.GetLength(1));
            for (Int32 r = 0; r < RowsCount; r++) for (Int32 c = 0; c < ColumnsCount; c++) this[r, c] = matrix[r, c];
        }

        private Matrix(Double[][] matrix)
        {
            _matrix = CreateMatrixArray(matrix.GetLength(0), matrix[0].GetLength(0));
            for (Int32 r = 0; r < RowsCount; r++) for (Int32 c = 0; c < ColumnsCount; c++) this[r, c] = matrix[r][c];
        }

        /// <summary> Матрица заполненная случайными числами </summary>
        /// <param name="rowsCount"> Количество строк матрицы </param>
        /// <param name="columnsCount"> Количество столбцов матрицы </param>
        /// <param name="min"> Минимальное число, которое может быть присвоено элементу матрицы (по умолчанию 0) </param>
        /// <param name="max"> Максимальное число, которое может быть присвоено элементу матрицы (по умолчанию < 100) </param>
        /// <returns> Случайная матрица </returns>
        public static Matrix Random(Int32 rowsCount, Int32 columnsCount, Int32 min = 0, Int32 max = 100)
        {
            Random rnd = new Random();
            Matrix m = new Matrix(rowsCount, columnsCount);
            for (Int32 r = 0; r < m.RowsCount; r++) for (Int32 c = 0; c < m.ColumnsCount; c++) m[r, c] = rnd.NextDouble() + rnd.Next(min, max);
            return m;
        }

        /// <summary> Получить единичную матрицу указанного порядка </summary>
        /// <param name="order"> Порядок матрицы </param>
        /// <returns> Единичная матрица указанного порядка </returns>
        public static Matrix IdentityMatrix(Int32 order)
        {
            Matrix m = new Matrix(order, order);
            for (int i = 0; i < order; i++) m[i, i] = 1;
            return m;
        }

        private static Double[][] CreateMatrixArray(Int32 rowsCount, Int32 columnsCount)
        {
            Double[][] matrix = new Double[rowsCount][];
            for (int r = 0; r < rowsCount; r++) matrix[r] = new Double[columnsCount];
            return matrix;
        }

        /// <summary> Получить копию матрицы </summary>
        /// <returns> Копия матрицы </returns>
        public Matrix Clone()
        {
            Matrix clone = new Matrix(RowsCount, ColumnsCount);
            for (Int32 r = 0; r < RowsCount; r++) for (Int32 c = 0; c < ColumnsCount; c++) clone[r, c] = this[r, c];
            return clone;
        }

        public override string ToString()
        {
            List<String> rows = new List<string>();
            for (int r = 0; r < RowsCount; r++)
                rows.Add($"{{ {String.Join(", ", _matrix[r].Select(i => $"{i:0.00}"))} }}");
            return $" {String.Join(", ", rows)} ";
        }

        /// <summary> Вывести матрицу на консоль </summary>
        /// <param name="description"> Примечания </param>
        /// <returns> Исходная матрица </returns>
        public Matrix View(Boolean showNulls = false, String description = default(String))
        {
            const Int32 itemWidth = 8;
            String view = String.Empty;
            for (int r = 0; r < RowsCount; r++)
            {
                String row = $" │ {String.Join(", ", _matrix[r].Select(i => i == 0.0 && !showNulls ? "-".PadLeft(itemWidth) : $"{i,itemWidth:0.00}"))} │\n";
                if (r == 0) view += $" ┌{new String(' ', row.Length - 4)}┐\n";
                view += row;
                if (r + 1 == RowsCount) view += $" └{new String(' ', row.Length - 4)}┘";
            }
            Console.WriteLine($"\n {description}\n{view}\n");
            return this;
        }

        public Matrix View(String description) => View(false, description);

        /// <summary> Транспонировать матрицу </summary>
        /// <returns> Транспонированная матрица </returns>
        public Matrix Transpose()
        {
            Matrix m = new Matrix(ColumnsCount, RowsCount);
            for (Int32 r = 0; r < RowsCount; r++) for (Int32 c = 0; c < ColumnsCount; c++) m[c, r] = this[r, c];
            return m;
        }

        public static Matrix operator -(Matrix a)
        {
            Matrix m = new Matrix(a.RowsCount, a.ColumnsCount);
            for (int r = 0; r < m.RowsCount; r++) for (int c = 0; c < m.ColumnsCount; c++) m[r, c] = -a[r, c];
            return m;
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            Matrix m = new Matrix(b.RowsCount, b.ColumnsCount);
            for (int r = 0; r < m.RowsCount; r++) for (int c = 0; c < m.ColumnsCount; c++) m[r, c] = a[r, c] + b[r, c];
            return m;
        }

        public static Matrix operator *(Double a, Matrix b)
        {
            Matrix m = new Matrix(b.RowsCount, b.ColumnsCount);
            for (int r = 0; r < m.RowsCount; r++) for (int c = 0; c < m.ColumnsCount; c++) m[r, c] = a * b[r, c];
            return m;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (b.RowsCount != a.ColumnsCount) throw new ArgumentException($"Количество строк матрицы B должно соответствовать количеству столбцов матрицы A (т.е. {a.ColumnsCount})");
            Matrix m = new Matrix(b.RowsCount, b.ColumnsCount);
            for (int r = 0; r < m.RowsCount; r++)
                for (int c = 0; c < m.ColumnsCount; c++)
                    for (int i = 0; i < b.RowsCount; i++)
                        m[r, c] += a[r, i] * b[i, c];
            return m;
        }

        
    }

    //public partial class Matrix
    //{
    //    public Int32? FindCheapestRowOrCol(out MatrixIs matrixOrientation)
    //    {
    //        Int32[] nullsByCols = new Int32[ColumnsCount];
    //        Int32 maxNullsInSingleCol = 0;
    //        Int32? colWithMaxNullsIndex = null;
    //        for (Int32 c = 0; c < ColumnsCount; c++)
    //        {
    //            for (Int32 r = 0; r < RowsCount; r++) nullsByCols[c] += this[r, c] == 0 ? 1 : 0;
    //            if (nullsByCols[c] > maxNullsInSingleCol)
    //            {
    //                maxNullsInSingleCol = nullsByCols[c];
    //                colWithMaxNullsIndex = c;
    //                if (maxNullsInSingleCol == ColumnsCount)
    //                {
    //                    matrixOrientation = MatrixIs.Column;
    //                    return colWithMaxNullsIndex;
    //                }
    //            }
    //        }

    //        Int32[] nullsByRows = new Int32[RowsCount];
    //        Int32 maxNullsInSingleRow = 0;
    //        Int32? rowWithMaxNullsIndex = null;
    //        for (Int32 r = 0; r < RowsCount; r++)
    //        {
    //            for (Int32 c = 0; c < ColumnsCount; c++) nullsByRows[r] += this[r, c] == 0 ? 1 : 0;
    //            if (nullsByRows[r] > maxNullsInSingleRow)
    //            {
    //                maxNullsInSingleRow = nullsByRows[r];
    //                rowWithMaxNullsIndex = r;
    //                if (maxNullsInSingleRow == RowsCount)
    //                {
    //                    matrixOrientation = MatrixIs.Row;
    //                    return rowWithMaxNullsIndex;
    //                }
    //            }
    //        }

    //        if (maxNullsInSingleCol >= maxNullsInSingleRow)
    //        {
    //            matrixOrientation = MatrixIs.Column;
    //            return colWithMaxNullsIndex;
    //        }

    //        if (maxNullsInSingleRow >= maxNullsInSingleCol)
    //        {
    //            matrixOrientation = MatrixIs.Row;
    //            return rowWithMaxNullsIndex;
    //        }

    //        matrixOrientation = MatrixIs.Column;
    //        return 0;
    //    }

    //    private static Boolean IsMatrix2x2(Matrix m)
    //    {
    //        return m.ColumnsCount == 2 && m.RowsCount == 2;
    //    }

    //    private static Boolean IsMatrix3x3(Matrix m)
    //    {
    //        return m.ColumnsCount == 3 && m.RowsCount == 3;
    //    }

    //    private static Double Det2x2(Matrix m)
    //    {
    //        return m[0, 0] * m[1, 1] - m[1, 0] * m[0, 1];
    //    }

    //    public Double Determinant()
    //    {
    //        if (IsMatrix2x2(this)) return Det2x2(this);

    //        MatrixIs matrixOrientation;
    //        Int32? rowOrColIndex = FindCheapestRowOrCol(out matrixOrientation);
    //        if (rowOrColIndex == null) return 0;

    //        Matrix minors;

    //        if (matrixOrientation == MatrixIs.Column)
    //        {
    //            if (rowOrColIndex == this.ColumnsCount) return 0;
    //            if (!IsMatrix3x3(this)) throw new NotImplementedException();
    //            minors = new Matrix(RowsCount, ColumnsCount - 1);
    //            for (Int32 r = 0; r < RowsCount; r++)
    //            {
    //                Matrix lm = LowerMatrixExcept(this, r, (Int32)rowOrColIndex);
    //            }
    //        }
    //        if(matrixOrientation == MatrixIs.Row)
    //        {
    //            if (rowOrColIndex == this.RowsCount) return 0;
    //            if (!IsMatrix3x3(this)) throw new NotImplementedException();
    //            minors = new Matrix(RowsCount - 1, ColumnsCount);
    //            for (Int32 c = 0; c < ColumnsCount; c++)
    //            {

    //            }
    //        }


    //        //TODO
    //        throw new NotImplementedException();
    //    }


        


    //    public static Matrix GetDeterminant(Matrix x)
    //    {
    //        if (IsMatrix2x2(x)) return Det2x2(x);

    //        if (!IsMatrix3x3(x)) return null;
    //        Matrix minors = new Matrix(x.RowsCount, x.ColumnsCount);

    //        for (int r = 0; r < x.RowsCount; r++)
    //            for (int c = 0; c < x.ColumnsCount; c++)
    //            {
    //                Matrix lm = LowerMatrixExcept(x, r, c);
    //                minors[r, c] = Det2x2(lm)[0, 0];
    //            }

    //        Matrix cofactors = Matrix.GetCofactors(minors);
    //        Matrix det = new Matrix(1);

    //        for (int c = 0; c < x.ColumnsCount; c++)
    //        {
    //            det[0, 0] += x[0, c] * cofactors[0, c];
    //        }

    //        return det;
    //    }


        

    //    public static Matrix GetCofactors(Matrix x)
    //    {
    //        Matrix cofactors = new Matrix(x.RowsCount, x.ColumnsCount);
    //        for (int r = 0; r < x.RowsCount; r++)
    //            for (int c = 0; c < x.ColumnsCount; c++)
    //                cofactors[r, c] = IsChangableItem(r, c) ? -x[r, c] : x[r, c];
    //        return cofactors;

    //        static Boolean IsChangableItem(Int32 rowIndex, Int32 colIndex) => (rowIndex % 2 == 1) != (colIndex % 2 == 1);
    //    }

    //    public static Matrix LowerMatrixExcept(Matrix m, Int32 excludableRowIndex, Int32 excludableColumnIndex)
    //    {
    //        Matrix lowerMatrix = new Matrix(m.RowsCount - 1, m.ColumnsCount - 1);
    //        for (int r = 0; r < m.RowsCount; r++)
    //        {
    //            if (r == excludableRowIndex) continue;
    //            for (int c = 0; c < m.ColumnsCount; c++)
    //            {
    //                if (c == excludableColumnIndex) continue;
    //                lowerMatrix[RowA(r), ColA(c)] = m[r, c];
    //            }
    //        }
    //        return lowerMatrix;
    //        Int32 RowA(Int32 rowB) => rowB > excludableRowIndex ? rowB - 1 : rowB;
    //        Int32 ColA(Int32 colB) => colB > excludableColumnIndex ? colB - 1 : colB;
    //    }

    //    public static Matrix Diagonals(Int32 order, Int32 offset = 0)
    //    {
    //        Matrix m = new Matrix(order, order);
    //        for (int i = 0; i < order; i++)
    //        {
    //            m[Correct(i), Correct(i + offset)] += 1;
    //            m[CorrectR(order - i - 1), Correct(i + offset)] += 2;
    //        }
    //        return m;
    //        Int32 CorrectR(Int32 initValue) => initValue < 0 ? order + initValue : initValue;
    //        Int32 Correct(Int32 initValue) => initValue < order ? initValue : initValue - order;
    //    }

    //    /// <summary> Получить обратную матрицу </summary>
    //    /// <returns> Обратная матрица </returns>
    //    public Matrix Reverse()
    //    {
    //        if (!IsSquareMatrix) throw new InvalidOperationException("Обратная матрица может быть получена только для квадратных матриц.");
    //        throw new NotImplementedException("Operation not implemented: 'Matrix;  public Matrix Reverse()'");
    //    }

    //    ///// <summary> Получить определитель матрицы </summary>
    //    ///// <returns> Определитель матрицы </returns>
    //    //public Double Det()
    //    //{
    //    //    if (!IsSquareMatrix) throw new InvalidOperationException("Определитель может быть получен только для квадратных матриц.");
    //    //    if (RowsCount > 4) throw new NotImplementedException("Операция не реализована для квадратных матриц с размерностю больше чем 4");
    //    //    Int32 order = RowsCount;
    //    //    Double det = 0;
    //    //    for (Int32 offset = 0; offset < order; offset++)
    //    //    {
    //    //        Double multA = 1;
    //    //        Double multB = 1;

    //    //        for (int i = 0; i < order; i++)
    //    //        {
    //    //            multA *= this[Correct(i), Correct(i + offset)];
    //    //            multB *= this[CorrectR(order - i - 1), Correct(i + offset)];
    //    //        }
    //    //        det = det + multA - multB;
    //    //    }

    //    //    return det;
    //    //    Int32 CorrectR(Int32 initValue) => initValue < 0 ? order + initValue : initValue;
    //    //    Int32 Correct(Int32 initValue) => initValue < order ? initValue : initValue - order;
    //    //}
    //}
}