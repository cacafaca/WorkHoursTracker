using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.WorkHoursTracker.Model
{
    public class ExcelCellCoordinates
    {
        const char firstLetter = 'A';
        const char lastLetter = 'Z';
        const char lowestDigit = '0';
        const char highestDigit = '9';

        int _row;
        int _column;

        public ExcelCellCoordinates(string column, int row)
        {
            _row = row;
            _column = convertAlphaToNumber(column.ToUpper());
        }
        public ExcelCellCoordinates(string cell)
        {
            string column = string.Empty;
            string row = string.Empty;
            foreach (char c in cell.ToUpper())
            {
                if (row == string.Empty)
                    if (firstLetter <= c && c <= lastLetter)
                        column += c;
                    else
                        row += c;
                else
                    if (lowestDigit <= c && c <= highestDigit)
                    row += c;
                else
                    throw new ArgumentOutOfRangeException(nameof(cell), "Expect column name letters and row number, like \"C6\".");
            }
            _row = int.Parse(row);
            _column = convertAlphaToNumber(column.ToUpper());
        }

        int convertAlphaToNumber(string alpha)
        {
            // Validate.
            if (alpha.Any(c => c < firstLetter || lastLetter < c))
                throw new ArgumentOutOfRangeException(nameof(alpha), "Must contains only characters from 'a'..'z'.");

            byte[] num = Encoding.ASCII.GetBytes(alpha.ToUpper())
                .Select(n => (byte)(n - Convert.ToByte(firstLetter) + 1)).ToArray();

            byte range = (byte)(Convert.ToByte(lastLetter) - Convert.ToByte(firstLetter) + 1);
            int indexValue = 0;
            for (int i = 0; i < num.Length; i++)
            {
                indexValue += Convert.ToInt32(Math.Pow(range, num.Length - i - 1)) * num[i];
            }
            return indexValue;
        }

        public int Row
        {
            get { return _row; }
        }
        public int Column
        {
            get { return _column; }
        }
    }
}
