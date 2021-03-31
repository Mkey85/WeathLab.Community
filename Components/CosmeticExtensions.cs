using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using WealthLab.Backtest;

namespace WealthLab.Community
{
    public static class CosmeticExtensions
    {
        /* Usage: run after Execute has finished
        public override void BacktestComplete()
        {
            this.DrawTradeLines(GetPositions(), false);
        } 
        */

        public static void DrawTradeLines(this UserStrategyBase obj, List<Position> lst, bool showSignal = false)
        {
            for (int iPos = 0; iPos < lst.Count; iPos++)
            {
                Position position = lst[iPos];
                int positionExitBar;
                double positionExitPrice;

                if (position.IsOpen)
                {
                    positionExitBar = position.Bars.Count - 1;
                    positionExitPrice = position.Bars.Close[positionExitBar];
                }
                else
                {
                    positionExitBar = position.ExitBar;
                    positionExitPrice = position.ExitPrice;
                }

                Color col;
                if (position.PositionType == PositionType.Long)
                    col = (positionExitPrice - position.EntryPrice) > 0 ? Color.Green : Color.Red;
                else
                    col = (positionExitPrice - position.EntryPrice) > 0 ? Color.Red : Color.Green;

                obj.DrawLine(position.EntryBar, position.EntryPrice, positionExitBar, positionExitPrice, col);

                if (showSignal)
                {
                    obj.DrawBarAnnotation(position.EntrySignalName, position.EntryBar, position.PositionType == PositionType.Long, Color.Black, 12);
                    obj.DrawBarAnnotation(position.ExitSignalName, position.ExitBar, position.PositionType == PositionType.Short, Color.Black, 12);
                }
            }
        }
    }
}
