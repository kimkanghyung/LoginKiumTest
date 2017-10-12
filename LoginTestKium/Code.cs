using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginTestKium
{
    public class Code
    {
        public readonly static TradeSection[] tradeSections = new TradeSection[13];
        public readonly static OrderSection[] orderSections = new OrderSection[6];

        public readonly static Searchgubun[] Searchgubuns = new Searchgubun[5];
        public readonly static RealDataGubun[] RealDataGubuns = new RealDataGubun[2];
        public readonly static MinGubun[] MinGubuns = new MinGubun[2];
        /*급등주로 추가*/
        public readonly static PriceGubun[] PriceGubuns = new PriceGubun[6]; /*0:전체조회, 2:5만원이상, 5:1만원이상, 6:5천원이상, 8:1천원이상, 9:10만원이상*/
        /* 5:5천주이상, 10:만주이상, 50:5만주이상, 100:10만주이상, 200:20만주이상, 300:30만주이상, 500:50만주이상, 1000:백만주이상*/
        public readonly static DealAmountGubun[] DealAmountGubuns = new DealAmountGubun[8];
        /* 시장구분 = 000:전체, 001:코스피, 101:코스닥 */
        public readonly static MarketGubun[] MarketGubuns = new MarketGubun[3];


        public class PriceGubun
        {
            private int codeNum;
            private string name;

            public PriceGubun(int c, string n)
            {
                this.codeNum = c;
                this.name = n;

            }

            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            public int Code
            {
                get
                {
                    return this.codeNum;

                }

            }

        }

        public class DealAmountGubun
        {
            private int codeNum;
            private string name;

            public DealAmountGubun(int c, string n)
            {
                this.codeNum = c;
                this.name = n;

            }

            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            public int Code
            {
                get
                {
                    return this.codeNum;

                }

            }

        }

        public class MarketGubun
        {
            private string code;
            private string name;

            public MarketGubun(string c, string n)
            {
                this.code = c;
                this.name = n;
            }

            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            public string Code
            {
                get
                {
                    return this.code;
                }
            }

        }

        public class TradeSection
        {
            private string code;
            private string name;

            public TradeSection(string c, string n)
            {
                this.code = c;
                this.name = n;
            }

            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            public string Code
            {
                get
                {
                    return this.code;
                }
            }

        }

        public class OrderSection
        {
            private int codeNum;
            private string name;

            public OrderSection(int c, string n)
            {
                this.codeNum = c;
                this.name = n;

            }

            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            public int Code
            {
                get
                {
                    return this.codeNum;

                }

            }
        }

        public class Searchgubun
        {
            private int codeNum;
            private string name;

            public Searchgubun(int c, string n)
            {
                this.codeNum = c;
                this.name = n;

            }

            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            public int Code
            {
                get
                {
                    return this.codeNum;

                }

            }
        }

        public class RealDataGubun
        {
            private int codeNum;
            private string name;

            public RealDataGubun(int c, string n)
            {
                this.codeNum = c;
                this.name = n;

            }

            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            public int Code
            {
                get
                {
                    return this.codeNum;

                }

            }
        }


        public class MinGubun
        {
            private int codeNum;
            private string name;

            public MinGubun(int c, string n)
            {
                this.codeNum = c;
                this.name = n;

            }

            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            public int Code
            {
                get
                {
                    return this.codeNum;

                }

            }
        }

        static Code()
        {
            /*급등주로 추가*/
            /*0:전체조회, 2:5만원이상, 5:1만원이상, 6:5천원이상, 8:1천원이상, 9:10만원이상*/
            PriceGubuns[0] = new PriceGubun(0, "전체조회");
            PriceGubuns[1] = new PriceGubun(2, "5만원이상");
            PriceGubuns[2] = new PriceGubun(5, "1만원이상");
            PriceGubuns[3] = new PriceGubun(6, "5천원이상");
            PriceGubuns[4] = new PriceGubun(8, "1천원이상");
            PriceGubuns[5] = new PriceGubun(9, "10만원이상");

            /* 5:5천주이상, 10:만주이상, 50:5만주이상, 100:10만주이상, 200:20만주이상, 300:30만주이상, 500:50만주이상, 1000:백만주이상*/
            DealAmountGubuns[0] = new DealAmountGubun(5, "5천주이상");
            DealAmountGubuns[1] = new DealAmountGubun(10, "만주이상");
            DealAmountGubuns[2] = new DealAmountGubun(50, "5만주이상");
            DealAmountGubuns[3] = new DealAmountGubun(100, "10만주이상");
            DealAmountGubuns[4] = new DealAmountGubun(200, "20만주이상");
            DealAmountGubuns[5] = new DealAmountGubun(300, "30만주이상");
            DealAmountGubuns[6] = new DealAmountGubun(500, "50만주이상");
            DealAmountGubuns[7] = new DealAmountGubun(1000, "백만주이상");

            /* 시장구분 = 000:전체, 001:코스피, 101:코스닥*/
            MarketGubuns[0] = new MarketGubun("000", "전체");
            MarketGubuns[1] = new MarketGubun("001", "코스피");
            MarketGubuns[2] = new MarketGubun("000", "코스닥");

            Searchgubuns[0] = new Searchgubun(1, "외인기간별매매");
            Searchgubuns[1] = new Searchgubun(2, "외인연속매수");
            Searchgubuns[2] = new Searchgubun(3, "기존매수대상");
            Searchgubuns[3] = new Searchgubun(4, "외국계창구매매상위");
            Searchgubuns[4] = new Searchgubun(5, "급등주로직");

            RealDataGubuns[0] = new RealDataGubun(1, "실시간데이터");
            RealDataGubuns[1] = new RealDataGubun(2, "조회로직데이터");

            MinGubuns[0] = new MinGubun(3 , "3분봉");
            MinGubuns[1] = new MinGubun(5 , "5분봉");

            orderSections[0] = new OrderSection(1, "신규매수");
            orderSections[1] = new OrderSection(1, "신규매도");
            orderSections[2] = new OrderSection(1, "매수취소");
            orderSections[3] = new OrderSection(1, "매도취소");
            orderSections[4] = new OrderSection(1, "매수정정");
            orderSections[5] = new OrderSection(1, "매도정상");

            tradeSections[0] = new TradeSection("00","지정가");
            tradeSections[1] = new TradeSection("03", "시장가");
            tradeSections[2] = new TradeSection("05", "조건부지정가");
            tradeSections[3] = new TradeSection("06", "최유리지정가");
            tradeSections[4] = new TradeSection("07", "최우선지정가");
            tradeSections[5] = new TradeSection("10", "지정가IOC");
            tradeSections[6] = new TradeSection("13", "시장가IOC");
            tradeSections[7] = new TradeSection("16", "최유리IOC");
            tradeSections[8] = new TradeSection("20", "지정가FOK");
            tradeSections[9] = new TradeSection("23", "시장가FOK");
            tradeSections[10] = new TradeSection("26", "최유리FOK");
            tradeSections[11] = new TradeSection("61", "시간외단일가매매");
            tradeSections[12] = new TradeSection("81", "시간외종가");
            


        }

    }
}
