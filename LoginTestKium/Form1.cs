using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginTestKium
{
    public partial class Form1 : Form
    {
        static System.Windows.Forms.Timer mTimer = new System.Windows.Forms.Timer();
        static bool checkFlag = false;
        int timeCnt = 0;
        /*현재일자를 구함*/
        DateTime dt = DateTime.Now.AddDays(-8);
        
        int Stock_cnt = 0;
        int Stock_cnt_2 = 0;
        int Stock_cnt_3 = 0;
        int Stock_cnt_4 = 0;
        int StockMinuteCnt = 0; /*분봉체크하는 로직 추가*/
        int InformStockCnt = 0;
        int nCount = 0;
        string g_stock_cd;
        string g_stock_cd_2;
        string g_stock_cd_3; 

        string g_kospiGubun;

        // StockListReq_Foreigner crf;
        private static ManualResetEvent _stopped = new ManualResetEvent(false);
        int MyBudget = 0; 


        List<string> stock_list = new List<string>(); 
        List<string> stock_list_2 = new List<string>(); 
        List<StockListReq_Foreigner> Foreigner_list = new List<StockListReq_Foreigner>();
        List<string> stock_list3 = new List<string>();

        List<string> stock_buy_list = new List<string>(); /*주식매수리스트*/
        Dictionary<string,int> ObjMinuteCheck = new Dictionary<string,int>(); /*매수주식대상중에 분봉차트 분석대상*/
        StreamWriter sw;

        Dictionary<string, int> Data20Line;

        Dictionary<string, WeekChart> DataWeek;
        Dictionary<string, WeekChart> DataDay;
        Dictionary<string, WeekChart> DataMinute;

        /*급등 관련 코드 추가*/
        bool realDataGubunflag = false;
        List<string> SharpIncrease_stock_list = new List<string>(); /*급등주 리스트 */
        Dictionary<string, int>  SharpIncrease_sell_list = new Dictionary<string, int>();  /*매수주식대상중에 매도 분봉차트 분석대상*/
        int SI_StockMinuteCnt = 0;
        int SI_sellStockMinuteCnt = 0;

        // FileStream Stockfs;
        // BinaryWriter Stockbw;

        public void AutoBuyStop()
        {
            mTimer.Enabled = false;
            mTimer.Stop();
        }

        public void AutoBuyStart()
        {
            BankStockcdSearch();

            if (Code.Searchgubuns[dealGubun.SelectedIndex].Code == 5)
            {

                //searchSharpIncrease("001", "1", "1", "5", "00000", "1", "0", "4", "0");
                search_10023SharpIncrease();

                // searchSharpIncrease("001", "1", "2", "1", "00000", "1", "0", "4", "0");
                mTimer.Interval = 5000;
                // SharpIncrease_stock_list

            }
            else if(Code.Searchgubuns[dealGubun.SelectedIndex].Code == 3)
            {
                realDataGubunflag = false;
                Check_DealPercent();
                
                //this.listBox1.Items.Add(timeCnt);
                mTimer.Interval = 180000;
                //mTimer.Enabled = true;
            }
            else
            {
                LogFileWrite("=============AutoBuyStart===============");
               // checkKospi();
                Req_Foreigner(1);
                //this.listBox1.Items.Add(timeCnt);
                mTimer.Interval = 180000;
                //mTimer.Enabled = true;
            }




        }

    /*
	1. 시장구분 = 000:전체, 001:코스피, 101:코스닥, 201:코스피200
	2. 등락구분 = 1:급등, 2:급락
	3. 시간구분 = 1:분전, 2:일전
	4. 시간 = 분 혹은 일입력
	5. 거래량구분 = 00000:전체조회, 00010:만주이상, 00050:5만주이상, 00100:10만주이상, 00150:15만주이상, 00200:20만주이상, 00300:30만주이상, 00500:50만주이상, 01000:백만주이상
	6. 종목조건 = 0:전체조회,1:관리종목제외, 3:우선주제외, 5:증100제외, 6:증100만보기, 7:증40만보기, 8:증30만보기
	7. 신용조건 = 0:전체조회, 1:신용융자A군, 2:신용융자B군, 3:신용융자C군, 4:신용융자D군, 9:신용융자전체
	8. 가격조건 = 0:전체조회, 1:1천원미만, 2:1천원~2천원, 3:2천원~3천원, 4:5천원~1만원, 5:1만원이상, 8:1천원이상
	9. 상하한포함 = 0:미포함, 1:포함
    */
        private void search_10019SharpIncrease(String parma1 , String parma2, String parma3, String parma4, String parma5, String parma6, String parma7, String parma8, String parma9)
        {
            Console.WriteLine("======급등주 10019 상위요청========");
            axKHOpenAPI1.SetInputValue("시장구분", parma1);
             axKHOpenAPI1.SetInputValue("등락구분", parma2);
             axKHOpenAPI1.SetInputValue("시간구분", parma3);
             axKHOpenAPI1.SetInputValue("시간", parma4);
             axKHOpenAPI1.SetInputValue("거래량구분", parma5);
             axKHOpenAPI1.SetInputValue("종목조건", parma6);
             axKHOpenAPI1.SetInputValue("신용조건", parma7);
             axKHOpenAPI1.SetInputValue("가격조건", parma8);
             axKHOpenAPI1.SetInputValue("상하한포함", parma9);

             int nRet = 0;
             nRet = axKHOpenAPI1.CommRqData("search_10019SharpIncrease", "OPT10019", 0, parma1);
             System.Threading.Thread.Sleep(1000);
            

        }
        /*
         * 	시장구분 = 000:전체, 001:코스피, 101:코스닥
            정렬구분 = 1:급증량, 2:급증률
            시간구분 = 1:분, 2:전일
            거래량구분 = 5:5천주이상, 10:만주이상, 50:5만주이상, 100:10만주이상, 200:20만주이상, 300:30만주이상, 500:50만주이상, 1000:백만주이상
            시간 = 분 입력
            종목조건 = 0:전체조회, 1:관리종목제외, 5:증100제외, 6:증100만보기, 7:증40만보기, 8:증30만보기, 9:증20만보기
            가격구분 = 0:전체조회, 2:5만원이상, 5:1만원이상, 6:5천원이상, 8:1천원이상, 9:10만원이상
         */
        private void search_10023SharpIncrease()
        {
            String parma1, parma2, parma3 , param4;

            parma1 = Code.MarketGubuns[MarketGubun.SelectedIndex].Code;
            parma2 = Code.PriceGubuns[priceStrdGubun.SelectedIndex].Code.ToString();
            parma3 = Code.DealAmountGubuns[dealAmountGubun_SI.SelectedIndex].Code.ToString();
            param4 = timeStrdbefore.Text;
            mTimer.Enabled = false;

            /*2017.10.13 위치변경*/
            SI_StockMinuteCnt = 0;

            Console.WriteLine("======급등주 10023 상위요청========");
            Console.WriteLine("MarketGubuns=" + parma1);
            Console.WriteLine("PriceGubuns=" + parma2);
            Console.WriteLine("DealAmountGubuns=" + parma3);
            Console.WriteLine("timeStrdbefore=" + param4);
            axKHOpenAPI1.SetInputValue("시장구분", parma1);
            axKHOpenAPI1.SetInputValue("정렬구분", "1");
            axKHOpenAPI1.SetInputValue("시간구분", "1"); /*시간구분 = 1:분, 2:전일*/
            axKHOpenAPI1.SetInputValue("거래량구분", parma3);
            axKHOpenAPI1.SetInputValue("시간", param4);
            axKHOpenAPI1.SetInputValue("종목조건", "1");
            axKHOpenAPI1.SetInputValue("가격구분", parma2);

            int nRet = 0;
            nRet = axKHOpenAPI1.CommRqData("search_10023SharpIncrease", "OPT10023", 0, parma1);
            System.Threading.Thread.Sleep(1000);

        }

        /*주식 분봉체크!!*/
        private void SharpIncreaseCheckMinuteChart()
        {

            String[] array = SharpIncrease_stock_list.ToArray();
            

           // LogFileWrite("SharpIncrease_stock_list.Count()== " + SharpIncrease_stock_list.Count());

            if (SharpIncrease_stock_list.Count() > SI_StockMinuteCnt)
            {
                string ObjStockCd = array[SI_StockMinuteCnt];
                string chratgubun = "0";

                if (Code.MinGubuns[Mingubun.SelectedIndex].Code == 3)
                {
                    chratgubun = "3";
                }
                else if (Code.MinGubuns[Mingubun.SelectedIndex].Code == 5)
                {
                    chratgubun = "5";
                }
                Console.WriteLine("chratgubun = " + chratgubun);


               if (Int32.Parse(this.LimitStockCnt.Text) <= stock_buy_list.Count)
               // if (SI_StockMinuteCnt > 5)
                {
                    LogFileWrite("==================급등주 매수 최대 매수 주식수 제한===========");
                    LogFileWrite("매수종목건수 =" + stock_buy_list.Count);
                    LogFileWrite("=============================================================");
                    SI_StockMinuteCnt = SharpIncrease_stock_list.Count();
                    SharpIncreaseCheckMinuteChart();
                    return;
                }
                SI_StockMinuteCnt++;
                

                axKHOpenAPI1.SetInputValue("종목코드", ObjStockCd);
                axKHOpenAPI1.SetInputValue("틱범위", chratgubun);
                axKHOpenAPI1.SetInputValue("수정주가구분", "0");

                int nRet = 0;

                nRet = axKHOpenAPI1.CommRqData("SharpIncreaseCheckMinuteChart", "OPT10080", 0, ObjStockCd );
                System.Threading.Thread.Sleep(5000); /*5초간쉰다..*/

                if (nRet == 0)
                {

                }
                else
                {

                }
            }
            else
            {
                
                SharpIncrease_stock_list.Clear();

                LogFileWrite("SharpIncreaseCheckMinuteChart 종료");
                if (Code.sellModeGubuns[sellModeGubun.SelectedIndex].Code ==3)
                    mTimer.Enabled = true;
                    //realDataGubunflag = true;
                    SellStockList_SI();

            }

        }

        private void SellStockList_SI()
        {

            Console.WriteLine("==================SellStockList_SI=====================");

            /*2017.10.13 위치변경*/
            System.Threading.Thread.Sleep(2000); /*1초간쉰다..*/
            

            string selectedAccount = this.banknum.Text;

            axKHOpenAPI1.SetInputValue("계좌번호", selectedAccount.Trim());
            axKHOpenAPI1.SetInputValue("비밀번호", "");
            axKHOpenAPI1.SetInputValue("비밀번호입력매체구분", "00");
            axKHOpenAPI1.SetInputValue("조회구분", "2");


            int nRet = axKHOpenAPI1.CommRqData("SellStockList_SI", "OPW00018", 0, "00018");
            System.Threading.Thread.Sleep(1000); /*1초간쉰다..*/
            if (nRet == 0)
            {
                this.listBox1.Items.Add("SellStockList_SI 성공");
            }
            else
            {
                this.listBox1.Items.Add("SellStockList_SI 실패");
            }


        }

        /*주식 매도시에 분봉체크!!*/
        private void sellStockChart_SI()
        {


            LogFileWrite("==================sellStockChart_SI=====================");
            LogFileWrite("SharpIncrease_sell_list.Count() = " + SharpIncrease_sell_list.Count());
            LogFileWrite("SI_sellStockMinuteCnt = " + SI_sellStockMinuteCnt);


            if (SharpIncrease_sell_list.Count() == 0)
            {
                SI_StockMinuteCnt = 0;
                return;
            }

            if (SharpIncrease_sell_list.Count() != SI_sellStockMinuteCnt)
            {
                string ObjStockCd = "";
                int ObjCnt = 0;
                var tmp = SharpIncrease_sell_list.ElementAt(SI_sellStockMinuteCnt);
                ObjStockCd = tmp.Key;
                ObjCnt = tmp.Value;
                string chratgubun = "0";

                if (Code.MinGubuns[Mingubun.SelectedIndex].Code == 3)
                {
                    chratgubun = "3";
                }
                else if (Code.MinGubuns[Mingubun.SelectedIndex].Code == 5)
                {
                    chratgubun = "5";
                }
                Console.WriteLine("chratgubun = " + chratgubun);
                Console.WriteLine("ObjCnt = " + ObjCnt);

                LogFileWrite("==================sellStockChart_SI 전송=====================" + ObjStockCd);

                axKHOpenAPI1.SetInputValue("종목코드", ObjStockCd);
                axKHOpenAPI1.SetInputValue("틱범위", chratgubun);
                axKHOpenAPI1.SetInputValue("수정주가구분", "0");
                SI_sellStockMinuteCnt++;

                int nRet = 0;

                nRet = axKHOpenAPI1.CommRqData("sellStockChart_SI", "OPT10080", 0, ObjStockCd + "|" + ObjCnt);
                System.Threading.Thread.Sleep(5000); /*5초간쉰다..*/

                if (nRet == 0)
                {
                    LogFileWrite("sellStockChart_SI 성공!! 종목코드 = " + ObjStockCd);
                }
                else
                {
                    LogFileWrite("sellStockChart_SI 실패!! 종목코드 = " + ObjStockCd);
                }
            }
            else
            {
                SI_StockMinuteCnt = 0;
                SI_sellStockMinuteCnt = 0;
                SharpIncrease_sell_list.Clear();
                Console.WriteLine("sellStockChart_SI 종료");
                LogFileWrite("sellStockChart_SI 종료");
                mTimer.Enabled = true;
                //realDataGubunflag = true;
               // if (Code.sellModeGubuns[sellModeGubun.SelectedIndex].Code == 1 || Code.sellModeGubuns[sellModeGubun.SelectedIndex].Code == 2)
               //     SellStockList();

            }

        }





        public void timer_Elapsed(object sender, EventArgs e)
        {
            timeCnt++;
           if (DateTime.Now.TimeOfDay > TimeSpan.Parse("09:00:00") && DateTime.Now.TimeOfDay < TimeSpan.Parse(this.EndTime.Text))
          {
                checkKospi();

                if (Code.Searchgubuns[dealGubun.SelectedIndex].Code == 5)
                {
                    realDataGubunflag = false;
                    search_10023SharpIncrease();
                    //searchSharpIncrease("001", "1", "2", "1", "00000", "1", "0", "4", "0");

                    // SharpIncrease_stock_list

                }
                else
                {
                    Console.WriteLine("주식장 시작");
                    realDataGubunflag = false;
                    LogFileWrite("주식장 시작 = " + g_kospiGubun);
                    Check_DealPercent();
                }
                

         


            }
            else if(DateTime.Now.TimeOfDay > TimeSpan.Parse("15:20:00") && DateTime.Now.TimeOfDay < TimeSpan.Parse(this.EndTime.Text))
           {
               Console.WriteLine("주식장 종료 10분전");
               LogFileWrite("주식장 종료 10분전");
         
           }
          else if (DateTime.Now.TimeOfDay > TimeSpan.Parse(this.EndTime.Text))
          {
                Console.WriteLine("주식장 종료");
                LogFileWrite("주식장 종료");
                mTimer.Enabled = false;
                sw.Close();
                
                Application.SetSuspendState(PowerState.Hibernate, false, false);
                return;
               //  checkKospi();
                // Check_DealPercent();
            }
          else
          {
              Console.WriteLine("주식장 시작전");
              LogFileWrite("주식장 시작전");
                //SellStockList();
                //checkKospi();
                //Check_DealPercent();
                if (Code.Searchgubuns[dealGubun.SelectedIndex].Code == 5)
                {
                    realDataGubunflag = false;
                    search_10023SharpIncrease();
                    // searchSharpIncrease("001", "1", "1", "5", "00000", "1", "0", "4", "0");
                    //searchSharpIncrease("001", "1", "2", "1", "00000", "1", "0", "4", "0");

                    // SharpIncrease_stock_list

                }




            }


        }

        /*step 1 대상을 구함 */
        public void Req_Foreigner(int flag) /*외인기간별매매상위요청*/
        {
            //if (Code.Searchgubuns[dealGubun.SelectedIndex].Code == 1 || flag == 1 )
            if (flag == 1)
            {
                Console.WriteLine("======코스피 외인기간별매매상위요청========");
                axKHOpenAPI1.SetInputValue("시장구분", "001");
                axKHOpenAPI1.SetInputValue("매매구분", "2");
                axKHOpenAPI1.SetInputValue("기간", "5");
                int nRet = 0;
                nRet = axKHOpenAPI1.CommRqData("Req_Foreigner", "OPT10034", 0, "1002");
                System.Threading.Thread.Sleep(1000);
            }
            //else if (Code.Searchgubuns[dealGubun.SelectedIndex].Code == 2 || flag == 2)
            else if(flag == 2)
            {
                Console.WriteLine("======코스피 외인연속순매매상위요청========");
                axKHOpenAPI1.SetInputValue("시장구분", "001");
                axKHOpenAPI1.SetInputValue("매매구분", "2");
                axKHOpenAPI1.SetInputValue("기준일구분", "1");
                int nRet = 0;
                nRet = axKHOpenAPI1.CommRqData("Req_Foreigner2", "OPT10035", 0, "1002");
                System.Threading.Thread.Sleep(1000);

            }
            //else if (Code.Searchgubuns[dealGubun.SelectedIndex].Code == 4 || flag == 3)
            else if (flag == 3)
            {
                Console.WriteLine("======코스피 외국계창구매매상위요청========");
                axKHOpenAPI1.SetInputValue("시장구분", "001");
                axKHOpenAPI1.SetInputValue("기간", "10");
                axKHOpenAPI1.SetInputValue("매매구분", "1");
                axKHOpenAPI1.SetInputValue("정렬구분", "1");
                axKHOpenAPI1.SetInputValue("현재가조건", "");
                int nRet = 0;
                nRet = axKHOpenAPI1.CommRqData("Req_Foreigner3", "OPT10037", 0, "1002");
                System.Threading.Thread.Sleep(1000);

            }else if (flag == 4)
            {
                Console.WriteLine("======코스닥 외인기간별매매상위요청========");
                axKHOpenAPI1.SetInputValue("시장구분", "101");
                axKHOpenAPI1.SetInputValue("매매구분", "2");
                axKHOpenAPI1.SetInputValue("기간", "5");
                int nRet = 0;
                nRet = axKHOpenAPI1.CommRqData("Req_Foreigner4", "OPT10034", 0, "1002");
                System.Threading.Thread.Sleep(1000);
            }
            else if (flag == 5)
            {
                Console.WriteLine("======코스닥 외인연속순매매상위요청========");
                axKHOpenAPI1.SetInputValue("시장구분", "101");
                axKHOpenAPI1.SetInputValue("매매구분", "2");
                axKHOpenAPI1.SetInputValue("기준일구분", "1");
                int nRet = 0;
                nRet = axKHOpenAPI1.CommRqData("Req_Foreigner5", "OPT10035", 0, "1002");
                System.Threading.Thread.Sleep(1000);

            }
            //else if (Code.Searchgubuns[dealGubun.SelectedIndex].Code == 4 || flag == 3)
            else if (flag == 6)
            {
                Console.WriteLine("======코스닥 외국계창구매매상위요청========");
                axKHOpenAPI1.SetInputValue("시장구분", "101");
                axKHOpenAPI1.SetInputValue("기간", "10");
                axKHOpenAPI1.SetInputValue("매매구분", "1");
                axKHOpenAPI1.SetInputValue("정렬구분", "1");
                axKHOpenAPI1.SetInputValue("현재가조건", "");
                int nRet = 0;
                nRet = axKHOpenAPI1.CommRqData("Req_Foreigner6", "OPT10037", 0, "1002");
                System.Threading.Thread.Sleep(1000);

            }
            //else if (Code.Searchgubuns[dealGubun.SelectedIndex].Code == 4 || flag == 3)
            else if (flag == 7)
            {
                Console.WriteLine("======코스피 매매상위요청========");
                axKHOpenAPI1.SetInputValue("시장구분", "001");
                axKHOpenAPI1.SetInputValue("기간", "5");
                int nRet = 0;
                nRet = axKHOpenAPI1.CommRqData("Req_Foreigner7", "OPT10036", 0, "1002");
                System.Threading.Thread.Sleep(1000);

            }
            //else if (Code.Searchgubuns[dealGubun.SelectedIndex].Code == 4 || flag == 3)
            else if (flag == 8)
            {
                Console.WriteLine("======코스닥 매매상위요청========");
                axKHOpenAPI1.SetInputValue("시장구분", "101");
                axKHOpenAPI1.SetInputValue("기간", "5");
                int nRet = 0;
                nRet = axKHOpenAPI1.CommRqData("Req_Foreigner8", "OPT10036", 0, "1002");
                System.Threading.Thread.Sleep(1000);

            }
            else if (flag == 9)
            {
                Console.WriteLine("======코스피 일별기관매매종목요청========");
                DateTime tmpdt1 = DateTime.Now;
                DateTime tmpdt2 = DateTime.Now.AddDays(-20);
                string fromDate = tmpdt2.ToString("yyyyMMdd");
                string tomDate = tmpdt1.ToString("yyyyMMdd");
                
                axKHOpenAPI1.SetInputValue("시작일자", fromDate);
                axKHOpenAPI1.SetInputValue("종목일자", tomDate);
                axKHOpenAPI1.SetInputValue("매매구분", "2");
                axKHOpenAPI1.SetInputValue("시장구분", "001");
                int nRet = 0;
                nRet = axKHOpenAPI1.CommRqData("Req_Foreigner9", "OPT10044", 0, "1002");
                System.Threading.Thread.Sleep(1000);

            }
            //else if (Code.Searchgubuns[dealGubun.SelectedIndex].Code == 4 || flag == 3)
            else if (flag == 10)
            {
                Console.WriteLine("======코스닥 일별기관매매종목요청========");
                DateTime tmpdt1 = DateTime.Now;
                DateTime tmpdt2 = DateTime.Now.AddDays(-20);
                string fromDate = tmpdt2.ToString("yyyyMMdd");
                string tomDate = tmpdt1.ToString("yyyyMMdd");

                axKHOpenAPI1.SetInputValue("시작일자", fromDate);
                axKHOpenAPI1.SetInputValue("종목일자", tomDate);
                axKHOpenAPI1.SetInputValue("매매구분", "2");
                axKHOpenAPI1.SetInputValue("시장구분", "101");
                int nRet = 0;
                nRet = axKHOpenAPI1.CommRqData("Req_Foreigner10", "OPT10044", 0, "1002");
                System.Threading.Thread.Sleep(1000);

            }


        }

        public Form1()
        {
            InitializeComponent();
            mTimer.Tick += new EventHandler(timer_Elapsed);
            //로그파일
            DateTime dt1 = DateTime.Now;
            string fromDate = dt1.ToString("yyyyMMdd");
            sw = new StreamWriter("C:/StockLog/" + fromDate + "stock" + ".log");
            Data20Line = new Dictionary<string, int>();
            DataWeek = new Dictionary<string, WeekChart>();
            DataDay = new Dictionary<string, WeekChart>();
            DataMinute = new Dictionary<string, WeekChart>();
            

            this.axKHOpenAPI1.OnEventConnect += this.axKHOenAPI_OnEventConnect;
            this.axKHOpenAPI1.OnReceiveTrData += this.axKHOpenAPI_OnReceiveTrData;
            this.axKHOpenAPI1.OnReceiveRealData += this.axKHOpenAPI_OnReceiveRealData;
            this.autoStocStopkButton.Click += new EventHandler(Button_Click);
            this.axKHOpenAPI1.OnReceiveChejanData += this.axKHOpenAPI_OnReceiveChejanData;
            initComboBox();

        }

        private void initComboBox()
        {
            

            for (int i = 0; i < Code.Searchgubuns.Length; i++)
            {
                this.dealGubun.Items.Add(Code.Searchgubuns[i].Name);

            }

            dealGubun.SelectedIndex = 0;

            for (int i = 0; i < Code.RealDataGubuns.Length; i++)
            {
                this.realdatagubun.Items.Add(Code.RealDataGubuns[i].Name);

            }
            realdatagubun.SelectedIndex = 0;


            for (int i = 0; i < Code.MinGubuns.Length; i++)
            {
                this.Mingubun.Items.Add(Code.MinGubuns[i].Name);

            }
            Mingubun.SelectedIndex = 0;

            /*급등주 추가*/
            for (int i = 0; i < Code.PriceGubuns.Length; i++)
            {
                this.priceStrdGubun.Items.Add(Code.PriceGubuns[i].Name);

            }
            priceStrdGubun.SelectedIndex = 0;

            for (int i = 0; i < Code.DealAmountGubuns.Length; i++)
            {
                this.dealAmountGubun_SI.Items.Add(Code.DealAmountGubuns[i].Name);

            }
            dealAmountGubun_SI.SelectedIndex = 0;

            for (int i = 0; i < Code.MarketGubuns.Length; i++)
            {
                this.MarketGubun.Items.Add(Code.MarketGubuns[i].Name);
            }
            MarketGubun.SelectedIndex = 0;

            for (int i = 0; i < Code.sellModeGubuns.Length; i++)
            {
                this.sellModeGubun.Items.Add(Code.sellModeGubuns[i].Name);
            }
            sellModeGubun.SelectedIndex = 0;
        }

        private void comboBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            string selectedAccount = (string)comboBox.SelectedItem;

            axKHOpenAPI1.SetInputValue("계좌번호", selectedAccount.Trim());
            axKHOpenAPI1.SetInputValue("비밀번호", "");
            axKHOpenAPI1.SetInputValue("상장폐지조회구분", "0");
            axKHOpenAPI1.SetInputValue("비밀번호입력매체구분", "00");

            this.listBox4.Items.Clear();

            int nRet = axKHOpenAPI1.CommRqData("계좌평가현황요청", "OPW00004", 0, "6001");


        }

        private void Button_Click(object sender, EventArgs e)
        {
            if (sender.Equals(this.autoStocStartkButton))
            {
                AutoBuyStart();
                
                
                // Debug.WriteLine(temp);


            }
            else if (sender.Equals(this.autoStocStopkButton))
            {
                AutoBuyStop();

            }
            else if (sender.Equals(this.loginButton))
            {
                if (axKHOpenAPI1.CommConnect() == 0)
                {
                    this.listBox1.Items.Add("로그인 시작");

                }
                else
                {
                    this.listBox1.Items.Add("로그인 실패");

                }
            }else if (sender.Equals(this.logoutButton))
            {
                axKHOpenAPI1.CommTerminate();
                this.listBox1.Items.Add("로그아웃");
            }


           

        }



        private void axKHOenAPI_OnEventConnect(object sender,AxKHOpenAPILib._DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            if(e.nErrCode == 0)
            {
                this.listBox1.Items.Add("로그인 시작");
                if(this.axKHOpenAPI1.GetConnectState() == 1)
                {

                    this.listBox1.Items.Add("접속상태:연결중");

                }
                else if(this.axKHOpenAPI1.GetConnectState() == 0)
                {
                    this.listBox1.Items.Add("접속상태:미연결");
                }
                getUserInfo();
            }
            else
            {
                this.listBox1.Items.Add("로그인 실패");
            }
        }

        private void getUserInfo()
        {
            this.userID.Text = axKHOpenAPI1.GetLoginInfo("USER_ID");
            this.userName.Text = axKHOpenAPI1.GetLoginInfo("USER_NAME");

            string[] acountArray = axKHOpenAPI1.GetLoginInfo("ACCNO").Split(';');
            this.banknum.Items.AddRange(acountArray);
            this.banknum.SelectedIndex = 0;

        }

        private void axKHOpenAPI_OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {

            if (e.sRQName == "주식기본정보")
            {
                int i = 0;

                string obj_stock_cd = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목코드").Trim();
                string obj_stock_cd_nm = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목명").Trim();
                string obj_stock_deal_amount = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "거래량").Trim();
                string obj_stock_real_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "시가").Trim();
                string obj_stock_high_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "고가").Trim();
                string obj_stock_low_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "저가").Trim();
                string obj_stock_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "현재가").Trim();
                string obj_stock_percent = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "등락율").Trim();
                string obj_stock_profit = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "영업이익").Trim();
                string obj_danggi_stock_profit = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "당기순이익").Trim();
                string obj_per = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "PER").Trim();
                string obj_low250 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "250최저").Trim();
                string obj_high250 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "250최고").Trim();
                string obj_EV = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "EV").Trim();
                string obj_Foreighner_percent = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "외인소진률").Trim();

                if (axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "영업이익").Trim().Length == 0)
                {
                    obj_stock_profit = "0";
                }
                if (obj_danggi_stock_profit == "")
                {
                    obj_danggi_stock_profit = "0";
                }

                float increase = 0;
                if (axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "등락율").Trim() == "")
                {
                    increase = 0;
                }
                else
                {
                    increase = float.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "등락율").Trim());
                }

                if (obj_per == "")
                {
                    obj_per = "0";
                }
                if (obj_low250 == "")
                {
                    obj_low250 = "0";
                }
                if (obj_high250 == "")
                {
                    obj_high250 = "0";
                }
                if (obj_EV == "")
                {
                    obj_EV = "0";
                }
                if (obj_Foreighner_percent == "") obj_Foreighner_percent = "0";

                float f_obj_stock_real_price = float.Parse(obj_stock_real_price);
                float f_obj_low250 = float.Parse(obj_low250);
                float f_obj_high250 = float.Parse(obj_high250);
                float LowPerCurrent = 0;
                float HighPerCurrent = 0;

                if (obj_low250 != "0" && obj_high250 != "0")
                {
                    LowPerCurrent = f_obj_stock_real_price / f_obj_low250 * 100; // 130 이상  저점대비 30%이상 상승
                    HighPerCurrent = f_obj_stock_real_price / f_obj_high250 * 100; // 80 이상 고점대비 30%이하
                }


                
                if (Int32.Parse(obj_stock_profit) > Int32.Parse(StockBenefit.Text)
                    && Int32.Parse(obj_danggi_stock_profit) > 0
                    && float.Parse(obj_per) > 3
                    && Math.Abs(float.Parse(obj_Foreighner_percent)) > 5
                    // && float.Parse(obj_per) < 40
                    //  && Math.Abs(LowPerCurrent) > 120
                    //  && Math.Abs(HighPerCurrent) > 70
                    )
                {
                    StockListReq_Foreigner crf = new StockListReq_Foreigner();
                    Console.WriteLine("=======================================");
                    Console.WriteLine("영업이익 + 종목명 = " + obj_stock_cd_nm);
                    Console.WriteLine("영업이익 = " + obj_stock_profit);
                    Console.WriteLine("=======================================");
                    LogFileWrite("=======================================");
                    LogFileWrite("영업이익 + 종목명 = " + obj_stock_cd_nm);
                    LogFileWrite("영업이익 = " + obj_stock_profit);
                    LogFileWrite("현재가 = " + f_obj_stock_real_price);
                    LogFileWrite("250최저가 = " + f_obj_low250);
                    LogFileWrite("250최고가 = " + f_obj_high250);
                    LogFileWrite("250최저점대비가격 = " + LowPerCurrent);
                    LogFileWrite("250최고점대비가격 = " + HighPerCurrent);
                    LogFileWrite("외인소진률 = " + obj_Foreighner_percent);
                    LogFileWrite("=======================================");
                    crf.SetData(obj_stock_cd, obj_stock_cd_nm, obj_stock_deal_amount, obj_stock_price, obj_stock_percent, obj_stock_profit);
                    Foreigner_list.Add(crf);
                }
                else
                {
                    // stock_list_2.Remove(obj_stock_cd);
                }
                System.Threading.Thread.Sleep(1000); /*1초간쉰다..*/
                Stock_cd_info();

            }
            else if (e.sRQName == "주식일봉차트조회")
            {
                int nCnt = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);

            }
            else if (e.sRQName == "계좌평가현황요청" || e.sRQName == "StockBuyListSearch" || e.sRQName == "BankStockcdSearch")
            {
                this.listBox4.Items.Add("예수금 = " + Int32.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "예수금").Trim()));
                this.listBox4.Items.Add("D+2추정예수금 = " + Int32.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "D+2추정예수금").Trim()));
                this.listBox4.Items.Add("총매입금액 = " + Int32.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "총매입금액").Trim()));
                this.listBox4.Items.Add("당일손익율 = " + Int32.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "당일손익율").Trim()));
                this.listBox4.Items.Add("당월손익율 = " + Int32.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "당월손익율").Trim()));
                this.listBox4.Items.Add("누적손익율 = " + Int32.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "누적손익율").Trim()));
                this.listBox4.Items.Add("당일투자손익 = " + Int32.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "당일투자손익").Trim()));
                this.listBox4.Items.Add("당월투자손익 = " + Int32.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "당월투자손익").Trim()));



                //Console.WriteLine("예수금 = " + Int32.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "예수금").Trim()));
                //Console.WriteLine("총매입금액 = " + Int32.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "총매입금액").Trim()));
                LogFileWrite("예수금 = " + Int32.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "예수금").Trim()));
                LogFileWrite("총매입금액 = " + Int32.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "총매입금액").Trim()));
                int TmptotalAmount = Int32.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "총매입금액").Trim());
                int LimitPrice = 0;
                //Console.WriteLine("예탁자산평가금액 = " + Int32.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "예탁자산평가금액").Trim()));
                if (this.totalAmount.Text.Trim() == "")
                {
                    LimitPrice = 15000000;
                }
                else
                {
                    LimitPrice = Int32.Parse(this.totalAmount.Text.Trim());
                }

                int nCnt = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);

                if (Int32.Parse(this.LimitStockCnt.Text) <= nCnt || Int32.Parse(this.LimitStockCnt.Text) <= stock_buy_list.Count)
                {
                    Console.WriteLine("주식매수 제한건수로 매매 불가 매수건수 = " + stock_buy_list.Count);

                } else if (e.sRQName == "StockBuyListSearch" & TmptotalAmount < LimitPrice) /* 계좌번호 예수금 조회후 구매한다.*/
                {
                    /*총매수 금액이 제한금액보다 작을때만 주식을 산다.*/
                    MyBudget = Int32.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "예수금").Trim()); /*나의 예산*/
                    int stock_price = 0;
                    string[] tmp = SplitString(e.sScrNo, "|");

                    stock_price = Int32.Parse(tmp[0]);
                    string l_stock_cd = tmp[1];
                    StockBuy(l_stock_cd, stock_price, MyBudget);
                }




                for (int i = 0; i < nCnt; i++)
                {
                    this.listBox4.Items.Add("종목명 = " + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목명").Trim());
                    this.listBox4.Items.Add("종목코드 = " + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목코드").Trim());
                    this.listBox4.Items.Add("현재가 = " + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "현재가").Trim());
                    this.listBox4.Items.Add("평가금액 = " + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "평가금액").Trim());
                    this.listBox4.Items.Add("손익금액 = " + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "손익금액").Trim());
                    this.listBox4.Items.Add("매입금액 = " + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "매입금액").Trim());
                    this.listBox4.Items.Add("금일매수수량 = " + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "금일매수수량").Trim());
                    this.listBox4.Items.Add("금일매도수량 = " + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "금일매도수량").Trim());
                    this.listBox4.Items.Add("손익률 = " + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "손익률").Trim());
                    //Console.WriteLine("매수 주식코드 = " + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목코드").Trim());
                    string TmpStockCD = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목코드").Trim().Substring(1, axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목코드").Trim().Length - 1);
                    if (stock_buy_list.Contains(TmpStockCD))
                    {

                    } else
                    {
                        /*구매목록에 넣어둔다.*/
                        stock_buy_list.Add(TmpStockCD);
                        Console.WriteLine("매수 주식코드 = " + TmpStockCD);
                        LogFileWrite("현재 매수주식코드 = " + TmpStockCD);
                    }


                }


            }
            /*매수 종목 코드 가져오기*/
            else if (e.sRQName == "Req_Foreigner" || e.sRQName == "Req_Foreigner2" || e.sRQName == "Req_Foreigner3"
                || e.sRQName == "Req_Foreigner4" || e.sRQName == "Req_Foreigner5" || e.sRQName == "Req_Foreigner6"
                 || e.sRQName == "Req_Foreigner7" || e.sRQName == "Req_Foreigner8" || e.sRQName == "Req_Foreigner9"
                 || e.sRQName == "Req_Foreigner10"
                )
            {
                int nCnt = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);
                //Foreiner_Stok fs = new Foreiner_Stok();
                // LogFileWrite("nCnt = " + nCnt);
                for (int i = 0; i < nCnt; i++)
                {
                    string f_stock_cd = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목코드").Trim();
                    string f_stock_cd_nm = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목명").Trim();
                    string f_stock_seq = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "순위").Trim();
                    Console.WriteLine("f_stock_cd = " + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목코드").Trim());
                    
                    if (!stock_list.Contains(f_stock_cd))
                        stock_list.Add(f_stock_cd); 
                }
                if (e.sRQName == "Req_Foreigner")
                {
                    Req_Foreigner(2);
                }
                else if (e.sRQName == "Req_Foreigner2")
                {

                    Req_Foreigner(3);

                }
                else if (e.sRQName == "Req_Foreigner3")
                {
                    Req_Foreigner(4);
                }
                else if (e.sRQName == "Req_Foreigner4")
                {

                    Req_Foreigner(5);

                }
                else if (e.sRQName == "Req_Foreigner5")
                {

                    Req_Foreigner(6);

                }
                else if (e.sRQName == "Req_Foreigner6")
                {
                    Req_Foreigner(7);
                }
                else if (e.sRQName == "Req_Foreigner7")
                {

                    Req_Foreigner(8);

                }
                else if (e.sRQName == "Req_Foreigner8")
                {

                    Req_Foreigner(9);

                }
                else if (e.sRQName == "Req_Foreigner9")
                {

                    Req_Foreigner(10);

                }
                else if (e.sRQName == "Req_Foreigner10")
                {
                    LogFileWrite("초기 분석자료 건수 = "+ stock_list.Count);
                    Decrease_Stock_3_5Day();

                }
                

            }
            // else if (e.sRQName == "searchSharpIncrease")
            else if (e.sRQName == "search_10023SharpIncrease")            
            {
                int nCnt = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);
                //Foreiner_Stok fs = new Foreiner_Stok();
                // LogFileWrite("nCnt = " + nCnt);
                for (int i = 0; i < nCnt; i++)
                {
                    string f_stock_cd = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목코드").Trim();
                    string f_stock_cd_nm = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목명").Trim();
                    string f_stock_seq = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "순위").Trim();
                   // LogFileWrite("종모코드 = " + f_stock_cd + "종목명 = " + f_stock_cd_nm);

                    if (!SharpIncrease_stock_list.Contains(f_stock_cd))
                    {
                        if (f_stock_cd_nm.Contains("KODEX") || f_stock_cd_nm.Contains("TIGER") || f_stock_cd_nm.Contains("KBSTAR")
                            || f_stock_cd_nm.Contains("레버리지") || f_stock_cd_nm.Contains("WTI")
                            || f_stock_cd_nm.Contains("선물")
                            || f_stock_cd_nm.Contains("하나머스트")
                             || f_stock_cd_nm.Contains("ETN")
                             || f_stock_cd_nm.Contains("스팩")
                            )
                        {

                        }else
                        {
                            LogFileWrite("종모코드 = " + f_stock_cd + "종목명 = " + f_stock_cd_nm);
                            SharpIncrease_stock_list.Add(f_stock_cd);
                        }
                        
                    }
                    if (i == 30) break;
                        

                }
                SharpIncreaseCheckMinuteChart();
            }
            else if (e.sRQName == "Decrease_Stock_3_5Day")
            {


                int nCnt = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);
                //Foreiner_Stok fs = new Foreiner_Stok();
                Dictionary<string, int> data = new Dictionary<string, int>();
                int strd_cnt = 0;
                float f_percent_total = 0;
                int f_present_price = 0;
                // Console.WriteLine("종목코드 = " + g_stock_cd);
                for (int i = 0; i < nCnt; i++)
                {
                    string tmpDay = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "일자").Trim();
                    float f_percent = 0;
                    f_present_price = Math.Abs(Int32.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종가").Trim()));
                    if (axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "등락율").Trim().Length == 0)
                    {
                        f_percent = 0;
                    }
                    else
                    {
                        f_percent = float.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "등락율").Trim());
                    }

                    data.Add(tmpDay, f_present_price);

                    if (i == 100) break;

                }
                WeekChart wc = new WeekChart(data);
                DateTime ret;
                DateTime ret2;
                DataDay.Add(g_stock_cd, wc);
                int result1 = 0;
                //int result2 = 0;

                if (wc.GetGoldCrossCompare5to20() != "" && (wc.GetGoldCrossCompare20to60() != ""))
                {
                    DateTime.TryParseExact(wc.GetGoldCrossCompare5to20().ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture
                    , System.Globalization.DateTimeStyles.None, out ret2);
                    DateTime.TryParseExact(wc.GetGoldCrossCompare20to60().ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture
                    , System.Globalization.DateTimeStyles.None, out ret);

                    result1 = DateTime.Compare(ret, ret2);
                    // result2 = DateTime.Compare(ret2, DateTime.Now.AddMonths(-20));
                } else if (wc.GetGoldCrossCompare5to20() != "" && (wc.GetGoldCrossCompare20to60() == ""))
                {
                    result1 = 1;
                }
                else
                {
                    result1 = 0;
                }




                if (((result1 > 0) && wc.Increase5Line() && wc.Increase20Line()) || (wc.WhichBigger5Or20() && wc.WhichBigger20Or60())) //&& wc.Increase5Line() && wc.Increase20Line())
                {
                    LogFileWrite("==========================");
                    LogFileWrite("종목코드 = " + g_stock_cd);
                    LogFileWrite("일봉20-60라인 골드크로스 일자 = " + wc.GetGoldCrossCompare20to60());
                    LogFileWrite("일봉5-20라인 골드크로스 일자 = " + wc.GetGoldCrossCompare5to20());
                    if (wc.Increase5Line())
                    {
                        LogFileWrite("일봉 5선 증가세");
                    }
                    else { LogFileWrite("일봉 5선 감소세"); }

                    if (wc.Increase20Line())
                    {
                        LogFileWrite("일봉 20선 증가세");
                    }
                    else { LogFileWrite("일봉 20선 감소세"); }
                    LogFileWrite("==========================");
                    if (!stock_list_2.Contains(g_stock_cd)) stock_list_2.Add(g_stock_cd);
                }

                System.Threading.Thread.Sleep(1000); /*1초간쉰다..*/
                Decrease_Stock_3_5Day(); /* 일봉 체크 */


            }
            else if (e.sRQName == "Check_DealPercent" || e.sRQName == "SellCheck_DealPercent")
            {
                int i = 0;
                string tmp = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "체결강도").Trim();
                string tmp2 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "시가").Trim();
                string tmp3 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "등락률").Trim();
                if (tmp.Length == 0)
                {
                    tmp = "0";
                }
                if (tmp2.Length == 0)
                {
                    tmp2 = "0";
                }
                if (tmp3.Length == 0)
                {
                    tmp3 = "0";
                }

                if (e.sRQName == "Check_DealPercent")
                {
                    float f_percent = float.Parse(tmp);
                    float f_percent2 = float.Parse(tmp3);
                    int i_price = Int32.Parse(tmp2);
                    //int f_20Lineprice = Data20Line[e.sScrNo];
                    if ((
                         (g_kospiGubun == "GOOD" && f_percent2 < Int32.Parse(buyStrdGood.Text))
                             || (g_kospiGubun == "NORMAL" && f_percent2 < Int32.Parse(buyStrdNormal.Text))
                             || (g_kospiGubun == "BAD" && f_percent2 < Int32.Parse(buyStrdBad.Text)))
                                 && (Int32.Parse(minPrice.Text) < Math.Abs(i_price) && Math.Abs(i_price) < Int32.Parse(maxPrice.Text)))
                    {
                        //매수한다.
                        LogFileWrite("====" + "[" + g_kospiGubun + "]" + "==Buy 체결강도체크=====");
                        LogFileWrite("종목코드 = " + e.sScrNo); /*종목코드*/
                        LogFileWrite("체결강도  = " + tmp);
                        //LogFileWrite("20선 가격  = " + f_20Lineprice);
                        LogFileWrite("현재가  = " + i_price);
                        LogFileWrite("등락률  = " + f_percent2);
                        LogFileWrite("===========");
                        //stock_buy_list.Add(e.sScrNo); 
                        /**/
                        if (stock_buy_list.Contains(e.sScrNo))
                        {
                            Console.WriteLine("stock_buy_list.Contains!!");
                            if(Int32.Parse(DateTime.Now.ToString("HHmm")) >= 1510 && (f_percent2 < -5))
                            {
                                StockBuyListSearch(e.sScrNo, Math.Abs(i_price));
                            }
                            //ObjMinuteCheck.Add(e.sScrNo, Math.Abs(i_price));
                            //구매한건 다시안산다.
                        }
                        else
                        {
                            //StockBuyListSearch(e.sScrNo, Math.Abs(i_price));
                            if (!ObjMinuteCheck.ContainsKey(e.sScrNo))
                            {
                                ObjMinuteCheck.Add(e.sScrNo, Math.Abs(i_price));
                            }
                            
                        }


                    }
                    Check_DealPercent();

                } else if (e.sRQName == "SellCheck_DealPercent")
                {
                    float f_percent = float.Parse(tmp);
                    float f_percent2 = float.Parse(tmp3);
                    float PlusStrd = float.Parse(this.PlusStrd.Text);
                    int i_price = Int32.Parse(tmp2);
                    string[] paramtmp = SplitString(e.sScrNo, "|");

                    string TmpStockCd = paramtmp[0];
                    int TmpCnt = Int32.Parse(paramtmp[1]);
                    LogFileWrite("======Sell 체결강도체크 START=====");
                    LogFileWrite("종목코드 = " + TmpStockCd); /*종목코드*/
                    LogFileWrite("체결강도  = " + tmp);
                    LogFileWrite("등락률  = " + f_percent2);
                    LogFileWrite("======Sell 체결강도체크 END=====");
                    if (f_percent < 150)
                        StockSell(TmpStockCd, TmpCnt, i_price);

                }

            }
            /*팔아야할 목록*/
            else if (e.sRQName == "SellStockList")
            {
                int i = 0;
                string obj_buy_amt = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "총매입금액").Trim();
                string obj_assessment_buy_amt = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "총평가금액").Trim();
                string obj_assessment_buy_profit_amt = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "총평가손익금액").Trim();
                string obj_total_profit_percent = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "총수익률(%)").Trim();
                string obj_estimate_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "추정예탁자산").Trim();
                string obj_borrow_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "총대출금").Trim();
                string obj_loan_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "총융자금액").Trim();
                string obj_lender_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "총대주금액").Trim();
                string obj_cnt = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "조회건수").Trim();

                int tempCnt = Int32.Parse(obj_cnt);

                if (tempCnt > 0)
                {
                    for (int t = 0; t < tempCnt; t++)
                    {

                        string f_stock_cd = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, t, "종목번호").Trim();
                        string f_stock_cd_nm = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, t, "종목명").Trim();
                        string f_stock_profit_percent = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, t, "수익률(%)").Trim();
                        string f_stock_cnt = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, t, "보유수량").Trim();
                        string f_present_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, t, "현재가").Trim();
                        LogFileWrite("=================================");
                        LogFileWrite("매수해서 팔아야하는 종목 = " + f_stock_cd_nm);
                        LogFileWrite("수익률(%) = " + f_stock_profit_percent);
                        f_stock_cd = f_stock_cd.Substring(1, f_stock_cd.Length - 1);
                        LogFileWrite("=================================");
                        //Console.WriteLine("f_stock_cd = " + f_stock_cd);
                        //Console.WriteLine("f_stock_cd_nm = " + f_stock_cd_nm);
                        //Console.WriteLine("f_stock_profit_percent = " + f_stock_profit_percent);

                        float ProfitPercent = float.Parse(f_stock_profit_percent);
                        float PlusStrd = float.Parse(this.PlusStrd.Text);
                        float MinusStrd = float.Parse(this.MinusStrd.Text);
                        int StockCnt = Int32.Parse(f_stock_cnt);
                        int Price = Int32.Parse(f_present_price);
                        //Console.WriteLine("판매 종목이익률 = " + ProfitPercent);
                        if (ProfitPercent > PlusStrd)  /*2%이득 or -2%손해시 매도 */
                        {
                            Console.WriteLine("판매 종목명 = " + f_stock_cd_nm);
                            Console.WriteLine("판매 종목이익률 = " + ProfitPercent);
                            LogFileWrite("====================");
                            LogFileWrite("판매 종목명 = " + f_stock_cd_nm);
                            LogFileWrite("판매 종목이익률 = " + ProfitPercent);

                            SellCheck_DealPercent(f_stock_cd, f_stock_cnt);

                        }
                        else if (ProfitPercent < MinusStrd) /*손절*/
                        {
                            LogFileWrite("====================");
                            LogFileWrite("판매 종목명 = " + f_stock_cd_nm);
                            LogFileWrite("판매 종목이익률 = " + ProfitPercent);
                            StockSell(f_stock_cd, StockCnt, Price);

                        }

                    }

                }

            }

            /*급등주 매도 체크로직*/
            else if (e.sRQName == "SellStockList_SI")
            {
                int i = 0;
                string obj_buy_amt = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "총매입금액").Trim();
                string obj_assessment_buy_amt = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "총평가금액").Trim();
                string obj_assessment_buy_profit_amt = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "총평가손익금액").Trim();
                string obj_total_profit_percent = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "총수익률(%)").Trim();
                string obj_estimate_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "추정예탁자산").Trim();
                string obj_borrow_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "총대출금").Trim();
                string obj_loan_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "총융자금액").Trim();
                string obj_lender_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "총대주금액").Trim();
                string obj_cnt = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "조회건수").Trim();

                int tempCnt = Int32.Parse(obj_cnt);

                if (tempCnt > 0)
                {
                    for (int t = 0; t < tempCnt; t++)
                    {

                        string f_stock_cd = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, t, "종목번호").Trim();
                        string f_stock_cd_nm = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, t, "종목명").Trim();
                        string f_stock_profit_percent = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, t, "수익률(%)").Trim();
                        string f_stock_cnt = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, t, "보유수량").Trim();
                        string f_present_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, t, "현재가").Trim();
                        LogFileWrite("=================================");
                        LogFileWrite("매수해서 팔아야하는 종목 = " + f_stock_cd_nm);
                        LogFileWrite("수익률(%) = " + f_stock_profit_percent);
                        f_stock_cd = f_stock_cd.Substring(1, f_stock_cd.Length - 1);
                        LogFileWrite("=================================");
                        //Console.WriteLine("f_stock_cd = " + f_stock_cd);
                        //Console.WriteLine("f_stock_cd_nm = " + f_stock_cd_nm);
                        //Console.WriteLine("f_stock_profit_percent = " + f_stock_profit_percent);

                        float ProfitPercent = float.Parse(f_stock_profit_percent);
                        float PlusStrd = float.Parse(this.PlusStrd_SI.Text);
                        float MinusStrd = float.Parse(this.MinusStrd_SI.Text);
                        int StockCnt = Int32.Parse(f_stock_cnt);
                        int Price = Int32.Parse(f_present_price);
                        //Console.WriteLine("판매 종목이익률 = " + ProfitPercent);
                        if (ProfitPercent < MinusStrd) /*손절*/
                        {
                            LogFileWrite("========손절로 매도============");
                            LogFileWrite("판매 종목명 = " + f_stock_cd_nm);
                            LogFileWrite("판매 종목이익률 = " + ProfitPercent);
                            StockSell(f_stock_cd, StockCnt, Price);

                        }
                        else if (Code.sellModeGubuns[sellModeGubun.SelectedIndex].Code == 1) /*차트 and 최대 매수 기준*/
                        {

                            

                            if (ProfitPercent >= PlusStrd)
                            {
                                LogFileWrite("=========차트 and 최대 매수 기준(이익나서 매도)=========== ");
                                LogFileWrite("판매 종목명 = " + f_stock_cd_nm);
                                LogFileWrite("판매 종목이익률 = " + ProfitPercent);

                                if (!SharpIncrease_sell_list.ContainsKey(f_stock_cd))
                                {
                                    SharpIncrease_sell_list.Add(f_stock_cd, StockCnt);
                                }
                            }

                        }
                        else if (Code.sellModeGubuns[sellModeGubun.SelectedIndex].Code == 2)/*차트만*/
                        {

                            LogFileWrite("=========차트 기준=========== ");

                            if (!SharpIncrease_sell_list.ContainsKey(f_stock_cd))
                                {
                                    SharpIncrease_sell_list.Add(f_stock_cd, StockCnt);
                                }
                            
                                
                        }
                        else if (Code.sellModeGubuns[sellModeGubun.SelectedIndex].Code == 3)/*최대 기준매도 기준*/
                        {

                            LogFileWrite("=========기준매도 기준=========== ");
                            if (ProfitPercent >= PlusStrd)
                            {
                                LogFileWrite("====================");
                                LogFileWrite("판매 종목명 = " + f_stock_cd_nm);
                                LogFileWrite("판매 종목이익률 = " + ProfitPercent);
                                StockSell(f_stock_cd, StockCnt, Price);
                            }
                        }
                        /*매도
                        else if (ProfitPercent >= PlusStrd) 
                        {
                            LogFileWrite("====================");
                            LogFileWrite("판매 종목명 = " + f_stock_cd_nm);
                            LogFileWrite("판매 종목이익률 = " + ProfitPercent);
                            StockSell(f_stock_cd, StockCnt, Price);
                        }
                        */
                        /*2017.10.12 매도시에 매도는 차트는 보지않음 */
                        //else
                        //{
                        // if(!SharpIncrease_sell_list.Contains(f_stock_cd))
                        //   if (!SharpIncrease_sell_list.ContainsKey(f_stock_cd))
                        //    SharpIncrease_sell_list.Add(f_stock_cd, StockCnt);
                        // }

                    }

                }
                /*2017.10.12 매도시에 매도는 차트는 보지않음 */
                if (Code.sellModeGubuns[sellModeGubun.SelectedIndex].Code == 1 || Code.sellModeGubuns[sellModeGubun.SelectedIndex].Code == 2)
                    sellStockChart_SI();

            }

            else if (e.sRQName == "checkKospi")
            {
                int i = 0;
                string tmp = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "등락률").Trim();
                if (tmp == "")
                {
                    tmp = "0";
                }
                float kpspiPercent = float.Parse(tmp);
                if (kpspiPercent < -0.5)
                {
                    Console.WriteLine("장이 안좋은 날");
                    g_kospiGubun = "BAD";
                }
                else if (kpspiPercent > -0.5 && kpspiPercent < 0.5)
                {
                    Console.WriteLine("보통인 날");
                    g_kospiGubun = "NORMAL";
                }
                else
                {
                    Console.WriteLine("좋은 날");
                    g_kospiGubun = "GOOD";
                }

            } else if (e.sRQName == "WeekDataGet")
            {
                int nCnt = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);

                Dictionary<string, int> data = new Dictionary<string, int>();
                string tmpStockCd = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "종목코드").Trim();



                for (int i = 0; i < nCnt; i++)
                {
                    string tmpDay = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "일자").Trim();
                    string f_present_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "현재가").Trim();
                    data.Add(tmpDay, Int32.Parse(f_present_price));
                    if (i == 100) break;

                }


                WeekChart wc = new WeekChart(data);

                DateTime ret;
                DateTime ret2;
                DataWeek.Add(e.sScrNo, wc);
                int result1 = 0;
                if (wc.GetGoldCrossCompare5to20() != "" && (wc.GetGoldCrossCompare20to60() != ""))
                {
                    DateTime.TryParseExact(wc.GetGoldCrossCompare5to20().ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture
                    , System.Globalization.DateTimeStyles.None, out ret2);
                    DateTime.TryParseExact(wc.GetGoldCrossCompare20to60().ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture
                    , System.Globalization.DateTimeStyles.None, out ret);

                    result1 = DateTime.Compare(ret, ret2);
                    // result2 = DateTime.Compare(ret2, DateTime.Now.AddMonths(-20));
                }
                else if (wc.GetGoldCrossCompare5to20() != "" && (wc.GetGoldCrossCompare20to60() == ""))
                {
                    result1 = 1;
                }
                else
                {
                    result1 = 0;
                }


                if (wc.Increase20Line() || wc.WhichBigger20Or60()) //&& wc.Increase5Line() && wc.Increase20Line())
                {
                    LogFileWrite("==========================");
                    LogFileWrite("종목코드 = " + e.sScrNo);
                    LogFileWrite("주봉 20-60라인 골드크로스 일자 = " + wc.GetGoldCrossCompare20to60());
                    LogFileWrite("주봉 5-20라인 골드크로스 일자 = " + wc.GetGoldCrossCompare5to20());
                    if (wc.Increase5Line())
                    {
                        LogFileWrite("주봉 5선 증가세");
                    }
                    else { LogFileWrite("주봉 5선 감소세"); }

                    if (wc.Increase20Line())
                    {
                        LogFileWrite("주봉 20선 증가세");
                    }
                    else { LogFileWrite("주봉 20선 감소세"); }
                    LogFileWrite("==========================");
                    stock_list3.Add(e.sScrNo);
                }


                WeekDataGet();



            }
            else if (e.sRQName == "SharpIncreaseCheckMinuteChart")
            {

                Dictionary<string, int> data = new Dictionary<string, int>();
                string tmpStockCd = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "종목코드").Trim();
                int nCnt = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);

                string l_stock_cd =  e.sScrNo;
                int stock_price = 0;


                for (int i = 0; i < nCnt; i++)
                {
                    string tmpDay = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "체결시간").Trim();
                    string f_present_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "현재가").Trim();
                    // LogFileWrite("CheckMinuteChart 수신!! 가격 = " + Math.Abs(Int32.Parse(f_present_price)));

                    data.Add(tmpDay, Math.Abs(Int32.Parse(f_present_price)));

                    if (i == 0) stock_price = Math.Abs(Int32.Parse(f_present_price));

                    if (i == 100) break;

                }

                WeekChart wc = new WeekChart(data);
                //DataMinute.Add(tmpStockCd, wc);
                //int result1 = 0;

                //if (wc.Increase20Line() || wc.WhichBigger5Or20()) /* 정방향 조건 */
                /*
                 5봉이 20선보다 크고 and 현재가가 5봉보다 큰거
                 */
                float min5to20 = float.Parse(min5div20.Text);
                float max5to20 = float.Parse(max5div20.Text);

                if (wc.WhichBigger5Or20() && (wc.getFirst5Line() < stock_price) 
                   && (wc.get20div5() > min5to20 && wc.get20div5() < max5to20) 
                    ) /*단타용 - 5선이 20선보다 위로올라가면 정방향으로 판단하여 매수*/
                {
                    LogFileWrite("====================!!!매수대상!!!=================");
                    LogFileWrite("CheckMinuteChart 수신!! 종목코드 = " + tmpStockCd);
                    LogFileWrite("3분봉 5선 증가세 = " + wc.Increase5Line());
                    LogFileWrite("3분봉 20선 증가세 = " + wc.Increase20Line());
                    LogFileWrite("5분봉이 20분봉보다 큰가? = " + wc.WhichBigger5Or20());
                    LogFileWrite("5/20 값 = " + wc.get20div5());
                    LogFileWrite("현재가  = " + stock_price);
                    LogFileWrite("=====================================================");


                    StockBuyListSearch(tmpStockCd, Math.Abs(stock_price));

                }else
                {
                    LogFileWrite("====================!!!매수대상 제외!!!=================");
                    LogFileWrite("종목코드 = " + tmpStockCd);
                    LogFileWrite("5분봉이 20분봉보다 큰가? = " + wc.WhichBigger5Or20());
                    LogFileWrite("5분봉 값 = " + wc.getFirst5Line());
                    LogFileWrite("현재가  = " + stock_price);
                    LogFileWrite("5/20 값 = " + wc.get20div5());
                    LogFileWrite("=====================================================");
                }

                System.Threading.Thread.Sleep(1000); /*1초간쉰다..*/
                SharpIncreaseCheckMinuteChart();


            }
            else if (e.sRQName == "sellStockChart_SI")
            {
                Dictionary<string, int> data = new Dictionary<string, int>();
                
                int nCnt = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);
                string tmpStockCd = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "종목코드").Trim();
                string[] tmp = SplitString(e.sScrNo, "|");

                string l_stock_cd = tmp[0];
                int StockCnt = Int32.Parse(tmp[1]);
                int stock_price = 0;


                for (int i = 0; i < nCnt; i++)
                {
                    string tmpDay = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "체결시간").Trim();
                    string f_present_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "현재가").Trim();
                    // LogFileWrite("CheckMinuteChart 수신!! 가격 = " + Math.Abs(Int32.Parse(f_present_price)));

                    data.Add(tmpDay, Math.Abs(Int32.Parse(f_present_price)));

                    if (i == 0) stock_price = Math.Abs(Int32.Parse(f_present_price));

                    if (i == 100) break;

                }

                WeekChart wc = new WeekChart(data);

                
                if (stock_price < wc.getFirst5Line()) /* 5일봉이 20일봉보다 내려가쓸 때 */
                {
                    LogFileWrite("=====================!!매도대상 체크!!================================");
                    LogFileWrite("CheckMinuteChart 수신!! 종목코드 = " + tmpStockCd);
                    LogFileWrite("3분봉 5선 증가세 = " + wc.Increase5Line());
                    LogFileWrite("3분봉 20선 증가세 = " + wc.Increase20Line());
                    LogFileWrite("5분봉이 20분봉보다 큰가? = " + wc.WhichBigger5Or20());
                    LogFileWrite("현재가 = " + stock_price);
                    LogFileWrite("5분봉 가격 = " + wc.getFirst5Line());
                    LogFileWrite("=====================================================");

                    StockSell(tmpStockCd, StockCnt, Math.Abs(stock_price));
                    //StockBuyListSearch(tmpStockCd, Math.Abs(stock_price));

                }

                System.Threading.Thread.Sleep(1000); /*1초간쉰다..*/
                sellStockChart_SI();
                //SIsellStock();


            }
            else if (e.sRQName == "CheckMinuteChart")
            {


                Dictionary<string, int> data = new Dictionary<string, int>();
                string tmpStockCd = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "종목코드").Trim();
                int nCnt = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);
                string[] tmp = SplitString(e.sScrNo, "|");

                string l_stock_cd = tmp[0];
                int stock_price = Int32.Parse(tmp[1]);


                for (int i = 0; i < nCnt; i++)
                {
                    string tmpDay = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "체결시간").Trim();
                    string f_present_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "현재가").Trim();
                    // LogFileWrite("CheckMinuteChart 수신!! 가격 = " + Math.Abs(Int32.Parse(f_present_price)));

                    data.Add(tmpDay, Math.Abs(Int32.Parse(f_present_price)));


                    if (i == 100) break;

                }

                if (data.Count > 20)
                {
                    WeekChart wc = new WeekChart(data);
                    DataMinute.Add(tmpStockCd, wc);
                    int result1 = 0;
                    LogFileWrite("=====================================================");
                    LogFileWrite("CheckMinuteChart 수신!! 종목코드 = " + tmpStockCd);
                    LogFileWrite("3분봉 5선 증가세 = " + wc.Increase5Line());
                    LogFileWrite("3분봉 20선 증가세 = " + wc.Increase20Line());
                    LogFileWrite("5분봉이 20분봉보다 큰가? = " + wc.WhichBigger5Or20());
                    LogFileWrite("20분봉이 60분봉보다 큰가? = " + wc.WhichBigger20Or60());
                    //LogFileWrite("5분/20분 골든크로스 = " + wc.GetGoldCrossCompare5to20());
                    //LogFileWrite("20분/60분 골든크로스 = " + wc.GetGoldCrossCompare20to60());
                    LogFileWrite("=====================================================");
                    if (wc.Increase20Line() || wc.WhichBigger20Or60())
                    {
                        LogFileWrite("===========!!!!!StockBuyListSearch!!!!!===============");
                        LogFileWrite("종목코드 = " + tmpStockCd);


                        StockBuyListSearch(tmpStockCd, Math.Abs(stock_price));

                    }
                }



                System.Threading.Thread.Sleep(1000); /*1초간쉰다..*/
                CheckMinuteChart();
            }
            else if (e.sRQName == "TodayCheckStockList")
            {
                int i = 0;

                string obj_stock_cd = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목코드").Trim();
                string obj_stock_cd_nm = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목명").Trim();
                string obj_stock_deal_amount = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "거래량").Trim();
                string obj_stock_real_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "시가").Trim();
                string obj_stock_high_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "고가").Trim();
                string obj_stock_low_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "저가").Trim();
                string obj_stock_price = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "현재가").Trim();
                string obj_stock_percent = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "등락율").Trim();
                LogFileWrite("TodayCheckStockList = " + e.sScrNo);

                string[] tmpT = { obj_stock_cd_nm, obj_stock_percent , e.sScrNo };

                
                SendInforomation(tmpT);
                TodayCheckStockList();
            }



            //return 1;

        }
        /*주식 분석*/
        /*종목 매수 대상중에 일봉 체크 */
        /*Step 1*/
        public void Decrease_Stock_3_5Day()
        {


            String[] array  = stock_list.ToArray();
            DateTime dt1 = DateTime.Now;
            string fromDate = dt1.ToString("yyyyMMdd");
            

            if (stock_list.Count() != Stock_cnt)
            {

                g_stock_cd = array[Stock_cnt];
                axKHOpenAPI1.SetInputValue("종목코드", g_stock_cd);
                axKHOpenAPI1.SetInputValue("시작일자", fromDate);
                //mTimer.Enabled = false;
                Stock_cnt++;
                int nRet = 0;
                nRet = axKHOpenAPI1.CommRqData("Decrease_Stock_3_5Day", "OPT10015", 0, "1002");

             
            }
           else
            {
                Console.WriteLine("Decrease_Stock_3_5Day 종료");
                LogFileWrite("Decrease_Stock_3_5Day 종료");
                stock_list.Clear();
                Stock_cd_info();
            }

        }
        /* 영업이익 check */
        /*Step 2*/
        public void Stock_cd_info()
        {
            String[] array = stock_list_2.ToArray();

            //Console.WriteLine("Stock_cnt_2 = " + Stock_cnt_2);
            //Console.WriteLine("stock_list_2.Count() = " + stock_list_2.Count());
            if (stock_list_2.Count() != Stock_cnt_2)
            {
                //Console.WriteLine("array[Stock_cnt] = " + array[Stock_cnt]);
                //Console.WriteLine(array[Stock_cnt]);
                g_stock_cd_2 = array[Stock_cnt_2];
                axKHOpenAPI1.SetInputValue("종목코드", g_stock_cd_2);
                //mTimer.Enabled = false;
                Stock_cnt_2++;
                int nRet = 0;
                 nRet = axKHOpenAPI1.CommRqData("주식기본정보", "OPT10001", 0, "1001");

               /* if (nRet == 0)
                {
                    this.listBox1.Items.Add("주식정보요청성공");
                }
                else
                {
                    this.listBox1.Items.Add("주식정보요청실패");
                }*/
            }
            else
            {
                Console.WriteLine("Stock_cd_info 종료");
                
                WeekDataGet();
                //Check_DealPercent();
            }
            
        }
        /*주봉데이터 가져오기 */
        private void WeekDataGet()
        {
            StockListReq_Foreigner[] array = Foreigner_list.ToArray();
            DateTime dt1 = DateTime.Now;
            string toDate = dt1.ToString("yyyyMMdd");
            string fromDate = DateTime.Now.AddYears(-2).ToString("yyyyMMdd");
            if (array.Count() != Stock_cnt_4)
            {
                axKHOpenAPI1.SetInputValue("종목코드", array[Stock_cnt_4].foreiner_obj_stock_cd);
                axKHOpenAPI1.SetInputValue("기준일자", toDate);
                axKHOpenAPI1.SetInputValue("끝일자", fromDate);
                axKHOpenAPI1.SetInputValue("수정주가구분", "0");
                
                int nRet = 0;
                nRet = axKHOpenAPI1.CommRqData("WeekDataGet", "OPT10082", 0, array[Stock_cnt_4].foreiner_obj_stock_cd);
                System.Threading.Thread.Sleep(2000); /*2초간쉰다..*/

                Stock_cnt_4++;

            }
            else
            {
                LogFileWrite("WeekDataGet 종료");
                mTimer.Enabled = true;

                BankStockcdSearch();
            }
        }

        /*외국인 매수대상중에 채결강도 체크*/
        private void Check_DealPercent()
        {

            LogFileWrite("==================Check_DealPercent=====================");
            //_stopped.Set();
            String[] array = stock_list3.ToArray();
            Console.WriteLine("[Stock_cnt_3] = " + Stock_cnt_3  );
            Console.WriteLine("[array.Count()] = " + array.Count());


            if (Code.RealDataGubuns[realdatagubun.SelectedIndex].Code == 2)
            {
                if (array.Count() != Stock_cnt_3)
                {
                    //Console.WriteLine("array[Stock_cnt] = " + array[Stock_cnt]);
                    //Console.WriteLine(array[Stock_cnt]);
                    g_stock_cd_3 = array[Stock_cnt_3];
                    LogFileWrite("==================Check_DealPercent 전송=====================" + g_stock_cd_3);
                    axKHOpenAPI1.SetInputValue("종목코드", g_stock_cd_3);
                    //mTimer.Enabled = false;
                    Stock_cnt_3++;
                    int nRet = 0;
                    nRet = axKHOpenAPI1.CommRqData("Check_DealPercent", "OPT10006", 0, g_stock_cd_3);
                    System.Threading.Thread.Sleep(1000); /*2초간쉰다..*/

                    if (nRet == 0)
                    {
                        LogFileWrite("체결강도 체크 성공!! 종목코드 = " + Stock_cnt_3);
                    }
                    else
                    {
                        LogFileWrite("체결강도 체크 실패!! 종목코드 = " + Stock_cnt_3);
                    }
                }
                else
                {
                    Console.WriteLine("Check_DealPercent 종료");
                    LogFileWrite("Check_DealPercent 종료");
                    //mTimer.Enabled = true; ;
                    Stock_cnt_3 = 0;
                    CheckMinuteChart();
                    // BankStockcdSearch();
                    //Check_DealPercent();
                    //SellStockList();
                }

            }else if (Code.RealDataGubuns[realdatagubun.SelectedIndex].Code == 1)
            {
                LogFileWrite("ObjMinuteCheck 건수 = " + ObjMinuteCheck.Count());
                

                if (ObjMinuteCheck.Count() > 0)
                {
                    Console.WriteLine("Check_DealPercent 실시간데이터!!");
                    LogFileWrite("Check_DealPercent 실시간데이터!!");
                    Stock_cnt_3 = 0;
                    CheckMinuteChart();
                }else
                {
                    realDataGubunflag = true;
                }
                
            }

            

        }


        /*주식 분봉체크!!*/
        private void CheckMinuteChart()
        {

            LogFileWrite("==================CheckMinuteChart=====================");
            //_stopped.Set();
            ;
            LogFileWrite("ObjMinuteCheck.Count()== " + ObjMinuteCheck.Count());

            if (ObjMinuteCheck.Count() != StockMinuteCnt)
            {
                string ObjStockCd = "";
                int ObjPrice = 0;
                var tmp = ObjMinuteCheck.ElementAt(StockMinuteCnt);
                ObjStockCd = tmp.Key;
                ObjPrice = tmp.Value;
                LogFileWrite("==================CheckMinuteChart 전송=====================" + ObjStockCd);
                axKHOpenAPI1.SetInputValue("종목코드", ObjStockCd);
                axKHOpenAPI1.SetInputValue("틱범위", "5");
                axKHOpenAPI1.SetInputValue("수정주가구분", "0");
                //mTimer.Enabled = false;
                StockMinuteCnt++;
                int nRet = 0;
                nRet = axKHOpenAPI1.CommRqData("CheckMinuteChart", "OPT10080", 0, ObjStockCd + "|" + ObjPrice);
                System.Threading.Thread.Sleep(2000); /*2초간쉰다..*/

                if (nRet == 0)
                {
                    LogFileWrite("CheckMinuteChart 성공!! 종목코드 = " + ObjStockCd);
                }
                else
                {
                    LogFileWrite("CheckMinuteChart 실패!! 종목코드 = " + ObjStockCd);
                }
            }
            else
            {
                StockMinuteCnt = 0;
                DataMinute.Clear();
                Console.WriteLine("CheckMinuteChart 종료");
                LogFileWrite("CheckMinuteChart 종료");
                SellStockList();
                
                
                if (Int32.Parse(DateTime.Now.ToString("mm")) >= 50 && Int32.Parse(DateTime.Now.ToString("mm")) <= 53)
                {
                TodayCheckStockList();
                }
                mTimer.Enabled = true;
                realDataGubunflag = true;

            }

        }

        /*주식을 매수한다. 계좌부터 조회*/
        private void StockBuyListSearch(string stock,int price)
        {

            string selectedAccount = this.banknum.Text;
            axKHOpenAPI1.SetInputValue("계좌번호", selectedAccount.Trim());
            axKHOpenAPI1.SetInputValue("비밀번호", "");
            axKHOpenAPI1.SetInputValue("상장폐지조회구분", "0");
            axKHOpenAPI1.SetInputValue("비밀번호입력매체구분", "00");

            this.listBox4.Items.Clear();

            int nRet = axKHOpenAPI1.CommRqData("StockBuyListSearch", "OPW00004", 0, price.ToString() +"|" + stock);
            System.Threading.Thread.Sleep(2000); /*2초간쉰다..*/
        
        }
        /* 현황조회 */
        private void BankStockcdSearch()
        {
            string selectedAccount = this.banknum.Text;
            axKHOpenAPI1.SetInputValue("계좌번호", selectedAccount.Trim());
            axKHOpenAPI1.SetInputValue("비밀번호", "");
            axKHOpenAPI1.SetInputValue("상장폐지조회구분", "0");
            axKHOpenAPI1.SetInputValue("비밀번호입력매체구분", "00");

            this.listBox4.Items.Clear();

            int nRet = axKHOpenAPI1.CommRqData("BankStockcdSearch", "OPW00004", 0, "00004");
            System.Threading.Thread.Sleep(1000); /*1초간쉰다..*/
              if (nRet == 0)
               {
                   LogFileWrite("계좌평가현황요청 성공");
               }
               else
               {
                   LogFileWrite("계좌평가현황요청 실패");
               }
               
        }

        private void StockBuy(string stock_cd , int StockPrice , int MyBudgetPrice)
        {
            string AccountNum = this.banknum.Text;
            string BuyStockCd = stock_cd;
            double TempD = System.Math.Truncate(3000000 * 0.1 / StockPrice);
            int nCnt = Int32.Parse(TempD.ToString()); /*총매수량*/
            //mTimer.Enabled = true;


            LogFileWrite("StockBuy 시작");
            //주문하기
            int lRet;
            if (nCnt == 0 )
            {
                return;
            }

            if (stock_buy_list.Contains(BuyStockCd))
            {
                LogFileWrite("StockBuy!!!SO SKIP!!!" + BuyStockCd);
                return;

            }

            lRet = axKHOpenAPI1.SendOrder("StockBuy", "7001", AccountNum, 1, BuyStockCd, nCnt, 0, "03" , "");
            System.Threading.Thread.Sleep(1000); /*1초간쉰다..*/
            LogFileWrite("============StockBuy===================");
            LogFileWrite("계좌번호 =" + AccountNum);
            LogFileWrite("주문주식코드 =" + BuyStockCd);
            LogFileWrite("매수건수 =" + nCnt);
            LogFileWrite("주식가격 =" + StockPrice);
            LogFileWrite("===============================");
            if (lRet == 0)
            {
                Console.WriteLine("주문이 전송 되었습니다.");
                listBox1.Items.Add("주문이 전송 되었습니다.");
                if (!stock_buy_list.Contains(BuyStockCd))
                {
                    stock_buy_list.Add(BuyStockCd);
                    
                }

            }
            else
            {
                listBox1.Items.Add("주문이 전송 실패 되었습니다.");
                Console.WriteLine("주문이 전송 실패 되었습니다.");
            }
        }

        private void SellStockList()
        {

            Console.WriteLine("==================SellStockList=====================");
           if (stock_buy_list.Count == 0)
            {
                Console.WriteLine("매수한 리스트가 없습니다.");
                return;
            }
            string selectedAccount = this.banknum.Text;

            axKHOpenAPI1.SetInputValue("계좌번호", selectedAccount.Trim());
            axKHOpenAPI1.SetInputValue("비밀번호", "");
            axKHOpenAPI1.SetInputValue("비밀번호입력매체구분","00");
            axKHOpenAPI1.SetInputValue("조회구분", "2");


            int nRet = axKHOpenAPI1.CommRqData("SellStockList", "OPW00018", 0, "00018");
            System.Threading.Thread.Sleep(1000); /*1초간쉰다..*/
            if (nRet == 0)
            {
                this.listBox1.Items.Add("SellStockList 성공");
            }
            else
            {
                this.listBox1.Items.Add("SellStockList 실패");
            }
        
            
        }

        /*외국인 매수대상중에 채결강도 체크*/
        private void SellCheck_DealPercent(string tmpstock_cd, string cnt)
        {
            

            LogFileWrite("============SellCheck_DealPercent START===================");
            LogFileWrite("종목코드 : " + tmpstock_cd);
            LogFileWrite("매도건수 : " + cnt);
            LogFileWrite("============SellCheck_DealPercent END===================");
            axKHOpenAPI1.SetInputValue("종목코드", tmpstock_cd);

           int nRet = 0;
            nRet = axKHOpenAPI1.CommRqData("SellCheck_DealPercent", "OPT10006", 0, tmpstock_cd + "|" + cnt);
            System.Threading.Thread.Sleep(2000); /*1초간쉰다..*/

        }

        /*판매하는 로직*/
        private void StockSell(string stock_cd,int cnt, int price)
        {
            string AccountNum = this.banknum.Text;
            string BuyStockCd = stock_cd;
           // mTimer.Enabled = true;
            int nCnt = cnt;
            int Tprice = price;


            LogFileWrite("StockSell 시작");
            LogFileWrite("============StockBuy===================");
            
            LogFileWrite("주문주식코드 =" + BuyStockCd);
            LogFileWrite("매도수 =" + nCnt);
            LogFileWrite("===============================");
            //주문하기
            int lRet;

            lRet = axKHOpenAPI1.SendOrder("StockSell", "7001", AccountNum, 2, BuyStockCd, nCnt, Tprice, "03", "");
            System.Threading.Thread.Sleep(2000); /*2초간쉰다..*/

            if (lRet == 0)
            {
                Console.WriteLine("매도주문이 전송 되었습니다.");
                listBox1.Items.Add("매도주문이 전송 되었습니다.");
                if (stock_buy_list.Contains(BuyStockCd))
                {
                    stock_buy_list.Remove(BuyStockCd);

                }


            }
            else
            {
                listBox1.Items.Add("주문이 전송 실패 되었습니다.");
                Console.WriteLine("주문이 전송 실패 되었습니다.");
            }
        }

        public void checkKospi()
        {
            string selectedAccount = this.banknum.Text;
            axKHOpenAPI1.SetInputValue("시장구분", "0");
            axKHOpenAPI1.SetInputValue("업종코드", "001");

            this.listBox4.Items.Clear();

            int nRet = axKHOpenAPI1.CommRqData("checkKospi", "OPT20001", 0, "20001");
            System.Threading.Thread.Sleep(2000); /*2초간쉰다..*/
            if (nRet == 0)
            {
                Console.WriteLine("kospi check 정상");
            }
            else
            {
                Console.WriteLine("kospi check 실패");
            }
        }


        private void axKHOpenAPI_OnReceiveChejanData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            if(e.sGubun == "0")
            {

                string fromDate = DateTime.Now.ToString("yyyyMMdd");
                string sendStockCD = axKHOpenAPI1.GetChejanData(9001).Trim().Substring(1, axKHOpenAPI1.GetChejanData(9001).Trim().Length - 1);
                string sendStockCDNM = axKHOpenAPI1.GetChejanData(302).Trim();
                
                string sendStockDealReqPrice = axKHOpenAPI1.GetChejanData(901).Trim();
                string sendStockDealPrice = axKHOpenAPI1.GetChejanData(910).Trim();
                string sendStockDealCnt = axKHOpenAPI1.GetChejanData(911).Trim();
                string sendStockDealGubun = axKHOpenAPI1.GetChejanData(906).Trim();
                string buysellGubun = axKHOpenAPI1.GetChejanData(946);



                string jumunST = axKHOpenAPI1.GetChejanData(913).Trim();
                string jumunCNT = axKHOpenAPI1.GetChejanData(900).Trim();
                string NotDealCNT = axKHOpenAPI1.GetChejanData(902).Trim();
                string jumunGUBUN = axKHOpenAPI1.GetChejanData(905).Trim();
                string maedosueGUBUN = axKHOpenAPI1.GetChejanData(907).Trim();




                string sprofit = "";
                if (axKHOpenAPI1.GetChejanData(8019).Trim() == "")
                {
                     sprofit = "0";
                }else
                {
                    sprofit = float.Parse(axKHOpenAPI1.GetChejanData(8019).Trim()).ToString();
                }




                LogFileWrite("========BUY/SELL LIST-START=====");
                LogFileWrite("일자 = " + fromDate);
                LogFileWrite("매도/매수구분 = " + buysellGubun);
                LogFileWrite("종목코드 = " + sendStockCD);
                LogFileWrite("종목명 = " + sendStockCDNM);
                LogFileWrite("거래건수 = " + sendStockDealCnt);
                LogFileWrite("요청금액 = " + sendStockDealReqPrice);
                LogFileWrite("체결건수 = " + sendStockDealCnt);
                LogFileWrite("체결금액 = " + sendStockDealPrice);
                LogFileWrite("구분 = " + sendStockDealGubun);
                LogFileWrite("손익률 =" + sprofit);
                LogFileWrite("주문상태 =" + jumunST);
                LogFileWrite("주문건수 =" + jumunCNT);
                LogFileWrite("주문구분 =" + jumunGUBUN);
                LogFileWrite("매도수구분 =" + maedosueGUBUN);
                LogFileWrite("========BUY/SELL LIST-END=====");
                
                if (sendStockDealCnt != "" && sendStockDealCnt == jumunCNT) /*체결누적건수와 주문건수와 같을때만 보낸다.*/
                {
                    string[] tmpT = { fromDate, sendStockCD, sendStockCDNM, sendStockDealCnt, sendStockDealReqPrice, sendStockDealPrice, jumunGUBUN, sprofit };
                    SendResult(tmpT);
                }
                
            }
            
        }

        private void axKHOpenAPI_OnReceiveRealData(object sender ,AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            //e.sRealKey; // 종목코드
            //e.sRealType; // 주식시세

            //LogFileWrite("sRealKey = " + e.sRealType);

            // LogFileWrite("종목코드 = " + e.sRealKey); /*종목코드*/
            // String tmpPercent2 = axKHOpenAPI1.GetCommRealData(e.sRealType, 12).Trim().Replace("+", "").Replace("-", "");
            // LogFileWrite("등락률 = " + axKHOpenAPI1.GetCommRealData(e.sRealType, 12).Trim().Replace("+", "").Replace("-", "")); 
            // LogFileWrite("현재가  = " + Int32.Parse(axKHOpenAPI1.GetCommRealData(e.sRealType, 10).Trim()));
           // LogFileWrite("등락률  = " + float.Parse(tmpPercent2));
            if (e.sRealType == "주식체결")
            {
                if (realDataGubunflag)
                {
                    String tmpStockcd = e.sRealKey; /* 주식 코드 */

                    String tmpPercent = axKHOpenAPI1.GetCommRealData(e.sRealType, 12).Trim().Replace("+","");
                    
                    if (axKHOpenAPI1.GetCommRealData(e.sRealType, 10).Trim().Equals(""))
                    {
                        LogFileWrite("종목코드 = " + tmpStockcd); /*종목코드*/
                        LogFileWrite("OnReceiveRealData 현재가  없어서 종료");
                        return;

                    }
                    else
                    {
                    //    LogFileWrite("현재가  = " + Int32.Parse(axKHOpenAPI1.GetCommRealData(e.sRealType, 10).Trim()));
                    }

                    if (tmpPercent.Equals(""))
                    {
                        LogFileWrite("종목코드 = " + tmpStockcd); /*종목코드*/
                        LogFileWrite("OnReceiveRealData 등락률  없어서 종료");
                        return;
                    }
                    else
                    {
                    //    LogFileWrite("등락률  = " + float.Parse(tmpPercent));
                    }



                    if (stock_list3.Contains(tmpStockcd))
                    {
                      //  LogFileWrite("종목코드 = " + tmpStockcd); /*종목코드*/
                    //    LogFileWrite("등락률 = " + tmpPercent); /*종목코드*/

                        float percent = float.Parse(tmpPercent);
                        int price = Int32.Parse(axKHOpenAPI1.GetCommRealData(e.sRealType, 10).Trim());

                        if ((
                            (g_kospiGubun == "GOOD" && percent < Int32.Parse(buyStrdGood.Text))
                                || (g_kospiGubun == "NORMAL" && percent < Int32.Parse(buyStrdNormal.Text))
                                || (g_kospiGubun == "BAD" && percent < Int32.Parse(buyStrdBad.Text)))
                                    && (Int32.Parse(minPrice.Text) < Math.Abs(price) && Math.Abs(price) < Int32.Parse(maxPrice.Text)))
                        {

                            

                            if (stock_buy_list.Contains(tmpStockcd))
                            {

                            }
                            else
                            {
                                //StockBuyListSearch(e.sScrNo, Math.Abs(i_price));
                                if (!ObjMinuteCheck.ContainsKey(tmpStockcd))
                                {
                                    LogFileWrite("====" + "[" + g_kospiGubun + "]" + "==Buy 체결강도체크=====");
                                    LogFileWrite("종목코드 = " + tmpStockcd); /*종목코드*/
                                    LogFileWrite("현재가  = " + price);
                                    LogFileWrite("등락률  = " + percent);
                                    LogFileWrite("===========");
                                    ObjMinuteCheck.Add(tmpStockcd, Math.Abs(price));
                                }

                            }


                        }


                    }
                }
                

            }



        }

        private void SendResult(string[] param)
        {


            StringBuilder postParams = new StringBuilder();
            postParams.Append("DEAL_DATE=" + param[0]);
            postParams.Append("&STOCK_CD=" + param[1]);
            postParams.Append("&STOCK_NM=" + param[2]);
            postParams.Append("&DEAL_STOCK_CNT=" + param[3]);
            postParams.Append("&DEAL_STOCK_REQ_PRICE=" + param[4]);
            postParams.Append("&DEAL_STOCK_PRICE=" + param[5]);
            postParams.Append("&DEAL_GUBUN=" + param[6]);
            postParams.Append("&DEAL_PROFIT=" + param[7]);

            LogFileWrite("========SendResultT=====");
            LogFileWrite("param[0] = " + param[0]);
            LogFileWrite("param[1] = " + param[1]);
            LogFileWrite("param[2] = " + param[2]);
            LogFileWrite("param[3] = " + param[3]);
            LogFileWrite("param[4] = " + param[4]);
            LogFileWrite("param[5] = " + param[5]);
            LogFileWrite("param[6] = " + param[6]);
            LogFileWrite("param[7] = " + param[7]);
            LogFileWrite("========SendResult=====");

            Encoding encoding = Encoding.UTF8;
            byte[] result = encoding.GetBytes(postParams.ToString());

            // 타겟이 되는 웹페이지 URL
            string Url = "http://192.168.25.33:8082/StockListInsert2.jsp";
            HttpWebRequest wReqFirst = (HttpWebRequest)WebRequest.Create(Url);

            // HttpWebRequest 오브젝트 설정
            wReqFirst.Method = "POST";
            wReqFirst.ContentType = "application/x-www-form-urlencoded";
            wReqFirst.ContentLength = result.Length;
            Stream postDataStream = wReqFirst.GetRequestStream();
            postDataStream.Write(result, 0, result.Length);
            postDataStream.Close();
            System.Threading.Thread.Sleep(1000); /*1초간쉰다..*/

            try
            {
                HttpWebResponse wRespFirst = (HttpWebResponse)wReqFirst.GetResponse();
                Stream respPostStream = wRespFirst.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);

                // 생성한 스트림으로부터 string으로 변환합니다.
                string resultPost = readerPost.ReadToEnd();
                LogFileWrite("resultPost = " + resultPost);
            }
            catch (Exception e)
            {
                Console.WriteLine( "e = " +e);
            }
           

        }

        /*외국인 매수대상중에 채결강도 체크*/
        private void TodayCheckStockList()
        {

            LogFileWrite("==================TodayCheckStockList=====================");
            //_stopped.Set();
            String[] array = stock_list3.ToArray();
            Console.WriteLine("TodayCheckStockList [InformStockCnt] = " + InformStockCnt);
            Console.WriteLine("TodayCheckStockList [array.Count()] = " + array.Count());
            string TmpStock = "";


            if (array.Count() != InformStockCnt)
            {

                TmpStock = array[InformStockCnt];
                LogFileWrite("==================TodayCheckStockList 전송=====================" + TmpStock);
                axKHOpenAPI1.SetInputValue("종목코드", TmpStock);
                //mTimer.Enabled = false;
                InformStockCnt++;
                int nRet = 0;
                string gubun = "NO";
                if (array.Count() == InformStockCnt)
                {
                    gubun = "LAST";
                }
                nRet = axKHOpenAPI1.CommRqData("TodayCheckStockList", "OPT10002", 0, gubun);
                System.Threading.Thread.Sleep(1000); /*1초간 쉰다..*/

                if (nRet == 0)
                {
                    LogFileWrite("TodayCheckStockList 체크 성공!! 종목코드 = " + TmpStock);
                }
                else
                {
                    LogFileWrite("TodayCheckStockList 체크 실패!! 종목코드 = " + TmpStock);
                }
            }
            else
            {
                LogFileWrite("TodayCheckStockList 종료");
                InformStockCnt = 0;

            }

        }

        private void SendInforomation(string[] param)
        {

            DateTime dt = DateTime.Now; 
            string s = dt.ToString("yyyyMMdd");
            StringBuilder postParams = new StringBuilder();
            postParams.Append("STOCKNM=" + param[0]);
            postParams.Append("&PERCENT=" + param[1]);
            postParams.Append("&GUBUN=" + param[2]);
            postParams.Append("&SDATE=" + s);

            LogFileWrite("========SendInforomation=====");
            LogFileWrite("param[0] = " + param[0]);
            LogFileWrite("param[1] = " + param[1]);
            LogFileWrite("param[2] = " + param[2]);
            LogFileWrite("s = " + s);
            LogFileWrite("========SendInforomation=====");



            Encoding encoding = Encoding.UTF8;
            byte[] result = encoding.GetBytes(postParams.ToString());

            // 타겟이 되는 웹페이지 URL
            string Url = "http://192.168.25.33:8082/StockListInform.jsp";
            HttpWebRequest wReqFirst = (HttpWebRequest)WebRequest.Create(Url);

            // HttpWebRequest 오브젝트 설정
            wReqFirst.Method = "POST";
            wReqFirst.ContentType = "application/x-www-form-urlencoded";
            wReqFirst.ContentLength = result.Length;
            Stream postDataStream = wReqFirst.GetRequestStream();
            postDataStream.Write(result, 0, result.Length);
            postDataStream.Close();
            System.Threading.Thread.Sleep(1000); /*1초간쉰다..*/

            try
            {
                HttpWebResponse wRespFirst = (HttpWebResponse)wReqFirst.GetResponse();
                Stream respPostStream = wRespFirst.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);

                // 생성한 스트림으로부터 string으로 변환합니다.
                string resultPost = readerPost.ReadToEnd();
                LogFileWrite("resultPost = " + resultPost);
            }
            catch (Exception e)
            {
                Console.WriteLine("e = " + e);
                //                throw;
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /*문자 나누기*/
        public string[] SplitString(String str, String delimStr)
        {
            char[] delimiter = delimStr.ToCharArray();
            return str.Split(delimiter);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            foreach (StockListReq_Foreigner a in Foreigner_list)
            {


            }

                //Console.WriteLine();
        }

        /*주봉데이터 가져오기 */
        private void WeekDataGet2()
        {
            StockListReq_Foreigner[] array = Foreigner_list.ToArray();
            DateTime dt1 = DateTime.Now;
            string toDate = dt1.ToString("yyyyMMdd");
            string fromDate = DateTime.Now.AddYears(-2).ToString("yyyyMMdd");
            //axKHOpenAPI1.SetInputValue("종목코드", "000660");
            //axKHOpenAPI1.SetInputValue("종목코드", "016740");
            //axKHOpenAPI1.SetInputValue("종목코드", "047310");
            axKHOpenAPI1.SetInputValue("종목코드", "130960");

            axKHOpenAPI1.SetInputValue("기준일자", toDate);
            axKHOpenAPI1.SetInputValue("끝일자", fromDate);
            axKHOpenAPI1.SetInputValue("수정주가구분", "0");

            int nRet = 0;
            nRet = axKHOpenAPI1.CommRqData("WeekDataGet", "OPT10082", 0, "000660");
            System.Threading.Thread.Sleep(2000); /*2초간쉰다..*/
            
        }

        private void TestClick(object sender, EventArgs e)
        {
            string[] tmpT = { "20170829", "000030", "SK증권", "0001023", "40000", "40111", "신규매도" ,"-001.45"};
            //float a = float.Parse("-0001.45");
            //Console.WriteLine(a);
            SendResult(tmpT);
            //LogFileWrite("12345");
            //LogFileWrite("테스트");
            //LogFileWrite("ㄱ;ㅁㄱ히냐");
            //WeekDataGet2();
            //string s = "20170911";
            //s = s.ToString("yyyy-MM-dd");


            //int result1 = DateTime.Compare(DateTime.Parse(), DateTime.Now.AddMonths(-1));

            // LogFileWrite("result1");
            // ObjMinuteCheck.Add("092070", 18850);
            // ObjMinuteCheck.Add("195870", 19900);
            // ObjMinuteCheck.Add("066570", 19900);
            // ObjMinuteCheck.Add("011070", 19900);
            // mTimer.Interval = 120000;
            // CheckMinuteChart();
            string[] tmpT2 = {"SK증권","1.5","LAST" };
           // LogFileWrite("xptmxm = " + Int32.Parse(DateTime.Now.ToString("HHmm")));
            SendInforomation(tmpT2);

        }

        public void LogFileWrite(string str)
        {
            //sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + " : " + str);
            //Stockbw = new BinaryWriter(Stockfs);
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + " : " + str);
            //Stockbw.Write(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + " : " + str );
            //Stockbw.Flush();
            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + " : " + str);
            sw.Flush();
            
            
        }

        private void EndTime_TextChanged(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void LimitStockCnt_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
