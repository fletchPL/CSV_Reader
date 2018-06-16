using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSV_Reader
{
    public class Representative
    {
        
        public string   nameOfRepresentative        { get; set; }
        public float    target                      { get; set; }
        public float    result                      { get; set; }
        public int      counter                     { get; set; }
        public int      counterHowManyNegativNumber { get; set; }
        public float    wielokrotnosc               { get; set; }
        public List<Client> clientListWithCondition = new List<Client>();
       // public List<Client> tempList;
           
        public float countTarget(List<Client> clientList)
        {
            /* tempList = clientList;*/
            //clientList.RemoveAt(0);
            foreach (Client c in clientList)
            {
                target += c.wysokoscRaty!=null ? float.Parse(c.wysokoscRaty):0; // nieprawidlowy ciag znaków
            }
            return target;
        }

        public float countResult(List<Client> clientList)
        {
           
            foreach (Client c in clientList)
            {
                result += c.ostatniaWplata!=null ? float.Parse(c.ostatniaWplata):0;
            }
            return result;
        }

        public int countHowMany(List<Client> clientList)
        {
            foreach (Client c in clientList)
            {
                if (c.ostatniaWplata != null)
                {
                    if (float.Parse(c.ostatniaWplata) != 0)
                    {
                        counter++;
                    }
                }
            }
            return counter;
        }

        public int countHowManyNegativ(List<Client> clientList)
        {
            
            foreach (Client c in clientList)
            {
                if (!String.IsNullOrEmpty(c.zaleglosci) && !String.IsNullOrEmpty(c.ostatniaWplata))
                {
                    if (float.Parse(c.zaleglosci) <= -1 && int.Parse(c.ostatniaWplata).Equals(0))
                    {
                        counterHowManyNegativNumber++;
                    }
                }
            }
            return counterHowManyNegativNumber;
        }

        public void countSixTimes(List<Client> clientList)
        {
            foreach(Client c in clientList)
            {
                if (!String.IsNullOrEmpty(c.ostatniaWplata) && !String.IsNullOrEmpty(c.wysokoscRaty))
                {
                    if (float.Parse(c.ostatniaWplata) > 6 * float.Parse(c.wysokoscRaty))
                    {
                        clientListWithCondition.Add(c);
                    }
                }
            }

            
        }

        public void generateNewFile(string path)
        {
            
            float conditionTmp;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Imie Nazwisko; Numer Telefonu ; Wielokrotnosc;");

            foreach(Client c in clientListWithCondition)
            {
                if (!String.IsNullOrEmpty(c.ostatniaWplata) && !String.IsNullOrEmpty(c.wysokoscRaty))
                {
                    conditionTmp = float.Parse(c.ostatniaWplata) / float.Parse(c.wysokoscRaty);
                    string line = c.imieNazwisko + "; " + c.telefon + "; " + conditionTmp.ToString("0.00") + "; ";
                    sb.AppendLine(line);
                }
            }
            
            File.WriteAllText(path, sb.ToString()); 

        }


    }
}
