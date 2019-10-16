using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Magrathea.ElementOperations;

namespace Magrathea.objects
{
    public class MagDateTime
    {
        private MagratheaExecutor magratheaExecutor;
        private Dictionary<string, typeObjectPair> dataValuesMap;
        private StringComparison ignoringCase = StringComparison.InvariantCultureIgnoreCase;

        public MagDateTime(MagratheaExecutor magratheaExecutor)
        {
            this.magratheaExecutor = magratheaExecutor;
            this.dataValuesMap = magratheaExecutor.getDataValuesMap();
        }

        public DateTime Now()
        {
            return DateTime.Now;
        }

        public DateTime tomorrow()
        {
            return DateTime.Now.AddDays(1);
        }

        public DateTime yesterday()
        {
            return DateTime.Now.AddDays(-1);
        }

        public object SetValue(XElement element)
        {
            // check date wanted
            DateTime dateToReturn;
            String date = null;
            try
            {
                date = element.FirstNode.ToString();
            }catch (Exception e)
            {
                date = "now";
            }
             if (date.Equals("now", ignoringCase))
            {
                dateToReturn = Now();
            }
            else if (date.Equals("tomorrow", ignoringCase))
            {
                dateToReturn = tomorrow();
            }
            else if (date.Equals("yesterday", ignoringCase))
            {
                dateToReturn = yesterday();
            }
            else
            {
                String format = element.Attribute("format").Value;
                //check if the format has a specific time in it
                String[] formats = "HH,hh".Split(',');
                Boolean hasTime = false;
                foreach (String time in formats)
                {
                    if (format.Contains(time))
                    {
                        hasTime = true;
                        break;
                    }
                }
                if (!hasTime)
                {
                    date = date + " 00:00:00";
                    format = format + " HH:mm:ss";
                }
                DateTime.TryParseExact(date, format,null,System.Globalization.DateTimeStyles.None,out dateToReturn);
            }
            return dateToReturn;
        }

        public Object dateTime(XElement element)
        {
            // find what type of dateTime operation is needed
            String elementName = element.Name.ToString();
            DateTime? dateToReturn = null;

            if (elementName.Equals("setPhaseValue", ignoringCase))
            {
                dateToReturn = (DateTime) SetValue(element);
            }
            else if (elementName.Equals("dateTime", ignoringCase))
            {
                // find what operation is required
                String type = element.Attribute("type").Value;
                if (type.Equals("addDays", ignoringCase))
                {
                    return addDays(element);
                }
                else if (type.Equals("toInt", ignoringCase))
                {
                    return toInt(element);
                }
                else if (type.Equals("toString", ignoringCase))
                {
                    return toString(element);
                }

            }
            return dateToReturn;
        }

        private Object toString(XElement element)
        {
            // find target to change
            String target = element.Attribute("target").Value;
            DateTime retrieved = (DateTime)magratheaExecutor.DataValuesMap(target).Value;
            // what part of the date to send back as int
            String format = element.Attribute("format").Value;
            if (format.Equals("dd"))
            {
                String day = retrieved.Day.ToString();
                if (day.Length < 2)
                {
                    day = "0" + day;
                }
                return day;
            }
            else if (format.Equals("d"))
            {
                String day = retrieved.Day.ToString();
                return day;
            }
            else if (format.Equals("D"))
            {
                String day = retrieved.DayOfWeek.ToString();
                return day;
            }
            else if (format.Equals("mm"))
            {
                String month = retrieved.Month.ToString();
                if (month.Length < 2)
                {
                    month = "0" + month;
                }
                return month;
            }
            else if (format.Equals("m"))
            {
                String month = retrieved.Month.ToString();
                return month;
            }
            else if (format.Equals("M"))
            {
                String month = retrieved.ToString("MMMM");
                return month;
            }
            else if (format.Equals("yyyy"))
            {
                return retrieved.Year.ToString();
            }
            else if (format.Equals("yy"))
            {
                String fullyear = retrieved.Year.ToString();
                return (fullyear.Substring(2, fullyear.Length));
            }
            else if (format.Equals("th"))
            {
                string min = retrieved.Hour.ToString();
                return min;
            }
            else if (format.Equals("tm"))
            {
                string min = retrieved.Minute.ToString();
                return min;
            }
            else if (format.Equals("ts"))
            {
                string min = retrieved.Second.ToString();
                return min;
            }
            return null;
        }

        private Object toInt(XElement element)
        {
            // find target to change
            String target = element.Attribute("target").Value;
            DateTime retrieved = (DateTime)magratheaExecutor.DataValuesMap(target).Value;
            // what part of the date to send back as int
            String format = element.Attribute("format").Value;
            if (format.Equals("dd"))
            {
                return retrieved.Day;
            }
            else if (format.Equals("mm"))
            {
                return retrieved.Month;
            }
            else if (format.Equals("yyyy"))
            {
                return retrieved.Year;
            }
            else if (format.Equals("yy"))
            {
                String fullyear = retrieved.Year.ToString();
                return Int32.Parse(fullyear.Substring(2, fullyear.Length));
            }
            return null;
        }

        public DateTime addDays(XElement element)
        {
            // get variable
            String target = element.Attribute("target").Value;
            DateTime retrieved = (DateTime)magratheaExecutor.DataValuesMap(target).Value;
            // System.out.println(retrieved);
            // get how many days to add
            int daysToAdd;
            // check for child elements
            List<XElement> children = element.Elements().ToList();
            if (children.Count >= 1)
            {
                daysToAdd = (int)magratheaExecutor.ExecuteElement(children[0], "");
            }
            else
            {
                //check for source
                String source = element.Attribute("source").Value;
                if (source != null)
                {
                    daysToAdd = (int)magratheaExecutor.DataValuesMap(source).Value;
                }
                else
                {
                    daysToAdd = Int32.Parse(element.FirstNode.ToString());
                }
            }

            magratheaExecutor.DataValuesMap(target).Value=(retrieved.AddDays(daysToAdd));
            // System.out.println((DateTime) dataValuesMap[target).Value);
            return retrieved;
        }
    }
}
