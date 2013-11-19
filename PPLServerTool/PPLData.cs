using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPLServerTool
{
    public class PPLData
    {
        private string productCode;
        private string programCode;
        private string productName;
        private string brandName;
        private string productImage;
        private string price;
        private string storeLink;
        private int startTime;
        private int endTime;

        public PPLData(string productCode, string programCode, string productName, string brandName, string productImage, string price, string storeLink, int startTime, int endTime)
        {
            setProductCode(productCode);
            setProgramCode(programCode);
            setProductName(productName);
            setBrandName(brandName);
            setProductImage(productImage);
            setPrice(price);
            setStoreLink(storeLink);
            setStartTime(startTime);
            setEndTime(endTime);
        }

        public void setProductCode(string productCode)
        {
            this.productCode = productCode;
        }
        public string getProductCode()
        {
            return productCode;
        }

        public void setProgramCode(string programCode)
        {
            this.programCode = programCode;
        }
        public string getProgramCode()
        {
            return programCode;
        }

        public void setProductName(string productName)
        {
            this.productName = productName;
        }
        public string getProductName()
        {
            return productName;
        }

        public void setBrandName(string brandName)
        {
            this.brandName = brandName;
        }
        public string getBrandName()
        {
            return brandName;
        }

        public void setProductImage(string productImage)
        {
            this.productImage = productImage;
        }
        public string getProductImage()
        {
            return productImage;
        }

        public void setPrice(string price)
        {
            this.price = price;
        }
        public string getPrice()
        {
            return price;
        }

        public void setStoreLink(string storeLink)
        {
            this.storeLink = storeLink;
        }
        public string getStoreLink()
        {
            return storeLink;
        }

        public void setStartTime(int startTime)
        {
            this.startTime = startTime;
        }
        public int getStartTime()
        {
            return startTime;
        }

        public void setEndTime(int endTime)
        {
            this.endTime = endTime;
        }
        public int getEndTime()
        {
            return endTime;
        }
    }
}
