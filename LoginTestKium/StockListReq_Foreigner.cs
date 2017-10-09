
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginTestKium
{
    class StockListReq_Foreigner
    {

        public string foreiner_obj_stock_cd;
        public string foreiner_obj_stock_cd_nm;
        string foreiner_obj_stock_deal_amount;
        string foreiner_obj_stock_high_price;
        string foreiner_obj_stock_low_price;
        string foreiner_obj_stock_price;
        string foreiner_obj_stock_percent;
        string foreiner_obj_stock_profit;


        public StockListReq_Foreigner()
        {
            
        }

        public void SetData(string obj_stock_cd , string obj_stock_cd_nm, string obj_stock_deal_amount, string obj_stock_price, string obj_stock_percent, string obj_stock_profit)
        {
            
           
            foreiner_obj_stock_cd = obj_stock_cd;
            foreiner_obj_stock_cd_nm = obj_stock_cd_nm;
            foreiner_obj_stock_deal_amount = obj_stock_deal_amount;
            foreiner_obj_stock_price = obj_stock_price;
            foreiner_obj_stock_percent = obj_stock_percent;
            foreiner_obj_stock_profit = obj_stock_profit;

        }

    }
}
