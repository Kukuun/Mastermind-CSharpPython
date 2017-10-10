using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind {
    class HistoryItem {
        public int[] Combination; // combination of colors
        public int PlacedCorrect; // colors placed correctly relative to master combination
        public int ColoredCorrect; // correct colors relative to master combination

        public HistoryItem(int[] comb, int placedCorrect, int coloredCorrect) {
            Combination = comb;
            PlacedCorrect = placedCorrect;
            ColoredCorrect = coloredCorrect;
        }
    }
}
