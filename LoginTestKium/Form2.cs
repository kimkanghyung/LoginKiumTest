using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginTestKium
{
    public partial class Form2 : Form
    {
        List<ULPEntityObject> ulpList;

        public Form2()
        {
            InitializeComponent();
            this.axKHOpenAPI1.OnEventConnect += this.axKHOpenAPI1_OnEventConnect;
            this.axKHOpenAPI1.OnReceiveTrData += this.axKHOpenAPI1_OnReceiveTrData;
            if (axKHOpenAPI1.CommConnect() != 0 ) System.Windows.Forms.MessageBox.Show("로그인 실패");

        }

        private void axKHOpenAPI1_OnEventConnect(object sendser, AxKHOpenAPILib._DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            if (e.nErrCode == 0)
            {
                requestPList();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("로그인 실패");
            }
        }

        private void requestPList()
        {
            this.axKHOpenAPI1.SetInputValue("시장구분", "000");
            this.axKHOpenAPI1.SetInputValue("상하한구분", "1");
            this.axKHOpenAPI1.SetInputValue("정렬구분", "3");
            this.axKHOpenAPI1.SetInputValue("종목조건", "10");
            this.axKHOpenAPI1.SetInputValue("거래량구분", "00000");
            this.axKHOpenAPI1.SetInputValue("신용조건", "0");
            this.axKHOpenAPI1.SetInputValue("매매금구분", "0");

            this.axKHOpenAPI1.CommRqData("상하한가요청", "OPT10017", 0, "5001");
        }

        private void axKHOpenAPI1_OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            if(e.sRQName == "상하한가요청")
            {
                int nCnt = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);

                if(ulpList == null)
                {
                    ulpList = new List<ULPEntityObject>();
                }

                for(int i = 0; i < nCnt; i++)
                {
                    ulpList.Add(new ULPEntityObject()
                    {
                        종목명 = axKHOpenAPI1.GetCommData(e.sTrCode,e.sRQName,i,"종목명").Trim(),
                        현재가 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "현재가").Trim(),
                        대비 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "전일대비").Trim(),
                        등락률 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "등락률").Trim(),
                        거래량 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "거래량").Trim(),
                        매수잔량 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "매수잔량").Trim(),
                        연속 = Int32.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "횟수").Trim())

                    });

                    ulpDataGridView.Rows.Add();
                    ulpDataGridView["종목명", i].Value = ulpList[i].종목명;
                    ulpDataGridView["현재가", i].Value = ulpList[i].현재가;
                    ulpDataGridView["대비", i].Value = ulpList[i].대비;
                    ulpDataGridView["등락률", i].Value = ulpList[i].등락률;
                    ulpDataGridView["거래량", i].Value = ulpList[i].거래량;
                    ulpDataGridView["매수잔량", i].Value = ulpList[i].매수잔량;
                    ulpDataGridView["연속", i].Value = ulpList[i].연속;

                    ulpDataGridView["현재가", i].Style.ForeColor = Color.Red;
                    ulpDataGridView["대비", i].Style.ForeColor = Color.Red;
                    ulpDataGridView["등락률", i].Style.ForeColor = Color.Red;

                }
                textBox1.Text = "상한가 [" + ulpList.Count + "종목]";
            }
        }
    }

    class ULPEntityObject
    {
        public string 종목명 { get; set; }
        public string 현재가 { get; set; }
        public string 대비 { get; set; }
        public string 등락률 { get; set; }
        public string 거래량 { get; set; }
        public string 매수잔량 { get; set; }
        public int 연속 { get; set; }

    }
}
