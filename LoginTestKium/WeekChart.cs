using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginTestKium
{
    class WeekChart
    {
        Dictionary<string, int> data;
        Dictionary<string, int> data60Line;
        Dictionary<string, int> data20Line;
        Dictionary<string, int> data5Line;
        Dictionary<string, int> data20than60;
      //  Dictionary<string, int> data60than20;
        
        Dictionary<string, int> data5than20;
        Dictionary<string, int> data20than5;
        bool flag5;
        bool flag20;
        int Price20line = 0;

        
        public WeekChart(Dictionary<string, int> tmp)
        {
            data = new Dictionary<string, int>();
            data5Line = new Dictionary<string, int>();
            data20Line = new Dictionary<string, int>();
            data60Line = new Dictionary<string, int>();

            data20than60 = new Dictionary<string, int>();
           // data60than20 = new Dictionary<string, int>();

            data5than20 = new Dictionary<string, int>();
            data20than5 = new Dictionary<string, int>();
            data = tmp;
            WeekAnalyse();

        }

        public void WeekAnalyse()
        {
            int Strd20Cnt = 0;
            int Strd5Cnt = 0;

            /*5일선 대상*/
            for (int i = 0; i < data.Count; i++)
            {
                int tmp5linePrice = 0;
                var tmp = data.ElementAt(i);

                if ((i + 6) < data.Count)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        var tmp2 = data.ElementAt(i + j);
                        tmp5linePrice = tmp5linePrice + tmp2.Value;
                    }
                    data5Line.Add(tmp.Key, tmp5linePrice / 5);
                }

            }

            

            /*20일선*/
            for (int i = 0; i < data.Count; i++)
            {
                int tmp20linePrice = 0;
                var tmp = data.ElementAt(i);

                if ((i + 21) < data.Count)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        var tmp2 = data.ElementAt(i + j);
                        tmp20linePrice = tmp20linePrice + tmp2.Value;
                    }
                    data20Line.Add(tmp.Key, tmp20linePrice / 20);
                }

            }

           

            /*60일선*/
            for (int i = 0; i < data.Count; i++)
            {
                int tmp60linePrice = 0;
                var tmp = data.ElementAt(i);
                

                if( (i+61) < data.Count)
                {
                    for (int j = 0; j < 60; j++)
                    {
                        var tmp2 = data.ElementAt(i + j);
                        tmp60linePrice = tmp60linePrice + tmp2.Value;
                    }
                    data60Line.Add(tmp.Key, tmp60linePrice / 60);
                }
                
            }
            

            foreach (var r in data20Line)
            {
                
                
                if(data60Line.ContainsKey(r.Key))
                {

                    if(r.Value > data60Line[r.Key])
                    {
                        data20than60.Add(r.Key , 1);
                    }
                    else
                    {
                        data20than60.Add(r.Key, 0);
                    }
                }

            }

            foreach (var r in data5Line)
            {
                //int i = 0;

                if (data20Line.ContainsKey(r.Key))
                {
                    //Console.WriteLine("===================================");
                    //Console.WriteLine("날짜 : " + r.Key);
                    //Console.WriteLine("5일자가격 : " + r.Value);
                    //Console.WriteLine("20일자가격 : " + data20Line[r.Key]);
                    //Console.WriteLine("===================================");
                    if (r.Value > data20Line[r.Key])
                    {
                        data5than20.Add(r.Key, 1);
                    }
                    else
                    {
                        data5than20.Add(r.Key, 0);
                    }
                }

            }


        }

        public string GetGoldCrossCompare20to60()
        {
           string goldcrossdate = "";

            for (int i = 0; i < data20than60.Count; i ++ )
           {
                if (i < data20than60.Count -1)
                {
                    var tmp = data20than60.ElementAt(i);
                    var Comparetmp = data20than60.ElementAt(i + 1);
                    if (tmp.Value != Comparetmp.Value && tmp.Value == 1)
                    {
                        goldcrossdate = tmp.Key;
                        break;

                    }
                }
                
           }

            return goldcrossdate;
        }

        public string DeathCrossCompare20to60()
        {
            string goldcrossdate = "";

            for (int i = 0; i < data20than60.Count; i++)
            {
                if (i < data20than60.Count - 1)
                {
                    var tmp = data20than60.ElementAt(i);
                    var Comparetmp = data20than60.ElementAt(i + 1);
                    if (tmp.Value != Comparetmp.Value && tmp.Value == 0)
                    {
                        goldcrossdate = tmp.Key;
                        break;

                    }
                }

            }

            return goldcrossdate;
        }

        public string GetGoldCrossCompare5to20()
        {
            string goldcrossdate = "";

            for (int i = 0; i < data5than20.Count; i++)
            {
                if (i < data5than20.Count -1)
                {
                    var tmp = data5than20.ElementAt(i);
                    var Comparetmp = data5than20.ElementAt(i + 1);
                    if (tmp.Value != Comparetmp.Value && tmp.Value == 1)
                    {
                        goldcrossdate = tmp.Key;
                        break;

                    }
                }
                
            }

            return goldcrossdate;
        }

        public string DeathCrossCompare5to20()
        {
            string goldcrossdate = "";

            for (int i = 0; i < data5than20.Count; i++)
            {
                if (i < data5than20.Count - 1)
                {
                    var tmp = data5than20.ElementAt(i);
                    var Comparetmp = data5than20.ElementAt(i + 1);
                    if (tmp.Value != Comparetmp.Value && tmp.Value == 0)
                    {
                        goldcrossdate = tmp.Key;
                        break;

                    }
                }

            }

            return goldcrossdate;
        }

        public bool Increase5Line()
        {
            int Strd5Cnt = 0; 
            for (int i = 0; i < data5Line.Count; i ++)
            {
                var tmp = data5Line.ElementAt(i);
                if(data5Line.Count > 10)
                {
                    if (i > 0 && i < 10)
                    {
                        var Comparetmp = data5Line.ElementAt(i + 1);
                        if (tmp.Value >= Comparetmp.Value)
                        {
                            Strd5Cnt++;
                        }
                    }
                }
                
            }
            if(Strd5Cnt > 7)
            {
                return true;
            }else
            {
                return false;
            }

            
        }

        public bool Increase20Line()
        {
            int Strd20Cnt = 0;
            for (int i = 0; i < data20Line.Count; i++)
            {

                var tmp = data20Line.ElementAt(i);
                if (data20Line.Count > 10)
                {
                    if (i > 0 && i < 10)
                    {
                        var Comparetmp = data20Line.ElementAt(i + 1);
                        if (tmp.Value >= Comparetmp.Value)
                        {
                            Strd20Cnt++;
                        }
                    }
                }

            }
            if (Strd20Cnt > 7)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool WeekIncrease20Line()
        {
            int Strd20Cnt = 0;
            for (int i = 0; i < data20Line.Count; i++)
            {
                var tmp = data20Line.ElementAt(i);
                if (data5Line.Count > 5)
                {
                    if (i > 0 && i < 5)
                    {
                        var Comparetmp = data20Line.ElementAt(i + 1);
                        if (tmp.Value >= Comparetmp.Value)
                        {
                            Strd20Cnt++;
                        }
                    }
                }

            }
            if (Strd20Cnt > 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool WhichBigger5Or20()
        {
            if (data5Line.Count > 0 && data20Line.Count > 0)
            {
                if (data5Line.First().Value > data20Line.First().Value)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }

        }
        public bool WhichBigger20Or60()
        {
            if (data20Line.Count > 0 && data60Line.Count > 0)
            {
                if (data20Line.First().Value > data60Line.First().Value)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }else
            {
                return true;
            }
            
        }
    }
}
