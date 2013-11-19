using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace PPLServerTool
{
    public class PPLDataList
    {
        private ArrayList pplList;

        public PPLDataList()
        {
            pplList = new ArrayList();
        }

        public void insertPPLData(PPLData pplData)
        {
            pplList.Add(pplData);
        }

        public PPLData getPPL(int index)
        {
            return (PPLData)pplList[index];
        }

        public ArrayList getPPLList()
        {
            return pplList;
        }

        public int getCount()
        {
            return pplList.Count;
        }

        public void clearList()
        {
            pplList.Clear();
        }
    }
}
