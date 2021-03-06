﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace DataRefresh.Filter
{
    public static class Filter
    {
        public static void DisplayFilteredValues<T>(
                this IEnumerable<T> values,
                String selectedValue,
                DataGridView displayBox,
                Func<T,Boolean> filter,
                Func<T,String> description,
                Func<T,String,Boolean> compare,
                IEnumerable<Func<T,Boolean>> additionalConditions,
                Color? disabledBack = null,
                Color? disabledFore = null,
                Color? disabledSelectedFore = null,
                Color? enabledBack = null,
                Color? enabledFore = null,
                Color? enabledSelectedFore = null,
                AutoScroll scrollTo = AutoScroll.Center,
                Boolean scrollIncludePartialRow = true)
        {
            Int32? selectedRow = null;
            var allEntries = values.Where(filter);
            foreach (var entry in allEntries)
            {
                String desc = description(entry);
                Int32 row = displayBox.Rows.Add(new Object[] { desc });
                if (additionalConditions.Select((x) => x(entry)).Where((x) => !x).Any())
                {
                    displayBox.Rows[row].DefaultCellStyle.BackColor = disabledBack.HasValue ? disabledBack.Value : Color.LightGray;
                    displayBox.Rows[row].DefaultCellStyle.ForeColor = disabledFore.HasValue ? disabledFore.Value : Color.DimGray;
                    displayBox.Rows[row].DefaultCellStyle.SelectionForeColor = disabledSelectedFore.HasValue ? disabledSelectedFore.Value : Color.DimGray;
                }
                else
                {
                    displayBox.Rows[row].DefaultCellStyle.BackColor = enabledBack.HasValue ? enabledBack.Value : Color.White;
                    displayBox.Rows[row].DefaultCellStyle.ForeColor = enabledFore.HasValue ? enabledFore.Value : Color.Black;
                    displayBox.Rows[row].DefaultCellStyle.SelectionForeColor = enabledSelectedFore.HasValue ? enabledSelectedFore.Value : Color.White;
                }
                if (selectedValue != null && compare(entry, selectedValue))
                {
                    selectedRow = row;
                }
            }
            if (selectedRow.HasValue)
            {
                displayBox.Rows[selectedRow.Value].Selected = true;
                int displayCount = displayBox.DisplayedRowCount(scrollIncludePartialRow);
                switch (scrollTo)
                {
                    case AutoScroll.None:
                        break;
                    case AutoScroll.Top:
                        displayBox.FirstDisplayedCell = displayBox.SelectedRows[0].Cells[0];
                        break;
                    case AutoScroll.Bottom:
                        int targetRow = selectedRow.Value - displayCount;
                        if (targetRow >= 0)
                        {
                            displayBox.FirstDisplayedScrollingRowIndex = targetRow;
                        }
                        break;
                    case AutoScroll.Center:
                        int midpoint = Convert.ToInt32(Math.Floor((double)displayCount / 2));
                        int firstRow = displayBox.FirstDisplayedScrollingRowIndex;
                        if ((firstRow + midpoint) > selectedRow.Value || (firstRow + displayCount - midpoint) <= selectedRow.Value)
                        {
                            displayBox.FirstDisplayedScrollingRowIndex = Math.Max(selectedRow.Value - midpoint, 0);
                        }
                        break;
                }
            }
        }

        public enum AutoScroll
        {
            None,
            Top,
            Center,
            Bottom
        }
    }
}
