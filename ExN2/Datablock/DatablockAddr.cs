using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExN2.Datablock {

    //=====================================================================================
    // auxiliary class: handling of the Siemens PLC address arithmetics
    public class DatablockAddr {
        public int iOffsByte { get; private set; }
        public int iOffsBit { get; private set; }

        //...............................................................................................
        /// <summary> round the addres according to properties of aItem       </summary>
        public void DoAddrRounding(DbVisuItem aItem) {

            // find the stronger rounding of:
            //  - based on item type
            //  - specified in a special field                                  
            tRoundAddr Rounding = Def.itemDefs[(int)aItem.Type].Rounding;
            if (aItem.ForceRounding > 0)
                if (aItem.ForceRounding > Rounding)
                    Rounding = aItem.ForceRounding;

            switch (Rounding) {
                case tRoundAddr.None: break;

                case tRoundAddr.Byte1:
                    if (iOffsBit > 0) {
                        iOffsBit = 0;
                        iOffsByte++;
                    }
                    break;

                case tRoundAddr.Byte2:
                    if (iOffsBit > 0) {
                        iOffsBit = 0;
                        iOffsByte++;
                    }
                    iOffsByte = ((iOffsByte + 1) / 2) * 2;
                    break;

                case tRoundAddr.Byte4:
                    if (iOffsBit > 0) {
                        iOffsBit = 0;
                        iOffsByte++;
                    }
                    iOffsByte = ((iOffsByte + 3) / 4) * 4;
                    break;
            }
        }

        //...............................................................................................
        public void IncrementAddr(tDbVisuItemType aItemType, int aOptLen) {

            DbItemDef itemDef = Def.itemDefs[(int)aItemType];

            iOffsBit += itemDef.iBitSize;
            if (iOffsBit >= 8) {
                iOffsByte++;
                iOffsBit = 0;
            }
            iOffsByte += itemDef.iByteSize;

            if (aItemType == tDbVisuItemType.String) {
                iOffsByte += aOptLen;
            }
        }
    }


}
