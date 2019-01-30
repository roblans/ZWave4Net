using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CommunicationValidator.Models
{
    public class LineStatusIndicator
    {

        public LineStatusIndicatorLine[] Lines { get; }

        public LineStatusIndicator(LogRow[] rows, double height)
        {
            if (!rows.Any())
                return;

            var lines = new List<LineStatusIndicatorLine>();
            var calcLineHeight = height / rows.Count();
            var lineHeight = Math.Max(0.5, calcLineHeight);

            for (int i = 0; i < rows.Count(); i++)
            {
                var row = rows[i];
                var topOffset = calcLineHeight * i;

                lines.Add(new LineStatusIndicatorLine()
                {
                    Brush = row.IsAck ? Brushes.Green :
                        row.IsNakOrCan ? Brushes.Red :
                        Brushes.Black,
                    Height = row.IsNakOrCan ? lineHeight * 4 : lineHeight,
                    Width = row.IsAck ? 10 :
                        row.IsNakOrCan ? 30 :
                        20,
                    Margin = new Thickness(0, topOffset, 0, 0)
                });
            }

            Lines = lines.ToArray();
        }

    }

    public class LineStatusIndicatorLine
    {
        public Thickness Margin { get; set; }

        public Brush Brush { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
}
